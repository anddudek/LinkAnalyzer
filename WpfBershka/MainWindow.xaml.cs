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
using System.ComponentModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Concurrent;

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

        private List<Product> _productsFound;
        public IEnumerable<Product> ProductsFound
        {
            get
            {
                _productBuffer.CompleteAdding();
                foreach( var link in _productBuffer.GetConsumingEnumerable())
                {
                    _productsFound.Add(link);
                }
                Console.WriteLine(_productBuffer.IsAddingCompleted.ToString());
                _productBuffer = new BlockingCollection<Product>();
                Console.WriteLine(_productBuffer.IsAddingCompleted.ToString());


                return _productsFound;
            }
        }

        private void AddLinkToCollection(Product link)
        {
            _productBuffer.Add(link);
            RaisePropertyChanged("ProductsFound");
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

        public ICommand AllowSaveToFileCommand
        {
            get
            {
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

        #endregion

        #region Functions

        private void StartStopApp()
        {
            if (_isAppRunning)
            {
                _isAppRunning = false;
                StartStopButtonContent = "Start";

                var a1 = new Product("000001", "15");


            }
            else
            {
                _isAppRunning = true;
                StartStopButtonContent = "Stop";
            }
        }

        private bool CanStartStopApp()
        {
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

        #endregion

        #region Variables

        private bool _isAppRunning;
        private bool _isSaveToFileEnabled;
        private BlockingCollection<Product> _productBuffer;

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

            // Initialize bindings
            CurrentNumberLabel = "Oczekiwanie na start...";
            ProgressValue = 0;
            LinksBeggining = "100000";
            LinksEnding = "999999";
            SaveFilePath = "C:\\BershkaLinks.txt";
            StartStopButtonContent = "Start";

            var a = new Product("0001", "12");
            var a2 = new Product("0002", "13");
            AddLinkToCollection(a);
            AddLinkToCollection(a2);
            //_productsFound.Add(a);
            //_productsFound.Add(a2);

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

        #endregion
    }

}
