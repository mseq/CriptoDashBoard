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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CriptoDashBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class KeysPage : Page
    {
        readonly string xmlFileName = "KeysData.xml";
        StorageFile xmlFile;
        XmlDocument doc = new XmlDocument();

        public KeysPage()
        {
            this.InitializeComponent();
        }

        private async void SaveKeysContent()
        {
            doc.SelectSingleNode("criptobot-keys").SelectSingleNode("ApiKey").InnerText = ApiKeyTextBox.Password;
            doc.SelectSingleNode("criptobot-keys").SelectSingleNode("SecretKey").InnerText = ApiSecretTextBox.Password;

            try
            {
                await doc.SaveToFileAsync(xmlFile);
            }
            catch (Exception ex)
            {
                Debug.Print("Error saving " + xmlFileName + "\n");
                Debug.Print(ex.Message);
            }
        }

        private async void LoadKeysContent()
        {
            try
            {
                xmlFile = await ApplicationData.Current.LocalFolder.GetFileAsync(xmlFileName);
                doc = await XmlDocument.LoadFromFileAsync(xmlFile);

                ApiKeyTextBox.Password = doc.SelectSingleNode("criptobot-keys").SelectSingleNode("ApiKey").InnerText;
                ApiSecretTextBox.Password = doc.SelectSingleNode("criptobot-keys").SelectSingleNode("SecretKey").InnerText;

                BinanceManager.apiKey = doc.SelectSingleNode("criptobot-keys").SelectSingleNode("ApiKey").InnerText;
                BinanceManager.secretKey = doc.SelectSingleNode("criptobot-keys").SelectSingleNode("SecretKey").InnerText;

            }
            catch (FileNotFoundException ex)
            {
                Debug.Print("Error loading " + xmlFileName + "\n");
                Debug.Print(ex.Message);

                CreateXmlFile();
            }
            catch (Exception ex)
            {
                Debug.Print("Error loading " + xmlFileName + "\n");
                Debug.Print(ex.Message);
            }
        }

        private async void CreateXmlFile()
        {
            doc.LoadXml("<criptobot-keys><ApiKey></ApiKey><SecretKey></SecretKey></criptobot-keys>");

            try
            {
                xmlFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(xmlFileName);
                await doc.SaveToFileAsync(xmlFile);
            }
            catch (Exception ex)
            {
                Debug.Print("Error saving " + xmlFileName + "\n");
                Debug.Print(ex.Message);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadKeysContent();
        }

        private void ApiKeyTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            SaveKeysContent();
            BinanceManager.apiKey = ApiKeyTextBox.Password;
        }

        private void ApiSecretTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            SaveKeysContent();
            BinanceManager.secretKey = ApiSecretTextBox.Password;
        }
    }
}
