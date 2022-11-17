using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rations_V2.ViewModels
{
    internal class CalculationValuesDialogViewModel : BaseViewModel
    {
        public static event Action OnMeasureOfInaccuracyChanged;

        private double _measureOfInaccuracy = Settings.Standart.Default.MeasureOfInaccuracy;

        public double MeasureOfInaccuracy
        {
            get { return _measureOfInaccuracy; }
            set { SetField(ref _measureOfInaccuracy, value); }
        }

        public event Action<bool> OnDialogResultChanged;

        public Command OkCommand { get; set; }
        public Command CancelCommand { get; set; }

        public CalculationValuesDialogViewModel()
        {
            OkCommand = new(OkDialogResult);
            CancelCommand = new(CancelDialogResult);
        }

        private void OkDialogResult(object parameter)
        {
            Settings.Standart.Default.MeasureOfInaccuracy = _measureOfInaccuracy;
            Settings.Standart.Default.Save();

            OnDialogResultChanged?.Invoke(true);
            OnMeasureOfInaccuracyChanged?.Invoke();
        }

        private void CancelDialogResult(object parameter)
        {
            OnDialogResultChanged?.Invoke(false);
        }
    }
}
