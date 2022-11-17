using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Rations_V2.Models
{
    [Serializable]
    public class Fattening : NevEnergyCow, INotifyPropertyChanged
    {
        public double LiveMeasurement { get; set; }
        private int _daysBeforeSlaughter;
        public int DaysBeforeSlaughter
        {
            get => _daysBeforeSlaughter;
            set => SetField(ref _daysBeforeSlaughter, value);
        }

        private bool _manualInputAllowed = false;
        public bool ManualDaysInputAllowed
        {
            get => _manualInputAllowed;
            set => SetField(ref _manualInputAllowed, value);
        }
        public bool UseFerrocianides { get; set; } = false;

        public Fattening() : base(Constants.IdFattening, "Výkrm")
        {
            StandartDataFileName = "Fattening.xml";
        }

        #region Calculation
        private double CalculateSk(int days)
        {
            return Constants.AL * Math.Exp(-0.693 * days / Constants.T1) + (1 - Constants.AL) * Math.Exp(-0.693 * days / Constants.T2);
        }

        public override void Cruth()
        {
            // Костыль
            double[] amounts = new double[] { 0, 0, 2, 0, 0, 0, 0, 2, 20, 0, 0, 0 };
            
            for (int i = 0; i < 12; i++)
            {
                var g = FoodGroups.ElementAt(i);
                g.Amount = amounts[i];
            }
        }

        public override double CalculateStandartActivityCs()
        {
            double baseResult = base.CalculateActivityCs();

            CalculateDaysBeforeSlaughter(baseResult);

            double error = 1 - (Settings.Standart.Default.MeasureOfInaccuracy / 100);
            // TODO Move this to separate function
            double rd = Settings.Standart.Default.StandartMeatActivityCs * error;
            double sk = CalculateSk(DaysBeforeSlaughter);
            double fun = (LiveMeasurement * sk - rd) / (Constants.KP_MEAT * (sk - 1));

            double ogr = UseFerrocianides ? fun * 1.5 : fun;

            double result = Math.Round(ogr, 2);

            return Math.Clamp(result, 0, 50000);
        }

        private void CalculateDaysBeforeSlaughter(double baseRationActivity)
        {
            if (ManualDaysInputAllowed)
                return;
            double error = 1 - (Settings.Standart.Default.MeasureOfInaccuracy / 100);
            // Move error in single function
            double rd = Settings.Standart.Default.StandartMeatActivityCs * error;
            double fun = Settings.Standart.Default.StandartMeatActivityCs;

            int days = 1;
            while (days <= 60)
            {
                double sk = CalculateSk(days);
                fun = (Constants.KP_MEAT * baseRationActivity) + (LiveMeasurement - Constants.KP_MEAT * baseRationActivity) * sk;

                if (fun <= rd)
                    break;

                days++;
            }
            if (days >= 60)
            {
                // Show warning text about this
            }
            DaysBeforeSlaughter = days;
        }

        public override IEnumerable<Tuple<string, SolidColorBrush>> BuildInfoText()
        {
            var baseInfo =  base.BuildInfoText().ToList();

            SolidColorBrush color;
            string text;

            double error =  Settings.Standart.Default.MeasureOfInaccuracy / 100;

            double cs = CalculateActivityCs();
            var val = (Constants.KP_MEAT * cs) + (LiveMeasurement - Constants.KP_MEAT * cs) * CalculateSk(DaysBeforeSlaughter);
            color = baseInfo.First().Item2;
            text = $"Odhadnutá hmotnostní aktivita Cs-137 v mase: {Math.Round(val, 0)} ± {Math.Round(val*error, 0)} [Bq/kg] po {DaysBeforeSlaughter} dnech";

            /*
            if (DaysBeforeSlaughter <= 60 && !ManualDaysInputAllowed)
            {
                color = Brushes.Green;
                text = $"Estimated value of Cs-137 content in meat: {Settings.Standart.Default.StandartMeatActivityCs * 0.8} Bq/kg after {DaysBeforeSlaughter} days";
            }
            else
            {
                double cs = CalculateActivityCs();
                var val = (Constants.KP_MEAT * cs) + (LiveMeasurement - Constants.KP_MEAT * cs) * CalculateSk(DaysBeforeSlaughter);
                color = Brushes.Black;
                text = $"Estimated value of Cs-137 content in meat: {Math.Round(val, 0)} Bq/kg";
            }
            */

            baseInfo.Add(Tuple.Create(text, color));

            return baseInfo;
        }

        #endregion

        #region Serialization
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("liveMeasurement", LiveMeasurement, typeof(double));
            info.AddValue("daysBeforeSlaughter", DaysBeforeSlaughter, typeof(int));
            info.AddValue("manualDaysInputAllowed", ManualDaysInputAllowed, typeof(bool));
            info.AddValue("useFerrocianides", UseFerrocianides, typeof(bool));
        }

        public Fattening(SerializationInfo info, StreamingContext context) : base (info, context)
        {
            LiveMeasurement = info.GetDouble("liveMeasurement");
            DaysBeforeSlaughter = info.GetInt32("daysBeforeSlaughter");
            ManualDaysInputAllowed = info.GetBoolean("manualDaysInputAllowed");
            UseFerrocianides = info.GetBoolean("useFerrocianides");
        }

        #endregion

        #region PropertyChanged
        // Maybe its better to move this to Cow class to add this functionaity to all cows,
        // or maybe not
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
