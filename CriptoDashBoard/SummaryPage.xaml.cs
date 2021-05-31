using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects;
using Binance.Net.Objects.Spot;
using System.Diagnostics;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Binance.Net.Objects.Spot.SpotData;
using System.Threading.Tasks;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CriptoDashBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SummaryPage : Page
    {
        readonly string xmlFileName = "SummaryData.xml";
        private StorageFile xmlFile;
        private XmlDocument doc = new XmlDocument();
        private Timer timer;

        private ObservableCollection<CriptoCoin> _criptoCoins = new ObservableCollection<CriptoCoin>();

        public ObservableCollection<CriptoCoin> CriptoCoins
        {
            get { return this._criptoCoins; }
        }

        public SummaryPage()
        {
            this.InitializeComponent();
        }

        private async void SaveSummaryContent()
        {
            doc.SelectSingleNode("criptobot-summary").SelectSingleNode("Wallet").InnerText = WalletTextBox.Text;
            doc.SelectSingleNode("criptobot-summary").SelectSingleNode("Savings").InnerText = SavingsTextBox.Text;

            try
            {
                await doc.SaveToFileAsync(xmlFile);
            }catch (Exception ex) {
                Debug.Print("Error saving " + xmlFileName + "\n");
                Debug.Print(ex.Message);
            }
        }

        private async void LoadSummaryContent()
        {
            try
            {
                xmlFile = await ApplicationData.Current.LocalFolder.GetFileAsync(xmlFileName);
                doc = await XmlDocument.LoadFromFileAsync(xmlFile);

                WalletTextBox.Text = doc.SelectSingleNode("criptobot-summary").SelectSingleNode("Wallet").InnerText;
                SavingsTextBox.Text = doc.SelectSingleNode("criptobot-summary").SelectSingleNode("Savings").InnerText;

            } catch (FileNotFoundException ex) {
                Debug.Print("Error loading " + xmlFileName + "\n");
                Debug.Print(ex.Message);

                CreateXmlFile();
            } catch (Exception ex) {
                Debug.Print("Error loading " + xmlFileName + "\n");
                Debug.Print(ex.Message);
            }
        }

        private async void CreateXmlFile()
        {
            doc.LoadXml("<criptobot-summary><Wallet>0.00000000</Wallet><Savings>0.00000000</Savings></criptobot-summary>");

            try
            {
                xmlFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(xmlFileName);
                await doc.SaveToFileAsync(xmlFile);
            } catch (Exception ex) {
                Debug.Print("Error saving " + xmlFileName + "\n");
                Debug.Print(ex.Message);
            }
        }

        private void WalletTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveSummaryContent();
        }

        private void SavingsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveSummaryContent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSummaryContent();
            BinanceTimer();
        }

        private void BinanceTimer()
        {
            timer = new Timer(LoadBinanceData, "Summary Page - BinanceTimer", TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(15));
        }

        //private void LoadBinanceData()
        public async void LoadBinanceData(object state)
        {
            // Tickers
            _ = BinanceManager.SubscribeTickerUpdates("BTCUSDT", BTCUSDT_Price, "###,##0.00");
            _ = BinanceManager.SubscribeTickerUpdates("BTCBRL", BTCBRL_Price, "###,##0.00");


            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var btcBalance = BinanceManager.GetAssetBalance("BTC");
                decimal assetsBTCValue = 0m;
                decimal btcTotal = 0m;
                CriptoCoin criptoCoin;

                CriptoCoins.Clear();
                foreach (BinanceBalance asset in BinanceManager.GetAccountInfo())
                {
                    if (asset.Free + asset.Locked > 0m)
                    {
                        criptoCoin = new CriptoCoin(asset.Asset, asset.Free, asset.Locked);
                        CriptoCoins.Add(criptoCoin);
                        assetsBTCValue += criptoCoin.DecTotalBTC;
                    }
                }

                BTC_Qty.Text = (btcBalance.Free + btcBalance.Locked).ToString("0.00000000");

                btcTotal += (WalletTextBox.Text != "") ? Decimal.Parse(WalletTextBox.Text) : 0m;
                btcTotal += (SavingsTextBox.Text != "") ? Decimal.Parse(SavingsTextBox.Text) : 0m;
                btcTotal += assetsBTCValue;

                BTCUSDT_Total.Text = (BTCUSDT_Price.Text != "") ? (btcTotal * Decimal.Parse(BTCUSDT_Price.Text)).ToString("###,##0.00") : "0.00";
                BTCBRL_Total.Text = (BTCBRL_Price.Text != "") ? (btcTotal * Decimal.Parse(BTCBRL_Price.Text)).ToString("###,##0.00") : "0.00";
            });

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            BinanceManager.UnSubscribeAll();
        }

        private void WalletTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            float val = float.Parse(WalletTextBox.Text);
            WalletTextBox.Text = val.ToString("###,##0.00000000");
        }

        private void SavingsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            float val = float.Parse(SavingsTextBox.Text);
            SavingsTextBox.Text = val.ToString("###,##0.00000000");
        }

    }
}
