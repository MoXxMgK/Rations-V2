using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace Rations_V2.Models
{
    [Serializable]
    public class NelEnergyCow : Cow
    {
        public NelEnergyCow(int id, string name) : base(id, name)
        {

        }

        public NelEnergyCow(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override Food GetStandart()
        {
            var f = base.GetStandart();

            f.Nev = 0;

            return f;
        }

        public override IEnumerable<Tuple<string, SolidColorBrush>> BuildInfoText()
        {
            var baseInfo = base.BuildInfoText().ToList();

            var cs = CalculateActivityCs();
            var sr = CalculateActivitySr();

            double error = Settings.Standart.Default.MeasureOfInaccuracy / 100;

            baseInfo.Add(Tuple.Create($"Odhadnutá objemová aktivita Cs-137 v mléce: {cs * 0.01} ± {Math.Round(cs * 0.01 * error, 0)} [Bq/l]", baseInfo.First().Item2));

            var deltaSr = sr - CalculateStandartActivitySr();

            SolidColorBrush color;
            string text;

            if (deltaSr > 0)
            {
                color = Brushes.Red;
                text = "Aktivity  Sr-90 v krmné dávce způsobí PŘEKROČENÍ NEJVYŠŠÍ PŘÍPUSTNÉ ÚROVNĚ";
            }
            else if (deltaSr < 0)
            {
                color = Brushes.Green;
                text = "Aktivita Sr-90 v krmné dávce VYHOVUJÍCÍ";
            }
            else
            {
                color = Brushes.Orange;
                text = "Pro aktivitu Sr-90 v krmné dávce MAXIMÁLNÍ HODNOTA ";
            }

            baseInfo.Add(Tuple.Create(text, color));
            baseInfo.Add(Tuple.Create($"Odhadnutá objemová aktivita Sr-90 v mléce: {Math.Round(sr * 0.0016, 1)} ± {Math.Round(sr * 0.0016 * error, 1)} [Bq/l]", color));

            return baseInfo;
        }
    }
}
