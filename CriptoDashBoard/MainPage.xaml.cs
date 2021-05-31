using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using System.Diagnostics;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CriptoDashBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly string xmlFileName = "KeysData.xml";
        StorageFile xmlFile;
        XmlDocument doc = new XmlDocument();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            string wlcPage;
            await LoadKeysContent();

            if (BinanceManager.Status)
            {
                wlcPage = "summary";
            } else
            {
                wlcPage = "keys";
            }

            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if(item is NavigationViewItem && item.Tag.ToString() == wlcPage)
                {
                    NavView.SelectedItem = item;
                    NavView_Navigate(item as NavigationViewItem);
                    break;
                }
            } 
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // find NavigationViewItem with Content that equals InvokedItem
            var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
            NavView_Navigate(item as NavigationViewItem);
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "summary":
                    ContentFrame.Navigate(typeof(SummaryPage));
                    break;

                case "positions":
                    ContentFrame.Navigate(typeof(PositionsPage));
                    break;

                case "orders":
                    //ContentFrame.Navigate(typeof(GamesPage));
                    break;

                case "log":
                    //ContentFrame.Navigate(typeof(MusicPage));
                    break;

                case "pairs":
                    //ContentFrame.Navigate(typeof(MyContentPage));
                    break;

                case "autostops":
                    //ContentFrame.Navigate(typeof(MyContentPage));
                    break;

                case "keys":
                    ContentFrame.Navigate(typeof(KeysPage));
                    break;
            }
        }
        private async Task LoadKeysContent()
        {
            try
            {
                xmlFile = await ApplicationData.Current.LocalFolder.GetFileAsync(xmlFileName);
                doc = await XmlDocument.LoadFromFileAsync(xmlFile);

                BinanceManager.Initialize(doc.SelectSingleNode("criptobot-keys").SelectSingleNode("ApiKey").InnerText,
                    doc.SelectSingleNode("criptobot-keys").SelectSingleNode("SecretKey").InnerText);

            }
            catch (FileNotFoundException ex)
            {
                Debug.Print("Error loading " + xmlFileName + "\n");
                Debug.Print(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Print("Error loading " + xmlFileName + "\n");
                Debug.Print(ex.Message);
            }
        }

    }
}
