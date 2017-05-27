using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WpfBershka
{
    public class Product : INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        private string _productLink;
        public string ProductLink
        {
            get
            {
                return _productLink;
            }
            set
            {
                if (_productLink != value)
                {
                    _productLink = value;
                    RaisePropertyChanged("ProductLink");
                }
            }
        }

        private string _productPrizeString;
        public string ProductPrizeString
        {
            get
            {
                return _productPrizeString;
            }
            set
            {
                if (_productPrizeString != value)
                {
                    _productPrizeString = value;
                    RaisePropertyChanged("ProductPrizeString");
                }
            }
        }

        public Product(string link, string prize)
        {
            ProductLink = link;
            ProductPrizeString = prize;
        }

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
