﻿using CurrencyRateProvider.DataModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRateProvider.Provider
{
    public class CurrencyConverterProvider : BaseProvider
    {
        //examples
        //https://free.currencyconverterapi.com/api/v6/convert?q=USD_PHP,PHP_USD&compact=ultra&date=2017-12-31


        const string urlFree = "https://free.currencyconverterapi.com/api/v6/convert?q={0}_{1}&compact=ultra{2}";
        //const string urlApiKey = "https://api.currencyconverterapi.com/api/v6/convert?q={0}_{1}&compact=ultra{2}&apiKey=[{3}]";
        const string dateString = "&date={0}-{1}-{2}";

        public CurrencyConverterProvider() : base()
        {
            var providerConfig = config.GetSection("CurrencyConverter");
            this.apiKey = providerConfig["ApiKey"];
        }

        public override string Name { get { return "CurrencyConverter"; } }

        public async override Task<Rate> GetRate(string sourceCur, string destCur, DateTime? date)
        {
            Rate rate = new Rate();
            rate.From = sourceCur;
            rate.To = destCur;

            string queryUrl = "";
                
            if (date.HasValue)
            {
                var dateQuery = string.Format(dateString, date.Value.Year, date.Value.Month, date.Value.Day);
                queryUrl = string.Format(urlFree, sourceCur, destCur, dateQuery);
                rate.Date = date.Value;
            }
            else {
                queryUrl = string.Format(urlFree, sourceCur, destCur, "");
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

                    if (date.HasValue)
                    {
                        var curkey = sourceCur + "_" + destCur;
                        var nestedResult = result[curkey];
                        var datekey = $"{date.Value.Year}-{date.Value.Month}-{date.Value.Day}";
                        rate.Value = nestedResult[datekey];
                        return rate;
                    }
                    else
                    {
                        var key = sourceCur + "_" + destCur;
                        rate.Value = result[key];
                        return rate;
                    }
                }
                else
                {
                    return null;
                }
            }

        }

    }
}
