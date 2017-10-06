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
using TestingMethods;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;

namespace MusicRecognition.TestingMethods
{
    /// <summary>
    /// Interaction logic for ConfusionMatrixWindow.xaml
    /// </summary>
    public partial class ConfusionMatrixWindow : Window
    {
        private ConfusionMatrixValidation validator;
        private LabelLogger logger;
        private int cvCount;

        public ConfusionMatrixWindow(AbstractClassification classificator, List<DataElement> data, int crossValidationSetsCount)
        {
            InitializeComponent();
            cvCount = crossValidationSetsCount;
            validator = new ConfusionMatrixValidation(classificator, data, cvCount);
            logger = new LabelLogger(infoLabel);
            SetConfusionMatrix();
            genreInfoCombobox.SelectedIndex = 0;            
        }

        private void UpdateROCCurve(SongGenre song)
        {
            plotter.Children.RemoveAll(typeof(LineGraph));
            // draw middle line
            List<Point> points = new List<Point> { new Point(0, 0), new Point(1, 1) };
            DrawLine(points, Colors.Blue, 0.25);

            // draw curve
            points.Add(new Point(validator.FPRate(song), validator.TPRate(song)));
            DrawLine(points, Colors.Red, 2, "Krzywa ROC");

            plotter.LegendVisible = false;
            plotter.IsEnabled = false;
        }

        private void DrawLine(List<Point> points, Color color, double thickness, string description = null)
        {
            points = points.OrderBy(y => y.Y).OrderBy(x => x.X).ToList();
            var pts = new EnumerableDataSource<Point>(points);
            pts.SetXMapping(x => x.X);
            pts.SetYMapping(y => y.Y);

            if (description != null)
                plotter.AddLineGraph(pts, color, thickness, description);
            else
                plotter.AddLineGraph(pts, color, thickness);
        }

        private void SetConfusionMatrix()
        {
            for(int x=0; x<validator.SongsCount; x++)
                for (int y=0; y<validator.SongsCount; y++)
                {
                    var cell = confusionMatrix.Children.Cast<UIElement>()
                        .Where(i => Grid.GetRow(i) == x + 1).Where(i => Grid.GetColumn(i) == y + 1).First();
                    Label label = (Label)cell;
                    label.Content = validator[(SongGenre)x, (SongGenre)y].ToString();
                }
        }

        private void UpdateGenreInfo(SongGenre genre)
        {
            logger.Clear();
            logger.WriteLog("OVERALL SETTINGS");
            logger.WriteLog($"Accuracy rate: {validator.AccuracyRate}");
            logger.WriteLog($"Correct: {validator.CorrectCount}");
            logger.WriteLog($"Errors: {validator.ErrorCount}");
            logger.WriteLog("");
            logger.WriteLog("SETTINGS SPECIFIED FOR CHOSEN GENRE");
            logger.WriteLog($"Precision: {validator.Precision(genre)}");
            logger.WriteLog($"Recall: {validator.Recall(genre)}");
            logger.WriteLog($"True positive: {validator.TP(genre)}");
            logger.WriteLog($"True negative: {validator.TN(genre)}");
            logger.WriteLog($"False positive: {validator.FP(genre)}");
            logger.WriteLog($"False negative: {validator.FN(genre)}");
        }

        private void genreInfoCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SongGenre song = (SongGenre)genreInfoCombobox.SelectedIndex;
            UpdateGenreInfo(song);
            UpdateROCCurve(song);
        }
    }
}
