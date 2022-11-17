using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Rations_V2.Models
{
    [Serializable]
    public abstract class Cow : ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public int Quantity { get; set; }
        public double AverageDailyGain { get; set; }

        public List<FoodGroup> FoodGroups { get; set; }

        public ObservableCollection<Food> AllFood
        {
            get
            {
                ObservableCollection<Food> foods = new();
                foreach (var fg in FoodGroups)
                    foreach(var f in fg.Foods)
                    {
                        foods.Add(f);
                    }

                return foods;
            }
        }

        public Cow(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public virtual void Cruth() { } // Костыль

        #region StandartData grab methods
        protected string StandartDataFileName;

        public virtual void LoadStandartData()
        {
            // TODO Make more better search in xml docuemnt

            string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Cows", StandartDataFileName);

            XmlDocument doc = new();
            doc.Load(dataPath);

            var nodes = doc.SelectNodes("//weigth");

            if (nodes == null)
                return;

            var ci = Constants.CultureInfo;

            foreach (XmlElement w in nodes)
            {
                double weight = Convert.ToDouble(w.GetAttribute("data"));

                var dataNodes = w.ChildNodes;

                if (dataNodes == null)
                    continue;

                Dictionary<double, Food> stds = new();

                foreach (XmlElement d in dataNodes)
                {
                    double upperLimit = Convert.ToDouble(d.GetAttribute("data"), ci);

                    XmlNodeList childs = d.ChildNodes;

                    double energy = Convert.ToDouble(childs[0].InnerText, ci);
                    double pdi = Convert.ToDouble(childs[1].InnerText, ci);
                    double c = Convert.ToDouble(childs[2].InnerText, ci);
                    double p = Convert.ToDouble(childs[3].InnerText, ci);
                    double dm = Convert.ToDouble(childs[4].InnerText, ci);
                    double a = Convert.ToDouble(childs[5].InnerText, ci);
                    double f = Convert.ToDouble(childs[6].InnerText, ci);

                    Food food = new()
                    {
                        DProtein = pdi,
                        EProtein = pdi,
                        Calcium = c,
                        Phosphorus = p,
                        Drymatter = dm,
                        Aminoacides = a,
                        Fibre = f,
                        Nel = energy
                    };

                    stds.Add(upperLimit, food);
                }

                _standarts.Add(weight, stds);
            }
        }

        protected Dictionary<double, Dictionary<double, Food>> _standarts = new();

        public virtual Food GetStandart()
        {
            // TODO Add file and standarts count check before taing values

            double firstKey = _standarts.Keys.First();

            foreach (var k in _standarts.Keys.Reverse())
            {
                if (Weight >= k)
                {
                    firstKey = k;
                    break;
                }
            }

            var data = _standarts[firstKey];

            double secondKey = data.Keys.First();

            foreach (var k in data.Keys.Reverse())
            {
                if (AverageDailyGain >= k)
                {
                    secondKey = k;
                    break;
                }
            }

            return _standarts[firstKey][secondKey];
        }
        #endregion

        #region Методы рассчетов
        private IEnumerable<FoodGroup> _usedGroups => FoodGroups.Where(g => g.SelectedFood != null);

        public virtual double CalculateActivityCs()
        {
            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.ActivityCs;
                }


            return Math.Round(result, 0);
        }

        public virtual double CalculateActivitySr()
        {
            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.ActivitySr;
                }

            return Math.Round(result, 0);
        }

        public virtual double CalculateDrymatter()
        {
            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.Drymatter;
                }

            return Math.Round(result / 1000, 1);
        }

        public virtual double CalculateAminoacides()
        {
            double val = CalculateDrymatter();

            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.Aminoacides;
                }

            result *= val;

            return Math.Round(result, 0);
        }

        public virtual double CalculateDProtein()
        {
            double val = CalculateDrymatter();

            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.DProtein;
                }

            result *= val;

            return Math.Round(result, 0);
        }
        public virtual double CalculateEProtein()
        {
            double val = CalculateDrymatter();

            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.EProtein;
                }

            result *= val;

            return Math.Round(result, 0);
        }

        public virtual double CalculatePhosphorus()
        {
            double val = CalculateDrymatter();

            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.Phosphorus;
                }

            result *= val;

            return Math.Round(result, 0);
        }

        public virtual double CalculateCalcium()
        {
            double val = CalculateDrymatter();

            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.Calcium;
                }

            result *= val;

            return Math.Round(result, 0);
        }

        public virtual double CalculateFibre()
        {
            double val = CalculateDrymatter();

            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.Fibre;
                }

            result *= val;

            return Math.Round(result / 1000, 1);
        }

        public virtual double CalculateNel()
        {
            double val = CalculateDrymatter();

            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.Nel;
                }

            result *= val;

            return Math.Round(result, 1);
        }

        public virtual double CalculateNev()
        {
            double val = CalculateDrymatter();

            double result = 0;

            foreach (var fg in FoodGroups)
                foreach (var f in fg.Foods)
                {
                    result += f.Amount * f.Nev;
                }

            result *= val;

            return Math.Round(result, 1);
        }
        #endregion

        #region Методы рассчета стандартных значений
        public virtual double CalculateStandartActivityCs()
        {
            return 0;
        }

        public virtual double CalculateStandartActivitySr()
        {
            return 0;
        }
        #endregion

        public virtual IEnumerable<Tuple<string, SolidColorBrush>> BuildInfoText()
        {
            List<Tuple<string, SolidColorBrush>> cs_info = new();

            var deltaCs = CalculateActivityCs() - CalculateStandartActivityCs();

            SolidColorBrush color;
            string text;
            if (deltaCs > 0)
            {
                color = Brushes.Red;
                text = "Aktivita pro Cs-137 v krmné dávce způsobí PŘEKROČENÍ NEJVYŠŠÍ PŘÍPUSTNÉ ÚROVNĚ";
            }
            else if (deltaCs < 0)
            {
                color = Brushes.Green;
                text = "Aktivita Cs-137 v krmné dávce VYHOVUJÍCÍ";
            }
            else
            {
                color = Brushes.Orange;
                text = "Pro aktivitu Cs-137 v krmné dávce MAXIMÁLNÍ HODNOTA";
            }

            cs_info.Add(Tuple.Create(text, color));

            return cs_info;
        }

        public virtual double CalculateFoodAmount()
        {
            return 0;
        }

        #region Serialization

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", Id, typeof(int));
            info.AddValue("name", Name, typeof(string));
            info.AddValue("weight", Weight, typeof(double));
            info.AddValue("quantity", Quantity, typeof(int));
            info.AddValue("avgDailyGain", AverageDailyGain, typeof(double));
            info.AddValue("foodGroups", FoodGroups, typeof(List<FoodGroup>));
            info.AddValue("stdDataFile", StandartDataFileName, typeof(string));
        }

        public Cow(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("id");
            Name = info.GetString("name");
            Weight = info.GetInt32("weight");
            Quantity = info.GetInt32("quantity");
            AverageDailyGain = info.GetDouble("avgDailyGain");
            FoodGroups = (List<FoodGroup>)info.GetValue("foodGroups", typeof(List<FoodGroup>));

            StandartDataFileName = info.GetString("stdDataFile");
        }

        #endregion
    }
}
