using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Rations_V2.Models;
using Rations_V2.Views;
using Rations_V2.Views.Dialogs;

namespace Rations_V2.ViewModels
{
    public class DataPageViewModel : BaseViewModel
    {
        // Список имен всех групп кормов для отображения в выпадающем списке
        public List<string> FoodGroupNamesCollection { get; set; } = new();

        // Выбранная в данныый момент группа кормов. Для использования в добавлении и т.д.
        private string _selectedFoodGroup;

        public string SelectedFoodGroup
        {
            get { return _selectedFoodGroup; }
            set 
            {
                SetField(ref _selectedFoodGroup, value);
                SelectedFoodCollection = FoodsCollection[value];
                OnPropertyChanged(nameof(SelectedFoodCollection));
            }
        }

        // Коллекция кормов на основе текущей выбранной группы
        public ObservableCollection<Food> SelectedFoodCollection { get; set; } = new();

        // Список всех доступных животных
        public List<Cow> Cows { get; set; } = new();

        // Текущее выбранное животное
        private Cow _selectedCow;

        public Cow SelectedCow
        {
            get { return _selectedCow; }
            set 
            {
                SetField(ref _selectedCow, value);
                UpdateCowRation();
            }
        }

        // Коллекция групп кормов с не нулевым количеством кормов внутри
        public ObservableCollection<Food> CowRationCollection { get; set; } = new();

        //Коллекция для хранения результатов вычислений по выбранной группе животных
        public ObservableCollection<Tuple<string, Food>> ResultsCollection { get; set; } = new();

        // Словарь со всеми коллекция групп кормов, распределенные для всех животных
        private Dictionary<string, ObservableCollection<Food>> FoodsCollection = new();

        // Свойство видимости для таблиц рациона животного. Не видно если нет корма
        public Visibility RationsTableVisibility
        {
            get
            {
                if (SelectedCow.FoodGroups.Any(fg => fg.Foods.Count > 0))
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public Visibility ResultsTableVisibility
        {
            get
            {
                if (RationsTableVisibility == Visibility.Visible && SelectedCow.Quantity > 0)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        private InlineCollection _infoText;

        public InlineCollection InfoText
        {
            get { return _infoText; }
            set { SetField(ref _infoText, value); }
        }


        // Команда для открытия диалогового окна добавления нового корма
        public Command AddNewFoodCommand { get; set; }
        public Command DeleteFoodCommand { get; set; }
        public Command EditFoodCommand { get; set; }
        public Command ShowResultsCommand { get; set; }

        public DataPageViewModel()
        {
            Init();

            Cows.Add(new LactatingCows());
            Cows.Add(new FirstCalfHeifers());
            Cows.Add(new Heifers6_12());
            Cows.Add(new Heifers12_24());
            Cows.Add(new BullCalves6_12());
            Cows.Add(new BullCalves12_24());
            Cows.Add(new Fattening());

            foreach (var cow in Cows)
            {
                List<FoodGroup> foodGroups = new();

                foreach (var fgd in Constants.FoodGroupsData)
                {
                    int id = fgd.Item1;
                    string name = fgd.Item2;

                    ObservableCollection<Food> foods = FoodsCollection[name];

                    foodGroups.Add(new(id, name, new ObservableCollection<Food>()));
                }

                cow.FoodGroups = foodGroups;

                cow.LoadStandartData();
            }

            foreach (var cow in Cows)
                cow.Cruth();

            SelectedCow = Cows[0];
            SelectedFoodGroup = FoodGroupNamesCollection[0];
        }

        public DataPageViewModel(SaveInfo info)
        {
            Init();

            FoodsCollection = info.FoodsCollection;
            Cows = info.Cows;

            foreach (var c in Cows)
                c.LoadStandartData();

            SelectedCow = Cows.ElementAt(0);
            SelectedFoodGroup = FoodGroupNamesCollection.ElementAt(0);
        }

        private void Init()
        {
            AddNewFoodCommand = new(AddNewFood);
            DeleteFoodCommand = new(DeleteFood);
            EditFoodCommand = new(EditFood);
            ShowResultsCommand = new(ShowResults);

            foreach (var fg in Constants.FoodGroupsData)
            {
                int id = fg.Item1;
                string name = fg.Item2;

                FoodGroupNamesCollection.Add(name);

                FoodsCollection.Add(name, new());
            }
        }

        private void UpdateCowRation()
        {
            OnPropertyChanged(nameof(SelectedCow));

            CowRationCollection.Clear();

            var foodGroups = SelectedCow.FoodGroups.Where(fg => fg.Foods.Count > 0);

            foreach (var fg in foodGroups)
            {
                foreach (var f in fg.Foods)
                {
                    if (fg.SelectedFood == f)
                        f.SetSelected(true);
                    else
                        f.SetSelected(false);
                    CowRationCollection.Add(f);
                }

                if (fg.Foods.All(f => !f.Selected))
                {
                    if (fg.Foods.Count > 0)
                    {
                        fg.Foods.First().Selected = true;
                    }
                }
            }
                

            OnPropertyChanged(nameof(RationsTableVisibility));
        }

        public void UpdateResults(InlineCollection infoTextInlines)
        {
            ResultsCollection.Clear();
            OnPropertyChanged(nameof(ResultsTableVisibility));

            var c = SelectedCow;

            if (c.Quantity == 0)
                return;

            Food stdFood = c.GetStandart();

            stdFood.ActivityCs = c.CalculateStandartActivityCs();
            stdFood.ActivitySr = c.CalculateStandartActivitySr();

            // Подготовка стандартного результата для животного
            Tuple<string, Food> standart = Tuple.Create("Nejvyšší přípustné úrovně", stdFood);
            
            // Рассчет текущих значения для животного
            Tuple<string, Food> actual = Tuple.Create("Aktuální hodnoty", new Food() 
            {
                ActivityCs = c.CalculateActivityCs(),
                ActivitySr = c.CalculateActivitySr(),
                Drymatter = c.CalculateDrymatter(),
                Aminoacides = c.CalculateAminoacides(),
                DProtein = c.CalculateDProtein(),
                EProtein = c.CalculateEProtein(),
                Phosphorus = c.CalculatePhosphorus(),
                Calcium = c.CalculateCalcium(),
                Fibre = c.CalculateFibre(),
                Nel = c.CalculateNel(),
                Nev = c.CalculateNev()
            });

            // Рассчет разницы
            Food delta = actual.Item2 - standart.Item2;
            ResultsCollection.Clear();
            ResultsCollection.Add(standart);
            ResultsCollection.Add(actual);
            ResultsCollection.Add(Tuple.Create("Rozdíl", delta));

            infoTextInlines.Clear();

            foreach (var info in c.BuildInfoText())
            {
                infoTextInlines.Add(new Run(info.Item1) { Foreground = info.Item2 });
                infoTextInlines.Add(new LineBreak());
            }
        }

        private void AddNewFood(object parameter)
        {
            AddNewFoodDialogWindow win = new(SelectedFoodGroup);
            win.ShowDialog();

            if (win.DialogResult == false)
                return;
            var food = win.GetNewFood();
            food.SelectionChanged += FoodSelectionChanged;
            SelectedFoodCollection.Add(food);

            foreach (var c in Cows)
            {
                Food fc = food.Copy();
                var fg = c.FoodGroups.First(fg => fg.Name == food.GroupName);
                if (!fg.Foods.Any(f => f.Amount > 0))
                {
                    fc.Amount = fg.Amount;
                } 
                fg.Foods.Add(fc);
            }

            /*
            var foodGroups = SelectedCow.FoodGroups;

            foreach (var fg in foodGroups)
            {
                if (fg.Foods.Count > 0)
                    fg.Foods.Last().Selected = true;
            }
            */
            UpdateCowRation();
        }

        private void FoodSelectionChanged(bool result, Food item)
        {
            var fg = SelectedCow.FoodGroups.First(i => i.Name == item.GroupName);

            if (result)
            {
                foreach (var f in fg.Foods)
                {
                    f.SetSelected(false);

                }
                item.SetSelected(true);
                fg.SelectedFood = item;
            }
            else
            {
                if (fg.Foods.All(f => !f.Selected))
                {
                    if (fg.Foods.Count > 0)
                        fg.Foods.First().Selected = true;
                }
            }
        }

        private void DeleteFood(object parameter)
        {
            Food f = (Food)parameter;

            SelectedFoodCollection.Remove(f);

            foreach (var c in Cows)
            {
                var r = c.FoodGroups.First(fg => fg.Foods.Any(food => food.Id == f.Id));
                r.Foods.Remove(r.Foods.First(food => food.Id == f.Id));
            }

            UpdateCowRation();
        }

        private void EditFood(object parameter)
        {
            Food f = (Food)parameter;

            new AddNewFoodDialogWindow(f).ShowDialog();

            foreach (var c in Cows)
            {
                var r = c.FoodGroups.First(fg => fg.Foods.Any(food => food.Id == f.Id));
                r.Foods.First(food => food.Id == f.Id).Edit(f);
            }

            UpdateCowRation();
        }

        private void ShowResults(object parameter)
        {
            ResultsPage page = new(Cows);

            MainWindowViewModel.NavigationService.Navigate(page);
        }

        public SaveInfo GetSaveInfo()
        {
            return new()
            {
                Cows = Cows,
                FoodsCollection = FoodsCollection
            };
        }
    }
}
