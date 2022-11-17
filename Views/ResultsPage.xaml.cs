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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Rations_V2.Models;
using Rations_V2.ViewModels;

namespace Rations_V2.Views
{
    /// <summary>
    /// Логика взаимодействия для ResultsPage.xaml
    /// </summary>
    public partial class ResultsPage : Page
    {
        private readonly ResultsPageViewModel _vm;

        public ResultsPage(IEnumerable<Cow> cows)
        {
            InitializeComponent();

            _vm = new(ResultGrid, cows);

            DataContext = _vm;

            this.Loaded += (s, e) => _vm.FillDataGridCommand.Execute(null); 
        }
    }
}
