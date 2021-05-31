using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Binance.Net.Objects.Spot.SpotData;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CriptoDashBoard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PositionsPage : Page
    {
        private ObservableCollection<Position> _positions = new ObservableCollection<Position>();

        public ObservableCollection<Position> Positions
        {
            get { return this._positions; }
        }

        public PositionsPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (BinanceBalance asset in BinanceManager.GetAccountInfo())
            {
                if (asset.Free + asset.Locked > 0m)
                {
                    //Positions.Add(new Position(asset.Asset, asset.Free + asset.Locked, 0.004119m, 0.003726m, 0.00432495m, 0.5m));
                }
            }
        }
    }

}
