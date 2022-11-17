using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rations_V2.Models;
using Rations_V2.Utils;
using System.Globalization;
using System.Windows;

namespace Rations_V2.ViewModels
{
    public class AddNewFoodDialogWindowViewModel : BaseViewModel
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetField(ref _title, value); }
        }

        public List<Food> DefaultFoods { get; set; } = new();

        private Food _currentFood;

        public Food CurrentFood
        {
            get { return _currentFood; }
            set 
            {
                SetField(ref _currentFood, value);
            }
        }

        public event Action<bool> OnDialogResultChanged;

        public Command DialogOkCommand { get; set; }
        public Command DialogCancelCommand { get; set; }

        private string _foodGroupName;

        private bool _editing = false;
        public bool Editing
        {
            get => _editing;
            set
            {
                SetField(ref _editing, value);
            }
        }

        private Food _editableFood;

        public Visibility FoodsVisibility => Editing ? Visibility.Hidden : Visibility.Visible;
        public AddNewFoodDialogWindowViewModel(string foodGroupName)
        {
            _foodGroupName = foodGroupName;

            Title = $"Přidat {foodGroupName}";

            DialogOkCommand = new(DialogOk);
            DialogCancelCommand = new(DialogCancel);

            PrepareFoods(foodGroupName);
        }

        public AddNewFoodDialogWindowViewModel(Food food)
        {
            Title = $"Edit {food.Name}";
            CurrentFood = food.Copy();
            _editableFood = food;
            Editing = true;

            DialogOkCommand = new(DialogOk);
            DialogCancelCommand = new(DialogCancel);

            OnPropertyChanged(nameof(FoodsVisibility));
        }

        private void PrepareFoods(string foodGroupName)
        {
            int id = Constants.FoodGroupsData.First(e => e.Item2 == foodGroupName).Item1;

            DefaultFoods = LoadFoodFromFile(id);

            DefaultFoods.Add(new()
            {
                //Name = "Vyber",
                Name = "Jiná krmiva",
                GroupName = _foodGroupName,
                Quantity = 1
            });

            CurrentFood = DefaultFoods.Last();
        }

        private void DialogOk(object parameter)
        {
            if (Editing)
            {
                _editableFood.Name = CurrentFood.Name;
                _editableFood.Quantity = CurrentFood.Quantity;
                _editableFood.ActivityCs = CurrentFood.ActivityCs;
                _editableFood.ActivitySr = CurrentFood.ActivitySr;
                _editableFood.Aminoacides = CurrentFood.Aminoacides;
                _editableFood.Calcium = CurrentFood.Calcium;
                _editableFood.Drymatter = CurrentFood.Drymatter;
                _editableFood.DProtein = CurrentFood.DProtein;
                _editableFood.EProtein = CurrentFood.EProtein;
                _editableFood.Phosphorus = CurrentFood.Phosphorus;
                _editableFood.Fibre = CurrentFood.Fibre;
                _editableFood.Nel = CurrentFood.Nel;
                _editableFood.Nev = CurrentFood.Nev;
            }

            this.OnDialogResultChanged?.Invoke(true);
        }

        private void DialogCancel(object parameter)
        {
            this.OnDialogResultChanged?.Invoke(false);
        }

        private List<Food> LoadFoodFromFile(int id)
        {
            string dataPath = FilePaths.GetFoodGroupDataPathById(id);

            List<Food> foods = new();

            if (String.IsNullOrEmpty(dataPath) || !File.Exists(dataPath))
                return foods;

            XmlDocument doc = new();
            doc.Load(dataPath);

            var nodes = doc.SelectNodes("//item");
            if (nodes != null)
            {
                CultureInfo ci = Constants.CultureInfo;
                foreach (XmlElement n in nodes)
                {
                    string name = n.GetAttribute("name");
                    double drymatter = Convert.ToDouble(n.GetAttribute("dm"), ci);
                    double aminoacides = Convert.ToDouble(n.GetAttribute("aac"), ci);
                    double fibre = Convert.ToDouble(n.GetAttribute("fibre"), ci);
                    double sugar = Convert.ToDouble(n.GetAttribute("sugar"), ci);
                    double dProtein = Convert.ToDouble(n.GetAttribute("dProtein"), ci);
                    double eProtein = Convert.ToDouble(n.GetAttribute("eProtein"), ci);
                    double calcium = Convert.ToDouble(n.GetAttribute("ca"), ci);
                    double phosphorus = Convert.ToDouble(n.GetAttribute("p"), ci);
                    double nel = Convert.ToDouble(n.GetAttribute("nel"), ci);
                    double nev = Convert.ToDouble(n.GetAttribute("nev"), ci);

                    Food f = new()
                    {
                        Name = name,
                        GroupName = _foodGroupName,
                        Drymatter = drymatter,
                        Aminoacides = aminoacides,
                        Fibre = fibre,
                        DProtein = dProtein,
                        EProtein = eProtein,
                        Calcium = calcium,
                        Phosphorus = phosphorus,
                        Nel = nel,
                        Nev = nev
                    };

                    foods.Add(f);
                }
            }

            return foods;
        }
    }
}
