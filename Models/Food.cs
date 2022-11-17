using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rations_V2.Models
{
    [Serializable]
    public class Food : ISerializable, INotifyPropertyChanged
    {
        private static int IdCounter = 1;

        public int Id;

        private bool _selected = false;
        public bool Selected
        {
            get => _selected;
            set
            {
                SetField(ref _selected, value);
                SelectionChanged?.Invoke(value, this);
            }
        }

        public event Action<bool, Food> SelectionChanged;

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                SetField(ref _name, value);
            }
        }
        public string GroupName { get; set; }

        private double _qunatity;
        public double Quantity
        {
            get => _qunatity;
            set => SetField(ref _qunatity, value);
        }
        public double Amount { get; set; }
        public double ActivityCs { get; set; }
        public double ActivitySr { get; set; }
        public double Drymatter { get; set; }
        public double Aminoacides { get; set; }
        public double DProtein { get; set; }
        public double EProtein { get; set; }
        public double Phosphorus { get; set; }
        public double Calcium { get; set; }
        public double Fibre { get; set; }
        public double Nel { get; set; }
        public double Nev { get; set; }

        public Food()
        {
            Id = IdCounter;
            IdCounter++;
        }

        public static Food operator -(Food first, Food second)
        {
            Food f = new Food();

            f.ActivityCs = Math.Round(first.ActivityCs - second.ActivityCs, 2);
            f.ActivitySr = Math.Round(first.ActivitySr - second.ActivitySr, 2);
            f.Drymatter = Math.Round(first.Drymatter - second.Drymatter, 2);
            f.Aminoacides = Math.Round(first.Aminoacides - second.Aminoacides, 2);
            f.DProtein = Math.Round(first.DProtein - second.DProtein, 2);
            f.EProtein = Math.Round(first.EProtein - second.EProtein, 2);
            f.Phosphorus = Math.Round(first.Phosphorus - second.Phosphorus, 2);
            f.Calcium = Math.Round(first.Calcium - second.Calcium, 2);
            f.Fibre = Math.Round(first.Fibre - second.Fibre, 2);
            f.Nel = Math.Round(first.Nel - second.Nel, 2);
            f.Nev = Math.Round(first.Nev - second.Nev, 2);

            return f;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("idCounter", IdCounter, typeof(int));
            info.AddValue("id", Id, typeof(int));
            info.AddValue("name", Name, typeof(string));
            info.AddValue("groupName", GroupName, typeof(string));
            info.AddValue("quantity", Quantity, typeof(double));
            info.AddValue("amount", Amount, typeof(double));
            info.AddValue("activityCs", ActivityCs, typeof(double));
            info.AddValue("activitySr", ActivitySr, typeof(double));
            info.AddValue("drymatter", Drymatter, typeof(double));
            info.AddValue("aminoacides", Aminoacides, typeof(double));
            info.AddValue("dProtein", DProtein, typeof(double));
            info.AddValue("eProtein", EProtein, typeof(double));
            info.AddValue("phosphorus", Phosphorus, typeof(double));
            info.AddValue("calcium", Calcium, typeof(double));
            info.AddValue("fibre", Fibre, typeof(double));
            info.AddValue("nel", Nel, typeof(double));
            info.AddValue("nev", Nev, typeof(double));
        }

        public Food(SerializationInfo info, StreamingContext context)
        {
            IdCounter = info.GetInt32("idCounter");
            Id = info.GetInt32("id");
            Name = info.GetString("name");
            GroupName = info.GetString("groupName");
            Quantity = info.GetDouble("quantity");
            Amount = info.GetDouble("amount");
            ActivityCs = info.GetDouble("activityCs");
            ActivitySr = info.GetDouble("activitySr");
            Drymatter = info.GetDouble("drymatter");
            Aminoacides = info.GetDouble("aminoacides");
            DProtein = info.GetDouble("dProtein");
            EProtein = info.GetDouble("eProtein");
            Phosphorus = info.GetDouble("phosphorus");
            Calcium = info.GetDouble("calcium");
            Fibre = info.GetDouble("fibre");
            Nel = info.GetDouble("nel");
            Nev = info.GetDouble("nev");
        }

        public void SetSelected(bool value)
        {
            _selected = value;
            OnPropertyChanged(nameof(Selected));
        }

        public virtual Food Copy()
        {
            return new Food()
            {
                Id = Id,
                Selected = Selected,
                Name = Name,
                GroupName = GroupName,
                Quantity = Quantity,
                Amount = Amount,
                ActivityCs = ActivityCs,
                ActivitySr = ActivitySr,
                Drymatter = Drymatter,
                Aminoacides = Aminoacides,
                DProtein = DProtein,
                EProtein = EProtein,
                Phosphorus = Phosphorus,
                Calcium = Calcium,
                Fibre = Fibre,
                Nel = Nel,
                Nev = Nev
            };
        }

        public virtual void Edit(Food f)
        {
            Id = f.Id;
            Selected = f.Selected;
            Name = f.Name;
            GroupName = f.GroupName;
            Quantity = f.Quantity;
            ActivityCs = f.ActivityCs;
            ActivitySr = f.ActivitySr;
            Drymatter = f.Drymatter;
            Aminoacides = f.Aminoacides;
            DProtein = f.DProtein;
            EProtein = f.EProtein;
            Phosphorus = f.Phosphorus;
            Calcium = f.Calcium;
            Fibre = f.Fibre;
            Nel = f.Nel;
            Nev = f.Nev;
        }

        #region Property Changed
        public event PropertyChangedEventHandler? PropertyChanged;

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
