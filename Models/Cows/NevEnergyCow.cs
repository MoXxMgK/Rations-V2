using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Rations_V2.Models
{
    [Serializable]
    public class NevEnergyCow : Cow
    {
        public NevEnergyCow(int id, string name) : base(id, name)
        {

        }

        public override void LoadStandartData()
        {
            base.LoadStandartData();
            foreach(var kvpair1 in _standarts)
            {
                foreach(var kvpair2 in kvpair1.Value)
                {
                    var f = kvpair2.Value;
                    f.Nev = f.Nel;
                    f.Nel = 0;
                }
            }
        }

        public NevEnergyCow(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override double CalculateFoodAmount()
        {
            var dm = 0.233 + (0.077 * Math.Pow(Weight, 0.75)) + (0.980 * AverageDailyGain);

            var dryFood = dm * 0.25; // Сено, солома
            var wetFood = dm * 0.40; // ЗМ, силос, сенаж, корнеплоды
            var concFood = dm * 0.30; // концентраты 

            return 0;
        }
    }
}
