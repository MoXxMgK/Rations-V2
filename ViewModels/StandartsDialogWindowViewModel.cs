using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rations_V2.ViewModels
{
    public class StandartsDialogWindowViewModel : BaseViewModel
    {
        public static event Action OnStandartsChanged;

        private double _milkActivityCs;

        public double MilkActivityCs
        {
            get { return _milkActivityCs; }
            set { SetField(ref _milkActivityCs, value); }
        }

        private double _milkActivitySr;

        public double MilkActivitySr
        {
            get { return _milkActivitySr; }
            set { SetField(ref _milkActivitySr, value); }
        }

        private double _meatActivityCs;

        public double MeatActivityCs
        {
            get { return _meatActivityCs; }
            set { SetField(ref _meatActivityCs, value); }
        }

        public event Action<bool> DialogResultChanged;

        public Command DialogSaveCommand { get; set; }
        public Command DialogCancelCommand { get; set; }

        public StandartsDialogWindowViewModel()
        {
            DialogSaveCommand = new(Save);
            DialogCancelCommand = new(Cancel);

            var settings = Settings.Standart.Default;

            MilkActivityCs = settings.StandartMilkActivityCs;
            MilkActivitySr = settings.StandartMilkActivitySr;
            MeatActivityCs = settings.StandartMeatActivityCs;
        }

        private void Save(object parameter)
        {
            var settings = Settings.Standart.Default;

            settings.StandartMilkActivityCs = MilkActivityCs;
            settings.StandartMilkActivitySr = MilkActivitySr;
            settings.StandartMeatActivityCs = MeatActivityCs;

            settings.Save();

            OnStandartsChanged?.Invoke();
            DialogResultChanged?.Invoke(true);
        }

        private void Cancel(object parameter)
        {
            DialogResultChanged?.Invoke(false);
        }
    }
}
