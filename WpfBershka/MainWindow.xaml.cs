using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace WpfBershka 
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Bindings

        private string _currentNumberLabel;
        public string CurrentNumberLabel
        {
            get
            {
                return _currentNumberLabel;
            }
            set
            {
                if (value != _currentNumberLabel)
                {
                    _currentNumberLabel = value;
                    RaisePropertyChanged("CurrentNumberLabel");
                }
            }
        }

        private double _progressValue;
        public double ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                if (value != _progressValue)
                {
                    _progressValue = value;
                    RaisePropertyChanged("ProgressValue");
                }
            }
        }

        private string _linksBeggining;
        public string LinksBeggining
        {
            get
            {
                return _linksBeggining;
            }
            set
            {
                if (value != _linksBeggining)
                {
                    _linksBeggining = value;
                    RaisePropertyChanged("LinksBeggining");
                }
            }
        }

        private string _linksEnding;
        public string LinksEnding
        {
            get
            {
                return _linksEnding;
            }
            set
            {
                if (value != _linksEnding)
                {
                    _linksEnding = value;
                    RaisePropertyChanged("LinksEnding");
                }
            }
        }

        private string _saveFilePath;
        public string SaveFilePath
        {
            get
            {
                return _saveFilePath;
            }
            set
            {
                if (value != _saveFilePath)
                {
                    _saveFilePath = value;
                    RaisePropertyChanged("SaveFilePath");
                }
            }
        }

        private string _startStopButtonContent;
        public string StartStopButtonContent
        {
            get
            {
                return _startStopButtonContent;
            }
            set
            {
                if (value != _startStopButtonContent)
                {
                    _startStopButtonContent = value;
                    RaisePropertyChanged("StartStopButtonContent");
                }
            }
        }

        private readonly List<Product> _productsFound;
        public List<Product> ProductsFound
        {
            get
            {
                return _productsFound;
            }
        }

        private readonly ObservableCollection<Product> _productsFoundCollection;
        public ObservableCollection<Product> ProductsFoundCollection
        {
            get
            {
                foreach (var link in ProductsFound)
                {
                    _productsFoundCollection.Add(link);
                }
                ProductsFound.Clear();
                return _productsFoundCollection;
            }
        }

        private void AddLinkToCollection(Product link)
        {
            while (_productBuffer.IsAddingCompleted)
            {
                Thread.Sleep(10);
            }
            _productBuffer.Add(link);
        }

        private void GenerateProductsFound()
        {
            _productBuffer.CompleteAdding();
            foreach (var link in _productBuffer.GetConsumingEnumerable())
            {
                _productsFound.Add(link);
            }
            _productBuffer = new BlockingCollection<Product>();
            RaisePropertyChanged("ProductsFoundCollection");
            // Here save to file
        }

        private int _threadsValue;
        public int ThreadsValue
        {
            get
            {
                return _threadsValue;
            }
            set
            {
                if (_threadsValue != value)
                {
                    _threadsValue = value;
                    RaisePropertyChanged("ThreadsValue");
                }
            }
        }


        #endregion

        #region Commands

        public ICommand StartStopCommand
        {
            get
            {
                return new RelayCommand(StartStopApp, CanStartStopApp);
            }
        }

        public ICommand BrowseForFileCommand
        {
            get
            {
                return new RelayCommand(BrowseForFolder, CanBrowseForFolder);
            }
        }

        public ICommand AllowSaveToFileBATCommand
        {
            get
            {
                _saveToFileFormat = SaveToFileFormat.Bat;
                return new RelayCommand(AllowSaveToFile, CanAllowSaveToFile);
            }
        }

        public ICommand AllowSaveToFileTXTCommand
        {
            get
            {
                _saveToFileFormat = SaveToFileFormat.Txt;
                return new RelayCommand(AllowSaveToFile, CanAllowSaveToFile);
            }
        }

        public ICommand DontAllowSaveToFileCommand
        {
            get
            {
                return new RelayCommand(DontAllowSaveToFile, CanDontAllowSaveToFile);
            }
        }

        public ICommand DeleteSelectedCommand
        {
            get
            {
                return new RelayCommand(DeleteSelected, CanDeleteSelected);
            }
        }

        public ICommand SelectAllCommand
        {
            get
            {
                return new RelayCommand(SelectAll, CanSelectAll);
            }
        }

        public ICommand OpenSelectedCommand
        {
            get
            {
                return new RelayCommand(OpenSelected, CanOpenSelected);
            }
        }

        #endregion

        #region Functions

        private void StartStopApp()
        {
            if (_isAppRunning) //Stop
            {
                _cancellationTokenSource.Cancel();
                CurrentNumberLabel = "Trwa przerywanie...";
                Task.Factory.ContinueWhenAll(_taskArray, (t) =>
                {
                    _isAppRunning = false;
                    StartStopButtonContent = "Start";
                    CurrentNumberLabel = "Przerwano";
                    LinksBeggining = _linkIterator.ToString().PadLeft(6, '0');
                    ProgressValue = 0;
                });
            }
            else //Start
            {
                if (int.Parse(LinksBeggining) >= int.Parse(LinksEnding))
                {
                    MessageBox.Show("Początkowy numer linku musi być większy od końcowego", "Błąd");
                    return;
                }
                _linkIterator = int.Parse(LinksBeggining);
                _cancellationTokenSource = new CancellationTokenSource();
                _taskArray = new Task<int>[ThreadsValue]; // How many tasks

                for (int i = 0; i < _taskArray.Length; i++)
                {
                    var x = i;
                    var _it = _linkIterator++;
                    var task = Task.Run(() =>
                    {
                        var link = GetLinkInfo(_it);
                        AddLinkToCollection(link);
                        return x;
                    }, _cancellationTokenSource.Token);
                    _taskArray[i] = task;
                }

                Task.Factory.ContinueWhenAny(_taskArray, (t) =>
                {
                    ContinueTask(t.Result);
                });

                _isAppRunning = true;
                StartStopButtonContent = "Stop";
                _linkIterator = int.Parse(LinksBeggining);
                CurrentNumberLabel = "Aktualny numer: " + _linkIterator.ToString();
                ProgressValue = 0;
            }
        }

        private bool CanStartStopApp()
        {
            if (_validationErrors.Count > 0)
                return false;
            if (_isAppRunning)
                return true;
            else
            {
                return true;
            }
        }

        private void BrowseForFolder()
        {
            using (var cofd = new CommonOpenFileDialog())
            {
                cofd.InitialDirectory = "C:\\";
                cofd.IsFolderPicker = true;
                CommonFileDialogResult result = cofd.ShowDialog();

                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(cofd.FileName))
                {
                    SaveFilePath = cofd.FileName + "\\BershkaLinks.txt";
                }
            }
        }

        private bool CanBrowseForFolder()
        {
            return true;
        }

        private void AllowSaveToFile()
        {
            _isSaveToFileEnabled = true;
        }

        private bool CanAllowSaveToFile()
        {
            return true;
        }

        private void DontAllowSaveToFile()
        {
            _isSaveToFileEnabled = false;
        }

        private bool CanDontAllowSaveToFile()
        {
            return true;
        }

        private void ContinueTask(int tResult)
        {
            if (_linkIterator >= int.Parse(LinksEnding))
            {
                _cancellationTokenSource.Cancel();
                Task.Factory.ContinueWhenAll(_taskArray, (t) =>
                {
                    _isAppRunning = false;
                    StartStopButtonContent = "Start";
                    CurrentNumberLabel = "Ukończono";
                    LinksBeggining = _linkIterator.ToString().PadLeft(6, '0');
                    ProgressValue = 100;
                });
                return;
            }
            Task<int> task;
            var _it = _linkIterator++;
            task = Task.Run(() =>
            {
                var link = GetLinkInfo(_it);
                AddLinkToCollection(link);

                CurrentNumberLabel = "Aktualny numer: " + (_it).ToString();
                ProgressValue = (((double)(_it) - double.Parse(LinksBeggining)) / (double.Parse(LinksEnding) - double.Parse(LinksBeggining))) * 100;
                return tResult;
            }, _cancellationTokenSource.Token);
            _taskArray[tResult] = task;
            Task.Factory.ContinueWhenAny(_taskArray, (t) =>
            {
                ContinueTask(t.Result);
            }, _cancellationTokenSource.Token);
        }

        private void DeleteSelected()
        {
            for (int i = ProductsFoundCollection.Count - 1; i >= 0; i--)
            {
                if (ProductsFoundCollection[i].TransformerIsSelected)
                {
                    ProductsFoundCollection.RemoveAt(i);
                }
            }            
        }

        private bool CanDeleteSelected()
        {
            return true;
        }

        private void LvTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GenerateProductsFound();
        }

        private void SelectAll()
        {
            foreach (var lvLink in ProductsFoundCollection)
            {
                lvLink.TransformerIsSelected = true;
            }
        }

        private bool CanSelectAll()
        {
            return true;
        }

        private void OpenSelected()
        {
            // TODO
            OpenWebsite("http://google.pl");
            OpenWebsite("http://wp.pl");
        }

        private bool CanOpenSelected()
        {
            return true;
        }

        private static void OpenWebsite(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        private static Product GetLinkInfo(int linkID)
        {
            // TODO: Real process link
            var link = new Product(linkID.ToString(), linkID.ToString());
            Thread.Sleep(2000);
            return link;
        }

        #endregion

        #region Variables

        private bool _isAppRunning;
        private bool _isSaveToFileEnabled;
        private SaveToFileFormat _saveToFileFormat;
        private BlockingCollection<Product> _productBuffer;
        private int _linkIterator;
        private Task<int>[] _taskArray;
        private CancellationTokenSource _cancellationTokenSource;
        private System.Timers.Timer lvTimer;

        #endregion

        #region Constructor

        public MainWindow()
        {
            DataContext = this;

            // Initialize default values
            _isAppRunning = false;
            _isSaveToFileEnabled = true;
            _productBuffer = new BlockingCollection<Product>();
            _productsFound = new List<Product>();
            _productsFoundCollection = new ObservableCollection<Product>();
            _cancellationTokenSource = new CancellationTokenSource();
            _validationErrors = new ObservableCollection<ValidationError>();
            _saveToFileFormat = SaveToFileFormat.Bat;

            // Timer to update ListView
            lvTimer = new System.Timers.Timer();
            lvTimer.Elapsed += LvTimer_Elapsed;
            lvTimer.Interval = 2000;
            lvTimer.Enabled = true;

            // Initialize bindings
            CurrentNumberLabel = "Oczekiwanie na start...";
            ProgressValue = 0;
            LinksBeggining = "100000";
            LinksEnding = "100015";
            SaveFilePath = "C:\\BershkaLinks.txt";
            StartStopButtonContent = "Start";
            ThreadsValue = 5;

            InitializeComponent();
        }

        #endregion

        #region EventHandlers

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public ObservableCollection<ValidationError> _validationErrors { get; private set; }
        private void WindowError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                _validationErrors.Add(e.Error);
            }
            else
            {
                _validationErrors.Remove(e.Error);
            }
            Console.WriteLine(_validationErrors.Count);
        }

        #endregion
    }

}
