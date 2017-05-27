using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBershka
{
    public class Product
    {
        private string _productLink;
        public string ProductLink
        {
            get
            {
                return _productLink;
            }
            set
            {
                _productLink = value;
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
                _productPrizeString = value;
            }
        }

        public Product(string link, string prize)
        {
            ProductLink = link;
            ProductPrizeString = prize;
        }
    }
}
