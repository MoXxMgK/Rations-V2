using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

using ZedGraph;
using Rations_V2.Models;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rations_V2.ViewModels
{
    public class MeatGraphViewModel : GraphViewModel
    {
        private double _liveMeasurement;

        public double LiveMeasurement
        {
            get { return _liveMeasurement; }
            set { SetField(ref _liveMeasurement, value); }
        }

        private double _rationActivityCs;

        public double RationActivityCs
        {
            get { return _rationActivityCs; }
            set { SetField(ref _rationActivityCs, value); }
        }

        private double _transferFactor = 0.04;

        public double TransferFactor
        {
            get { return _transferFactor; }
            set { SetField(ref _transferFactor, value); }
        }

        private double _daysBeforeSlaughter = 30;

        public double DaysBeforeSlaughter
        {
            get { return _daysBeforeSlaughter; }
            set { SetField(ref _daysBeforeSlaughter, value); }
        }

        public Command PrintGraphCommand { get; set; }

        public MeatGraphViewModel(ZedGraphControl graph, Cow cow) : base(graph, cow)
        {
            PrintGraphCommand = new(PrintGraph);

            Fattening c = cow as Fattening;

            RationActivityCs = cow.CalculateActivityCs();
            LiveMeasurement = c.LiveMeasurement;
            DaysBeforeSlaughter = c.DaysBeforeSlaughter;
            graph.IsShowPointValues = true;

            graph.PointValueEvent += Graph_PointValueEvent;
        }

        private string Graph_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            PointPair point = curve[iPt];

            string info = $"Den: {point.X}. Aktivita Cs-137: {Math.Round(point.Y)}";

            return info;
        }

        protected override void FillGraph()
        {
            var settings = Settings.Standart.Default;
            _graph.GraphPane.CurveList.Clear();

            PointPairList points = new PointPairList();
            PointPairList point_RefL = new PointPairList();

            for (int i = 0; i <= DaysBeforeSlaughter; i++)
            {
                int t1 = 3;
                int t2 = 55;
                double a = 0.35;
                double c = (TransferFactor * RationActivityCs) + (LiveMeasurement - TransferFactor * RationActivityCs) *
                    (a * Math.Exp(-0.693 * i / t1) + (1 - a) * Math.Exp(-0.693 * i / t2));

                points.Add(i, c);
                point_RefL.Add(i, settings.StandartMeatActivityCs);
            }

            LineItem curve = _graph.GraphPane.AddCurve("Odhadnutá hmotnostní aktivita Cs-137 v mase", points, System.Drawing.Color.Red);
            curve.Line.Width = 1.5f;
            LineItem curve_RefL = _graph.GraphPane.AddCurve("Nejvyšší přípustné úrovně aktivita Cs-137 v mase", point_RefL, System.Drawing.Color.Green, SymbolType.None);
            curve_RefL.Line.Width = 2.5f;

            var pane = _graph.GraphPane;
            pane.Title.Text = "";
            pane.XAxis.Title.Text = "Den";
            pane.YAxis.Title.Text = "Hmotnostní aktivita Cs-137, [Bq/kg]";


            _graph.AxisChange();
            _graph.Invalidate();
        }

        private void PrintGraph(object parameter)
        {
            System.Windows.Controls.PrintDialog pd = new();
            pd.PageRangeSelection = System.Windows.Controls.PageRangeSelection.AllPages;
            pd.UserPageRangeEnabled = true;

            if (pd.ShowDialog() != true)
                return;

            FlowDocument doc = new();
            doc.PageWidth = pd.PrintableAreaWidth;
            doc.PageHeight = pd.PrintableAreaHeight;
            doc.PagePadding = new System.Windows.Thickness(96 * 0.5, 96 * 0.75, 96 * 0.25, 96 * 0.25);
            doc.FontSize = 12;
            doc.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");
            doc.ColumnGap = 0;
            doc.ColumnWidth = pd.PrintableAreaWidth;
            
            // Header
            doc.Blocks.Add(new Paragraph(new Run("Header")) { FontSize = 18});

            doc.Blocks.Add(new Paragraph(new Run($"Měření zvířat in-vivo, hmotnostní aktivita Cs-137: {LiveMeasurement} [Bq/kg]")));
            doc.Blocks.Add(new Paragraph(new Run($"Aktivita Cs-137 přijatá 1 zvířetem s krmivem: {RationActivityCs} [Bq/den]")));
            doc.Blocks.Add(new Paragraph(new Run($"Přestupový koeficient krmivo – maso: {TransferFactor}")));
            doc.Blocks.Add(new Paragraph(new Run($"Odhad doby do porážky: {DaysBeforeSlaughter}")));

            var graphImage = _graph.GraphPane.GetImage();

            MemoryStream ms = new();
            ((System.Drawing.Bitmap)graphImage).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            Image finalImage = new() { Source = image };

            doc.Blocks.Add(new BlockUIContainer(finalImage));

            pd.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "MeatGraph");
        }
    }
}
