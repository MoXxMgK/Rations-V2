using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rations_V2.Models
{
    [Serializable]
    public class SaveInfo : ISerializable
    {
        public List<Cow> Cows { get; init; }
        public Dictionary<string, ObservableCollection<Food>> FoodsCollection { get; init; }

        public SaveInfo() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("foodsCollection", FoodsCollection, typeof(Dictionary<string, ObservableCollection<Food>>));
            info.AddValue("cows", Cows, typeof(List<Cow>));
        }

        public SaveInfo(SerializationInfo info, StreamingContext context)
        {
            FoodsCollection = info.GetValue("foodsCollection", typeof(Dictionary<string, ObservableCollection<Food>>))
                as Dictionary<string, ObservableCollection<Food>>;
            Cows = info.GetValue("cows", typeof(List<Cow>)) as List<Cow>;
            
        }
    }
}
