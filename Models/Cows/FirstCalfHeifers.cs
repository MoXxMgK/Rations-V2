using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace Rations_V2.Models
{
    [Serializable]
    public class FirstCalfHeifers : NelEnergyCow
    {
        public FirstCalfHeifers() : base(Constants.IdFirstCalfHeifers, "Prvotelky")
        {
            StandartDataFileName = "First-calf heifers.xml";
        }

        public override void Cruth()
        {
            // Костыль
            double[] amounts = new double[] { 0, 0, 3, 0, 0, 0, 20, 5, 10, 1, 0, 0 };
            
            for (int i = 0; i < 12; i++)
            {
                var g = FoodGroups.ElementAt(i);
                g.Amount = amounts[i];
            }
        }

        public FirstCalfHeifers(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override double CalculateStandartActivityCs()
        {
            double error = (1 - (Settings.Standart.Default.MeasureOfInaccuracy / 100)) * (100 / Constants.TF_MILK_CS);
            return Math.Round(Settings.Standart.Default.StandartMilkActivityCs * error, 0);
        }

        public override double CalculateStandartActivitySr()
        {
            double error = (1 - (Settings.Standart.Default.MeasureOfInaccuracy / 100)) * (100 / Constants.TF_MILK_SR);
            return Math.Round(Settings.Standart.Default.StandartMilkActivitySr * error, 0);
        }
    }
}
