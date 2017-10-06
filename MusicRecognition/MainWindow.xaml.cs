using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using MusicRecognition.GaussianMixtureModels;
using MusicRecognition.TestingMethods;

namespace MusicRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // knn
        private const int k = 2;
        // gmm
        private const int d = 3;
        private const double treshold = 0.001;
        // Random Decision Forest
	    private const int classesNum = 4;
		private const double rCoefficient = 0.5;
	    private const int treesNum = 50;

        private const int crossValidationSetsCount = 4;
        private const string ROOT_PATH = @"..\..\..\..\..\";
        private const string PATH_COUNTRY = ROOT_PATH + @"data\country\";
        private const string PATH_CLASSICAL = ROOT_PATH + @"data\classical\";
        private const string PATH_HIPHOP = ROOT_PATH + @"data\hiphop\";
        private const string PATH_ROCK = ROOT_PATH + @"data\rock\";
		private const string PATH_TEST = ROOT_PATH + @"data\test\";
        private List<DataElement> _trainingData;
        private AbstractClassification classificator;
        private BackgroundWorker backgroundWorker;
        private LabelLogger logger;

		public MainWindow()
        {
            InitializeComponent();
            logger = new LabelLogger(infoLabel);
            classificator = new kNN(k);
            GetTrainingDataFromDb();
            InitBackgroundWorker();
            AlgorithmCombobox.SelectionChanged += new SelectionChangedEventHandler(OnAlgorithmComboboxChanged);
            try
            {
                PrintInitLogs();
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Nie znaleziono katalogów Rock, HipHop, Country, Muzyka klasyczna w folderze data!");
                logger.WriteLog("BŁĄD");
            }
        }

        /// <summary>
        /// Prints basic info about algorithm and training set
        /// </summary>
        private void PrintInitLogs()
        {
            logger.WriteLog($"Wartość k dla knn: {k}");
            logger.WriteLog($"Wartość progu dla GMM: {treshold}");
            logger.WriteLog($"Liczba podzbiorów kroswalidacji: {crossValidationSetsCount}");
            logger.WriteLog($"Pobrano {_trainingData.Count} elementów z bazy danych");
            logger.WriteLog("Stan plików treningowych na dysku:");
            logger.WriteLog("Rock: " + Directory.GetFiles(PATH_ROCK).Where(el => el.Contains(".wav")).Count());
            logger.WriteLog("HipHop: " + Directory.GetFiles(PATH_HIPHOP).Where(el => el.Contains(".wav")).Count());
            logger.WriteLog("Country: " + Directory.GetFiles(PATH_COUNTRY).Where(el => el.Contains(".wav")).Count());
            logger.WriteLog("Muzyka klasyczna: " + Directory.GetFiles(PATH_CLASSICAL).Where(el => el.Contains(".wav")).Count());
        }

        private void OnAlgorithmComboboxChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedAlgorithm = (e.AddedItems[0] as ComboBoxItem).Content as string;
            switch (selectedAlgorithm)
            {
                case "knn":
                    classificator = new kNN(k);
                    break;
                case "gmm":
                    classificator = new GMM(d,treshold);
                    break;
				case "rdf":
					classificator = new RandomDecisionForest(classesNum, treesNum, rCoefficient);
					break;
			}
        }

        /// <summary>
        /// Searches songs of known genre and adds to database
        /// </summary>
        private void updateDbButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Operacja może zająć dużo czasu. Kontynuować?", "Uwaga!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;

            updateDbButton.IsEnabled = false;
            findSongButton.IsEnabled = false;
            TestsButon.IsEnabled = false;
            progressBar.IsIndeterminate = true;
            backgroundWorker.RunWorkerAsync();
        }

        private void InitBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = false;
            backgroundWorker.DoWork += backgroundWorker_doWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            updateDbButton.IsEnabled = true;
            findSongButton.IsEnabled = true;
            TestsButon.IsEnabled = true;
            progressBar.IsIndeterminate = false;
        }

        private void backgroundWorker_doWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (Database db = new Database())
                {
                    db.UpdateElements(PATH_CLASSICAL, PATH_ROCK, PATH_HIPHOP, PATH_COUNTRY);
                }
            }
            catch (ExecutionEngineException)
            {
                MessageBox.Show("Błąd z wykonaniem skryptu pythona. Upewnij się, że nie używasz piosenki z polskimi znakami w nazwie.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetTrainingDataFromDb()
        {
            using (Database db = new Database())
            {
                _trainingData = db.GetElements();
            }
        }

        /// <summary>
        /// Computes genre of specified song using currently chosen classificator
        /// </summary>
        /// <param name="testData">Song</param>
        /// <returns>Song genre converted to string</returns>
        public string ComputeGenre(DataElement testData)
        {
            SongGenre genre = classificator.GetClass(testData);
            return DataElement.ToString(genre);
        }

        /// <summary>
        /// Tests new song and identifies its genre
        /// </summary>
        private void findSongButton_Click(object sender, RoutedEventArgs e)
        {
            string songPath = DisplayOpenFileDialog();
            if (songPath == null)
                return;
            logger.Clear();
            logger.WriteLog($"WYBRANO: {songPath.Split(new[] { '\\' }).Last() }");
            logger.WriteLog($"Sprawdzanie, czy klasyfikator nauczony");
            if (!classificator.AfterLearning)
            {
                logger.WriteLog($"Uczenie...");
                classificator.Learn(_trainingData);
            }

            logger.WriteLog("Obliczanie gatunku nowej piosenki");
            try
            {
                string type = ComputeGenre(new DataElement(songPath));
                logger.WriteLog($"GATUNEK: {type}");
            }
            catch (ExecutionEngineException)
            {
                MessageBox.Show("Błąd z wykonaniem skryptu pythona. Upewnij się, że piosenka nie ma polskich znaków w nazwie.");
                logger.WriteLog("BŁĄD");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.WriteLog("BŁĄD");
            }

		}

		/// <summary>
		/// Displays open file dialog to user to choose *.wav file
		/// </summary>
		/// <returns>Path to chosen *.wav file</returns>
		private string DisplayOpenFileDialog()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + ROOT_PATH);
            openFileDialog1.Title = "Wybierz piosenkę";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.DefaultExt = "wav";
            openFileDialog1.Filter = "Wave files (*.wav)|*.wav";
            openFileDialog1.ReadOnlyChecked = true;
            openFileDialog1.ShowReadOnly = true;

            if (openFileDialog1.ShowDialog() == false)
            {
                return null;
            }
            return openFileDialog1.FileName;
        }

        private void TestsButon_Click(object sender, RoutedEventArgs e)
        {
            (new ConfusionMatrixWindow(classificator, _trainingData, crossValidationSetsCount)).Show();
        }
    }
}
