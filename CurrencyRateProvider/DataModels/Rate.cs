using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyRateProvider.DataModels
{
    public class Rate
    {
        public string From { get; set; }

        public string To { get; set; }

        public DateTime Date { get; set; }

        public Decimal Value { get; set; }


        public decimal Convert(decimal amount)
        {
            return (amount * this.Value);
        }


    }

}