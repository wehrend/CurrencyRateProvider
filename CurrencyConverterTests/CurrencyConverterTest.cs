using CurrencyRateProvider;
using CurrencyRateProvider.DataModels;
using CurrencyRateProvider.Provider;
using FluentAssertions;
using Shouldly;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CurrencyConverterTests
{
    //Todo: Implement api settings, implement fallback / factory
    //you need to copy the settings file into the debug subfolder
    public class CurrencyConverterTest
    {
        public ITestOutputHelper output { get; set; }

        public CurrencyConverterTest(ITestOutputHelper outputHelper)
        {
            this.output = outputHelper;
        }

        [Fact]
        public void GetSupported_Currencies_CurrencyConverterProvider()
        {
            //Arrange 
            IProvider provider = new CurrencyConverterProvider();

            //Act
            var currrencies = provider.GetSupportedCurrencies();

            //Assert
            Assert.NotNull(currrencies);
        }



        [Fact]
        public void Test_Current_Rate_CurrencyConverterProvider()
        {
            //Arrange
            IProvider provider = new CurrencyConverterProvider();
            var expectedRate = new Rate()
            {
                From = "EUR",
                To = "USD",
                Value = 1.199908m

            };

            //Act
            var rate = provider.GetRate("EUR", "USD").Result;

            //Assert
            Assert.NotEqual(0m,rate.Value);
        }

        [Fact]
        public void Test_History_Conversion_CurrencyConverterProvider()
        {
            //Arrange
            IProvider provider = new CurrencyConverterProvider();
            var expectedRate = new Rate()
            {
                From = "EUR",
                To = "USD",
                Value = 1.199908m

            };
            var amountEur = 1.45m;
            //Act
            var rate = provider.GetRate("EUR", "USD").Result;
            var amountUsd = rate.Convert(amountEur);

            //Assert
            Assert.Equal(amountUsd, amountEur * rate.Value);
        }


        [Fact]
        public void Test_History_Rate_CurrencyConverterProvider()
        {

            //Arrange
            IProvider provider = new CurrencyConverterProvider();
            DateTime historyDate = new DateTime(2017, 12, 31);
            var expectedRate = new Rate()
            {
                From = "EUR",
                To = "USD",
                Date = historyDate,
                Value = 1.199908m

            };

            //Act
            var rate = provider.GetRate("EUR", "USD", historyDate ).Result;

            //Assert
            Assert.Equal(Math.Round(rate.Value, 6), expectedRate.Value);
            //rate.Should().BeEquivalentTo(expectedRate);
        }

        [Fact]
        public void Test_Current_Rate_CurrencylayerProvider()
        {
            //Arrange
            IProvider provider = new CurrencylayerProvider();
            var expectedRate = new Rate()
            {
                From = "EUR",
                To = "USD",
                Value = 1.199908m

            };

            //Act
            var rate = provider.GetRate("EUR", "USD").Result;

            //Assert
            Assert.NotEqual(0m, rate.Value);
        }

        [Fact]
        public void Test_History_Rate_CurrencylayerProvider()
        {

            //Arrange
            IProvider provider = new CurrencylayerProvider();
            DateTime historyDate = new DateTime(2017, 12, 31);
            var expectedRate = new Rate()
            {
                From = "USD",
                To = "EUR",
                Date = historyDate,
                //example was EUR-> USD , so here its the inverse
                Value = Math.Round((1m / 1.199908m)+0.000001m,6) //roun to six digits after 

            };

            //Act
            var rate = provider.GetRate("USD", "EUR", historyDate).Result;

            //Assert
            Assert.Equal(rate.Value, expectedRate.Value);
        }
    }
}
