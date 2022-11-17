using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Rations_V2.ViewModels;
using Rations_V2.Models;
using Rations_V2.Views.Dialogs;

namespace Rations_V2.Views
{
    /// <summary>
    /// Логика взаимодействия для DataPage.xaml
    /// </summary>
    public partial class DataPage : Page
    {
        private DataPageViewModel _vm;

        public DataPage()
        {
            InitializeComponent();

            _vm = new();

            DataContext = _vm;

            BindEvents();
        }

        public DataPage(SaveInfo info)
        {
            InitializeComponent();

            _vm = new(info);

            DataContext = _vm;

            BindEvents();
        }

        private void BindEvents()
        {
            CalculationValuesDialogViewModel.OnMeasureOfInaccuracyChanged += () => _vm.UpdateResults(InfoText.Inlines);
            StandartsDialogWindowViewModel.OnStandartsChanged += () => _vm.UpdateResults(InfoText.Inlines);
        }

        private void DataChanged(object sender, TextChangedEventArgs e)
        {
            // Вызов обновления таблицы при изменении количества корма
            _vm.UpdateResults(InfoText.Inlines);
        }

        private void CheckboxDataChanged(object sender, RoutedEventArgs e)
        {
            _vm.UpdateResults(InfoText.Inlines);
        }

        public SaveInfo GetSaveInfo()
        {
            return _vm.GetSaveInfo();
        }

        private void ComboBoxDataChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm.UpdateResults(InfoText.Inlines);
        }
    }
}
