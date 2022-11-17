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

using Rations_V2.Models;
using Rations_V2.ViewModels;

namespace Rations_V2.Views
{
    /// <summary>
    /// Логика взаимодействия для MeatGraphWindow.xaml
    /// </summary>
    public partial class MeatGraphWindow : Window
    {
        private readonly MeatGraphViewModel _vm;

        public MeatGraphWindow(Cow cow)
        {
            InitializeComponent();

            _vm = new(GraphControl, cow);

            DataContext = _vm;
        }

        public void GraphDataChanged(object sender, TextChangedEventArgs e)
        {
            _vm.FillGraphCommand.Execute(null);
        }
    }
}
