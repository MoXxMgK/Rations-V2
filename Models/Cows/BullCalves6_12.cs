using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rations_V2.Models
{
    [Serializable]
    public class BullCalves6_12 : NevEnergyCow
    {
        public BullCalves6_12() : base(Constants.IdBullCalves6_12, "Býčci, 6 – 12 měsíců")
        {
            StandartDataFileName = "Bull calves 6-12.xml";
        }

        public override void Cruth()
        {
            // Костыль
            double[] amounts = new double[] { 0, 0, 2.3, 0, 0, 0.5, 0, 1, 10, 0, 0, 0 };
           
            for (int i = 0; i < 12; i++)
            {
                var g = FoodGroups.ElementAt(i);
                g.Amount = amounts[i];
            }
        }

        public BullCalves6_12(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override double CalculateStandartActivityCs()
        {
            double error = (1 - (Settings.Standart.Default.MeasureOfInaccuracy / 100)) * (150 / Constants.TF_MEAT_6_12_CS);
            return Math.Round(Settings.Standart.Default.StandartMeatActivityCs * error, 0);
        }
    }
}
