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

namespace Rations_V2.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для CalculationValuesDialogWindow.xaml
    /// </summary>
    public partial class CalculationValuesDialogWindow : Window
    {
        private CalculationValuesDialogViewModel _vm;
        public CalculationValuesDialogWindow()
        {
            _vm = new();

            DataContext = _vm;

            _vm.OnDialogResultChanged += (r) => this.DialogResult = r; 

            InitializeComponent();
        }
    }
}
