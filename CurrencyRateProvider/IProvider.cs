using CurrencyRateProvider.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRateProvider
{
    public interface IProvider
    {
        string Name { get; }

        Task<Rate> GetRate( string source, string dest, DateTime? refDate= null);

        Task<IEnumerable<string>> GetSupportedCurrencies();
        
    }
}
