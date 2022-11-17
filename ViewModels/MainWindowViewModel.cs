using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

using Rations_V2.Views;
using Rations_V2.Views.Dialogs;
using Rations_V2.Models;
using System.IO;
using Microsoft.Win32;

namespace Rations_V2.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public static NavigationService NavigationService { get; private set; }

        private string CurrentFile = null;

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetField(ref _title, value); }
        }


        public MainWindowViewModel(NavigationService navService)
        {
            NewFileCommand = new(NewFile);
            OpenFileCommand = new(OpenFile);
            SaveFileCommand = new(SaveFile);
            EditStandartsCommand = new(EditStandarts);
            EditCalculationValuesCommand = new(EditCalculationValues);
            CloseWindowCommand = new(CloseWindow);
            

           NavigationService = navService;

            Title = Constants.AppName;
        }

        public void WindowLoaded()
        {
            IndexPage page = new();

            NavigationService.Navigate(page);
        }
        

        public Command NewFileCommand { get; set; }
        public Command OpenFileCommand { get; set; }
        public Command SaveFileCommand { get; set; }
        
        public Command EditStandartsCommand { get; set; }
        public Command EditCalculationValuesCommand { get; set; }
        
        public Command CloseWindowCommand { get; private set; }

        private void NewFile(object parameter)
        {
            DataPage page = new();

            NavigationService.Navigate(page);
           // Title = $"PRiMM - Nový soubor";
            Title = $"{Constants.AppName} - Nový soubor";
        }

        private void OpenFile(object parameter)
        {
            OpenFileDialog fd = new();
            fd.Filter = "Rations document (.primm)|*.primm";

            if (fd.ShowDialog() != true)
                return;

            SaveInfo info;

            using (FileStream fs = new FileStream(fd.FileName, FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();

                info = formatter.Deserialize(fs) as SaveInfo;
            }

            DataPage page = new(info);

            NavigationService.Navigate(page);

            DateTime created = File.GetCreationTime(fd.FileName);

            Title = $"{ Constants.AppName} - {Path.GetFileNameWithoutExtension(fd.FileName)} | {created.ToString("dd.MM.yyyy")}";
        }

        private void SaveFile(object parameter)
        {
            // TODO Add feature to save current file without overwrite
            if (NavigationService.Content == null)
                return;

            if (NavigationService.Content.GetType() != typeof(DataPage))
                return;

            SaveFileDialog fd = new();
            fd.FileName = "ration";
            fd.DefaultExt = ".primm";
            fd.Filter = "Rations document (.primm)|*.primm";
            fd.AddExtension = true;

            if (fd.ShowDialog() != true)
                return;

            DataPage page = NavigationService.Content as DataPage;

            var saveInfo = page.GetSaveInfo();

            IFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream(fd.FileName, FileMode.Create))
            {
                formatter.Serialize(fs, saveInfo);
            }

            DateTime created = File.GetCreationTime(fd.FileName);

            Title = $"{Constants.AppName} - {Path.GetFileNameWithoutExtension(fd.FileName)} | {created.ToString("dd.MM.yyyy")}";
        }

        private void EditStandarts(object parameter)
        {
            StandartsDialogWindow win = new();

            win.ShowDialog();
        }

        private void EditCalculationValues(object parameter)
        {
            CalculationValuesDialogWindow win = new CalculationValuesDialogWindow();

            win.ShowDialog();
        }

        private void CloseWindow(object parameter)
        {
            ((MainWindow)parameter).Close();
        }
    }
}
