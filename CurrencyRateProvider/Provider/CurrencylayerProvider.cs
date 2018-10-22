using CurrencyRateProvider.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRateProvider.Provider
{
    public class CurrencylayerProvider :  BaseProvider
    {
        const string baseUrlWithMode = "http://apilayer.net/api/{0}";
        string historyMode = "historical";
        string liveMode = "live";
        string accessTemplate = "?access_key={0}";
        const string dateString = "&date={0}-{1}-{2}";


        public CurrencylayerProvider() : base()
        {
            var providerConfig = config.GetSection("Currencylayer");
            this.apiKey = providerConfig["ApiKey"];
        }

        public override string Name { get { return "Currencylayer"; } }

        public async override Task<Rate> GetRate(string sourceCur, string destCur, DateTime? date)
        {
            Rate rate = new Rate();
            rate.From = sourceCur;
            rate.To = destCur;

            //check first if source and target are the same to, save unnecessary queries
            if (!rate.To.Equals(rate.From))
            {
                rate.Value = 1.0m;
                return rate;
            }

            string queryUrl = "";


            if (date.HasValue)
            {
                var dateQuery = string.Format(dateString, date.Value.Year, date.Value.Month, date.Value.Day);
                var access = string.Format(accessTemplate, apiKey);
                var historyUrl = string.Format(baseUrlWithMode, historyMode);
                var fullUrl = historyUrl+ access +"&source={0}&currencies={1}"+dateQuery; 
                queryUrl = string.Format(fullUrl, sourceCur, destCur);
                rate.Date = date.Value;
            }
            else
            {
                var access = string.Format(accessTemplate, apiKey);
                var historyUrl = string.Format(baseUrlWithMode, liveMode);
                var fullUrl = historyUrl + access + "&source={0}&currencies={1}";
                queryUrl = string.Format(fullUrl, sourceCur, destCur, "");
                rate.Date = DateTime.Now;
            }

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(queryUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    if (jsonResult.Contains("error"))
                        ThrowApiError(jsonResult);
                    dynamic result = JsonConvert.DeserializeObject(jsonResult);
                    var key = sourceCur + destCur;
                    var nestedResult =result["quotes"];
                    rate.Value = nestedResult[key];
                    return rate;
                }
                else
                {
                    return null;
                }
            }

        }

        public override Task<Dictionary<string,string>> GetSupportedCurrencies()
        {
            throw new NotImplementedException();
        }
    }
}
