using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rations_V2.Models
{
    [Serializable]
    public class Heifers12_24 : NevEnergyCow
    {
        public Heifers12_24() : base(Constants.IdHeifers12_24, "Jalovice starší než 1 rok")
        {
            StandartDataFileName = "Heifers 12-24.xml";
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

        public Heifers12_24(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override double CalculateStandartActivityCs()
        {
            double error = (1 - (Settings.Standart.Default.MeasureOfInaccuracy / 100)) * (150 / Constants.TF_MEAT_12_CS);
            return Math.Round(Settings.Standart.Default.StandartMeatActivityCs * error, 0);
        }
    }
}
