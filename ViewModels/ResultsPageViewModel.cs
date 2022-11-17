using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Forms;

using Rations_V2.Models;
using Rations_V2.Views;

namespace Rations_V2.ViewModels
{
    public class ResultsPageViewModel : BaseViewModel
    {
        private Visibility _controlVisibility = Visibility.Visible;

        public Visibility ControlVisibility
        {
            get { return _controlVisibility; }
            set { SetField(ref _controlVisibility, value); }
        }

        public bool MeatGraphButtonEnabled => _cows.Any(c => c.Id == Constants.IdFattening);
        public bool MilkGraphButtonEnabled => _cows.Any(c => c.Id == Constants.IdLactatingCows);

        private DataGridView _dg;
        private IEnumerable<Cow> _cows;

        public Command FillDataGridCommand { get; set; }

        public ResultsPageViewModel(DataGridView dg, IEnumerable<Cow> cows)
        {
            FillDataGridCommand = new(FillDataGrid);

            GoBackCommand = new(GoBack);
            OpenGraphCommand = new(OpenGraph);
            PrintCommand = new(Print);

            _dg = dg;
            _cows = cows.Where(c => c.Quantity > 0);

            _dg.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            _dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void FillDataGrid(object parameter)
        {

            _dg.Columns.Add("groupName", "Group");  // 0

            // Fill dataGrid using paseed values and calculate them
            int cowsCount = _cows.Count();

            for (int i = 0; i < cowsCount; i++)
            {

                var cow = _cows.ElementAt(i);
                _dg.Columns.Add(cow.Name, cow.Name);

            }
            _dg.Columns.Add("totalQuantity", "Celková zásoba krmiva, [kg]");  // +1
            _dg.Columns.Add("amountOfForage", "Denní spotřeba krmiva, [kg/den]");  // +2
            _dg.Columns.Add("numberOfFeedingDays", "Počet dnů, na které vystačí zásoba krmiva");  //+ 3

            _dg.Rows.Add(3);
            _dg.Rows[0].Cells[0].Value = "Počet zvířat";
            _dg.Rows[1].Cells[0].Value = "Aktivita Cs-137 přijatá 1 zvířetem s krmivem, [Bq/den]";
            _dg.Rows[2].Cells[0].Value = "Aktivita Sr-90 přijatá 1 zvířetem s krmivem, [Bq/den]";

            int tableOffset = _dg.Rows.Count;

            foreach (var fg in _cows.First().FoodGroups.Where(g => g.Foods.Count > 0))
            {
                foreach (var ft in fg.Foods)
                {
                    _dg.Rows.Add($"{fg.Name} / {ft.Name}");
                }
            }

            // Bad code, replace this later for better one
            for (int i = 1; i <= cowsCount; i++)
            {
                var cow = _cows.ElementAt(i - 1);
                _dg.Rows[0].Cells[i].Value = cow.Quantity;

                var foods = cow.FoodGroups.Where(ft => ft.Foods.Count > 0);

                //double totalCs = cow.CalculateActivityCs();
                //double totalSr = cow.CalculateActivitySr();

                double totalCs = 0;

                foreach (var fg in cow.FoodGroups)
                    foreach (var f in fg.Foods)
                    {
                        totalCs += f.Amount * f.ActivityCs;
                    }

                double totalSr = 0;
                foreach (var fg in cow.FoodGroups)
                    foreach (var f in fg.Foods)
                    {
                        totalSr += f.Amount * f.ActivitySr;
                    }

                _dg.Rows[1].Cells[i].Value = totalCs;
                _dg.Rows[2].Cells[i].Value = totalSr;

                int startRow = 3;

                foreach (var fg in foods)
                {
                    foreach (var ft in fg.Foods)
                    {
                        /* Old code
                        double amount = 0;

                        if (ft == fg.SelectedFood)
                        {
                            amount = fg.Amount;
                        }*/

                        _dg.Rows[startRow].Cells[i].Value = ft.Amount;

                        _dg.Rows[startRow].Cells[cowsCount + 1].Value = ft.Quantity;

                        startRow++;

                    }

                }

            }

            int items = _dg.RowCount;

            for (int i = 3; i < items; i++)
            {
                double totalQuantity = 0;
                for (int j = 1; j <= cowsCount; j++)
                {
                    int animalsCount = (int)_dg.Rows[0].Cells[j].Value;
                    double foodCount = (double)_dg.Rows[i].Cells[j].Value;
                    totalQuantity += animalsCount * foodCount;
                }

                double amountOfForage = (double)_dg.Rows[i].Cells[cowsCount + 1].Value;
                double numberOfFeedingDays = Math.Round(amountOfForage / totalQuantity, 0);

                _dg.Rows[i].Cells[cowsCount + 2].Value = totalQuantity;
                _dg.Rows[i].Cells[cowsCount + 3].Value = numberOfFeedingDays;
            }
        }

        #region ButtonCommands
        public Command GoBackCommand { get; set; }

        private void GoBack(object parameter)
        {
            MainWindowViewModel.NavigationService.GoBack();
        }

        public Command OpenGraphCommand { get; set; }

        private void OpenGraph(object parameter)
        {
            int id = Convert.ToInt32(parameter);

            Cow cow = _cows.First(c => c.Id == id);

            MeatGraphWindow win = new(cow);

            win.Show();
            win.Focus();
        }

        public Command PrintCommand { get; set; }

        private void Print(object parameter)
        {

            System.Windows.Controls.PrintDialog pd = new();
            pd.PageRangeSelection = System.Windows.Controls.PageRangeSelection.AllPages;
            pd.UserPageRangeEnabled = true;

            if (pd.ShowDialog() != true)
                return;

            FlowDocument doc = new();
            doc.PageWidth = pd.PrintableAreaWidth;
            doc.PageHeight = pd.PrintableAreaHeight;
            doc.PagePadding = new Thickness(96 * 0.5, 96 * 0.75, 96 * 0.25, 96 * 0.25);
            doc.FontSize = 12;
            doc.FontFamily = new FontFamily("Times New Roman");
            doc.ColumnGap = 0;
            doc.ColumnWidth = pd.PrintableAreaWidth;

            doc.Blocks.Add(new Paragraph(new Run("Výsledek predikce:")) { FontSize = 24 });
            var settings = Settings.Standart.Default;
            doc.Blocks.Add(new Paragraph(new Run($"Datum: {DateTime.Now.ToString("dd.MM.yyyy")}\n" +
                $"Nejvyšší přípustné úrovně: aktivita Cs-137 v mléce - {settings.StandartMilkActivityCs} [Bq/l], " +
                $"aktivita Sr-90 v mléce - {settings.StandartMilkActivitySr} [Bq/l], " +
                $"aktivita Cs-137 v mase - {settings.StandartMeatActivityCs} [Bq/kg]\n" +
                $"Nejistota - {settings.MeasureOfInaccuracy}%")));

            // Separator from header and table
            doc.Blocks.Add(new Paragraph()
            {
                Background = Brushes.Black,
                FontSize = 2
            });

            // Table printing
            Cow cow = _cows.First();

            Table foodTabel = new Table();
            foodTabel.Columns.Add(new TableColumn());
            foodTabel.Columns.Add(new TableColumn());
            foodTabel.Columns.Add(new TableColumn());
            foodTabel.Columns.Add(new TableColumn());
            foodTabel.Columns.Add(new TableColumn());
            foodTabel.Columns.Add(new TableColumn());
            foodTabel.Columns.Add(new TableColumn());

            TableRow foodHeaderRow = new() { FontSize = 14, FontWeight = FontWeights.Bold };

            // Header row cells
            foodHeaderRow.Cells.Add(new(new Paragraph(new Run("Krmivo"))) { ColumnSpan = 2 });
            foodHeaderRow.Cells.Add(new(new Paragraph(new Run("Aktivita Cs-137"))) { TextAlignment = TextAlignment.Center });
            foodHeaderRow.Cells.Add(new(new Paragraph(new Run("Aktivita Sr-90"))) { TextAlignment = TextAlignment.Center });
            foodHeaderRow.Cells.Add(new(new Paragraph(new Run("Celková zásoba krmiva, [kg]"))) { TextAlignment = TextAlignment.Center });
            foodHeaderRow.Cells.Add(new(new Paragraph(new Run("Celková denní spotřeba krmiva, [kg/d]"))) { TextAlignment = TextAlignment.Center });
            foodHeaderRow.Cells.Add(new(new Paragraph(new Run("Počet dnů, na které vystačí zásoba krmiva"))) { TextAlignment = TextAlignment.Center });

            TableRowGroup foodHeaderRowGroup = new();
            foodHeaderRowGroup.Rows.Add(foodHeaderRow);

            foodTabel.RowGroups.Add(foodHeaderRowGroup);

            // Using loop to fill table with data about cow ration

            foreach (var fg in cow.FoodGroups)
            {
                foreach (var ft in fg.Foods)
                {
                    TableRow row = new();
                    Paragraph p1 = new(new Run($"{fg.Name} / {ft.Name}")); // Name and type of food

                    // Find if food is currenty selected to get proper amount

                    row.Cells.Add(new TableCell(p1) { ColumnSpan = 2 });
                    row.Cells.Add(new TableCell(new Paragraph(new Run($"{ft.ActivityCs}")) { TextAlignment = TextAlignment.Center }));
                    row.Cells.Add(new TableCell(new Paragraph(new Run($"{ft.ActivitySr}")) { TextAlignment = TextAlignment.Center }));
                    row.Cells.Add(new TableCell(new Paragraph(new Run($"{ft.Quantity}")) { TextAlignment = TextAlignment.Center }));

                    double foodPerDay = 0;
                    foreach (var c in _cows)
                    {
                        int quantity = c.Quantity;
                        var foodGroup = c.FoodGroups.First(g => g.Id == fg.Id);
                        /*if (foodGroup.SelectedFood == ft)
                            foodPerDay += foodGroup.Amount * quantity;
                        */

                        foodPerDay = foodGroup.Foods.Sum(f => f.Amount) * quantity;
                    }

                    row.Cells.Add(new TableCell(new Paragraph(new Run($"{foodPerDay}")) { TextAlignment = TextAlignment.Center }));

                    double numberOfFeedingDays = Math.Round(ft.Quantity / foodPerDay, 1);
                    row.Cells.Add(new TableCell(new Paragraph(new Run($"{numberOfFeedingDays}")) { TextAlignment = TextAlignment.Center }));

                    TableRowGroup rowGroup = new();
                    rowGroup.Rows.Add(row);

                    foodTabel.RowGroups.Add(rowGroup);
                }
            }

            doc.Blocks.Add(foodTabel);

            foreach (var c in _cows)
            {
                doc.Blocks.Add(new Paragraph()
                {
                    Background = Brushes.Black,
                    FontSize = 2
                });
                // Name of cow
                doc.Blocks.Add(new Paragraph(new Run(c.Name)) { FontSize = 16, FontWeight = FontWeights.Bold });
                // кол-во of cow
                doc.Blocks.Add(new Paragraph(new Run($"Počet zvířat: {c.Quantity}")){ FontSize = 14 });
                // Cow ration activity info
                doc.Blocks.Add(new Paragraph(new Run($"Aktivita Cs-137 přijatá 1 zvířetem s krmivem: {c.CalculateActivityCs()} Bq/den, Aktivita Sr-90 přijatá 1 zvířetem s krmivem: {c.CalculateActivitySr()} Bq/den"))
                {
                    FontSize = 14
                });

                //Extractions text from info bulding method
                var infoTextData = c.BuildInfoText();

                List<string> infoText = new();

                foreach (var t in infoTextData)
                {
                    infoText.Add(t.Item1);
                }

                doc.Blocks.Add(new Paragraph(new Run(String.Join("\n", infoText))));

                // Preparing table columns
                Table table = new Table();
                table.Columns.Add(new TableColumn());
                table.Columns.Add(new TableColumn());
                table.Columns.Add(new TableColumn());
                table.Columns.Add(new TableColumn());
                //table.Columns.Add(new TableColumn());
                //table.Columns.Add(new TableColumn());
                //table.Columns.Add(new TableColumn());
                // 6 columns in total

                TableRow headerRow = new() { FontSize = 14, FontWeight = FontWeights.Bold };

                // Header row cells
                headerRow.Cells.Add(new(new Paragraph(new Run("Krmivo"))) { ColumnSpan = 2 });
                headerRow.Cells.Add(new(new Paragraph(new Run("Zkrmeno za den 1 zvířetem, [kg]"))) { TextAlignment = TextAlignment.Center });
                headerRow.Cells.Add(new(new Paragraph(new Run("Zkrmeno za den, [kg]"))) { TextAlignment = TextAlignment.Center });
                //headerRow.Cells.Add(new(new Paragraph(new Run("Col4"))) { TextAlignment = TextAlignment.Center });
                //headerRow.Cells.Add(new(new Paragraph(new Run("Col5"))) { TextAlignment = TextAlignment.Center });
                //headerRow.Cells.Add(new(new Paragraph(new Run("col6"))) { TextAlignment = TextAlignment.Center });

                TableRowGroup headerRowGroup = new();
                headerRowGroup.Rows.Add(headerRow);

                table.RowGroups.Add(headerRowGroup);

                // Using loop to fill table with data about cow ration
                foreach (var fg in c.FoodGroups)
                {
                    foreach (var ft in fg.Foods)
                    {
                        TableRow row = new();
                        Paragraph p1 = new(new Run($"{fg.Name} / {ft.Name}")); // Name and type of food

                        // Find if food is currenty selected to get proper amount
                        double amountInRation = ft.Amount;
                        /*
                        if (fg.SelectedFood == ft)
                            amountInRation = fg.Amount;
                        */
                        Paragraph p2 = new(new Run($"{amountInRation}"));

                        Paragraph p3 = new(new Run($"{amountInRation * c.Quantity}"));

                        row.Cells.Add(new TableCell(p1) { ColumnSpan = 2 });
                        row.Cells.Add(new TableCell(p2) { TextAlignment = TextAlignment.Center });
                        row.Cells.Add(new TableCell(p3) { TextAlignment = TextAlignment.Center });
                        //row.Cells.Add(new TableCell(new Paragraph(new Run("null"))) { TextAlignment = TextAlignment.Center });
                        //row.Cells.Add(new TableCell(new Paragraph(new Run("null"))) { TextAlignment = TextAlignment.Center });
                        //row.Cells.Add(new TableCell(new Paragraph(new Run("null"))) { TextAlignment = TextAlignment.Center });

                        TableRowGroup rowGroup = new();
                        rowGroup.Rows.Add(row);

                        table.RowGroups.Add(rowGroup);
                    }
                }

                doc.Blocks.Add(table);
            }

            pd.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Rations Document");
        }

        #endregion
    }
}
