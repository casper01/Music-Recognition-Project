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
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;

namespace ROCCurve
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ROCGraph : Window
    {
        public ROCGraph()
        {
            InitializeComponent();
            List<Point> points = new List<Point> { new Point(0, 0), new Point(1, 1) };
            DrawLine(points, Colors.Blue, 0.25);

            points.Add(new Point(0.5, 0.2));
            points = points.OrderBy(y=>y.Y).OrderBy(x => x.X).ToList();
            DrawLine(points, Colors.Red, 2, "Krzywa ROC");
            plotter.LegendVisible = false;
            plotter.IsEnabled = false;
        }
        private void DrawLine(List<Point> points, Color color, double thickness, string description = null)
        {
            var pts = new EnumerableDataSource<Point>(points);
            pts.SetXMapping(x => x.X);
            pts.SetYMapping(y => y.Y);
            
            if (description != null)
                plotter.AddLineGraph(pts, color, thickness, description);
            else
                plotter.AddLineGraph(pts, color, thickness);
        }
    }
}
