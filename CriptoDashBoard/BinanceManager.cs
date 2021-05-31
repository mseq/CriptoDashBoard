using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects;
using Binance.Net.Objects.Spot;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Objects;
using Binance.Net.Objects.Spot.SpotData;

namespace CriptoDashBoard
{
    public static class BinanceManager
    {
        public static bool Status { get; set; }
        private static BinanceSocketClient sckcli;
        private static BinanceClient cli;

        public static string apiKey { get; set; }
        public static string secretKey { get; set; }

        static BinanceManager()
        {
            Status = false;
        }

        public static void Initialize(string myApiKey, string mySecretKey)
        {
            apiKey = myApiKey;
            secretKey = mySecretKey;

            sckcli = new BinanceSocketClient(new BinanceSocketClientOptions
            {
                ApiCredentials = new ApiCredentials(apiKey, secretKey)
            });
            cli = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials(apiKey, secretKey)
            });

            Status = true;
        }

        public static CallResult SubscribeTickerUpdates(string symbol, TextBlock textBlock, string format)
        {
            return sckcli.Spot.SubscribeToSymbolTickerUpdates(symbol, data =>
            {
                string price = data.LastPrice.ToString(format);
                _ = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    textBlock.Text = price;
                });
            });
        }

        public static BinanceBalance GetAssetBalance(string symbol)
        {
            foreach (var balance in cli.General.GetAccountInfo().Data.Balances.ToList())
            {
                if (balance.Asset == symbol)
                {
                    return balance;
                }
            }

            return null;
        }

        public static List<BinanceBalance> GetAccountInfo()
        {
            return cli.General.GetAccountInfo().Data.Balances.ToList();
        }

        public static decimal GetPrice(string symbol)
        {
            if (symbol == "BTCBTC" || symbol == "USDTUSDT")
            {
                return 1m;
            }

            try
            {
                return cli.Spot.Market.GetCurrentAvgPrice(symbol).Data.Price;
            } catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                return 0m;
            }
        }

        public static void UnSubscribeAll()
        {
            sckcli.UnsubscribeAll();
        }

    }
}
