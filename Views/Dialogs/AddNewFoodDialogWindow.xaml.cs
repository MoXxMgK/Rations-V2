using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Rations_V2.ViewModels;
using Rations_V2.Models;

namespace Rations_V2.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для AddNewFoodDialogWindow.xaml
    /// </summary>
    public partial class AddNewFoodDialogWindow : Window
    {
        private AddNewFoodDialogWindowViewModel _vm;

        public AddNewFoodDialogWindow(string foodGroupName)
        {
            InitializeComponent();

            _vm = new(foodGroupName);

            _vm.OnDialogResultChanged += DialogResultChanged;

            this.DataContext = _vm;
        }

        public AddNewFoodDialogWindow(Food food)
        {
            InitializeComponent();

            _vm = new(food);
            _vm.OnDialogResultChanged += DialogResultChanged;

            this.DataContext = _vm;
        }

        public Food GetNewFood()
        {
            return this._vm.CurrentFood;
        }

        private void DialogResultChanged(bool result)
        {
            this.DialogResult = result;
        }
    }
}
