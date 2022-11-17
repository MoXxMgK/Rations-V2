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
    public class FoodGroup : ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }

        public Food SelectedFood { get; set; }

        public ObservableCollection<Food> Foods { get; set; }

        public FoodGroup(int id, string name, ObservableCollection<Food> foods)
        {
            Id = id;
            Name = name;
            Foods = foods;
        }

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", Id, typeof(int));
            info.AddValue("name", Name, typeof(string));
            info.AddValue("amount", Amount, typeof(double));
            info.AddValue("foods", Foods, typeof(ObservableCollection<Food>));
            info.AddValue("selectedFood", SelectedFood, typeof(Food));
        }

        public FoodGroup(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("id");
            Name = info.GetString("name");
            Amount = info.GetDouble("amount");
            Foods = info.GetValue("foods", typeof(ObservableCollection<Food>)) as ObservableCollection<Food>;
            SelectedFood = info.GetValue("selectedFood", typeof(Food)) as Food;
        }
        #endregion
    }
}
