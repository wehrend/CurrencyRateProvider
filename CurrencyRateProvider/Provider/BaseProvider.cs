using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CurrencyRateProvider.DataModels;
using Microsoft.Extensions.Configuration;

namespace CurrencyRateProvider.Provider
{
    public abstract class BaseProvider : IProvider
    {
        public abstract string Name { get; }

        protected string apiKey;

        protected IConfiguration config;

        public BaseProvider()
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("CurrencyProviderSettings.json", optional: false, reloadOnChange: true)
                .Build();

            this.apiKey = config["ApiKey"];
        }

        public abstract Task<Rate> GetRate(string source, string dest, DateTime? refDate = null);

        public abstract Task<Dictionary<string,string>> GetSupportedCurrencies();

        public void ThrowApiError(string response)
        {
            throw new Exception($"Api has changed: {response}");
        }

        
    }
}
