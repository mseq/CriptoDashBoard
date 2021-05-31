using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.Net.Objects.Spot.SpotData;

namespace CriptoDashBoard
{
    public class CriptoCoin
    {
        public string Coin { get; private set; }
        public string QtyFree { get; private set; }
        public string QtyLocked { get; private set; }
        public string PriceBTC { get; private set; }
        public string PriceUSDT { get; private set; }
        public string TotalBTC { get; private set; }
        public string TotalUSDT { get; private set; }


        public decimal DecQtyFree { get; private set; }
        public decimal DecQtyLocked { get; private set; }
        public decimal DecPriceBTC { get; private set; }
        public decimal DecPriceUSDT { get; private set; }
        public decimal DecTotalBTC { get; private set; }
        public decimal DecTotalUSDT { get; private set; }


        public CriptoCoin (string coin, decimal qtyFree, decimal qtyLocked)
        {
            Update(coin, qtyFree, qtyLocked);
        }

        public void Update()
        {
            BinanceBalance balance = BinanceManager.GetAssetBalance(Coin);
            Update(this.Coin, balance.Free, balance.Locked);
        }

        private void Update(string coin, decimal qtyFree, decimal qtyLocked)
        {
            decimal priceBTC;
            decimal priceUSDT;

            Coin = coin;
            DecQtyFree = qtyFree;
            DecQtyLocked = qtyLocked;

            if (coin == "USDC" || coin == "USDT" || coin == "BUSD")
            {
                QtyFree = qtyFree.ToString("###,##0.00");
                QtyLocked = qtyLocked.ToString("###,##0.00");

                priceBTC = BinanceManager.GetPrice("BTC" + coin);
                DecPriceBTC = priceBTC;
                PriceBTC = priceBTC.ToString("###,##0.00");

                DecTotalBTC = ((qtyFree + qtyLocked) / priceBTC);
                TotalBTC = DecTotalBTC.ToString("0.00000000");
            }
            else
            {
                QtyFree = qtyFree.ToString("###,##0.0000");
                QtyLocked = qtyLocked.ToString("###,##0.0000");

                priceBTC = BinanceManager.GetPrice(coin + "BTC");
                DecPriceBTC = priceBTC;
                PriceBTC = priceBTC.ToString("0.00000000");

                DecTotalBTC = ((qtyFree + qtyLocked) * priceBTC);
                TotalBTC = DecTotalBTC.ToString("0.00000000");
            }

            priceUSDT = BinanceManager.GetPrice(coin + "USDT");

            DecPriceUSDT = priceUSDT;
            PriceUSDT = priceUSDT.ToString("###,##0.00");

            DecTotalUSDT = ((qtyFree + qtyLocked) * priceUSDT);
            TotalUSDT = DecTotalUSDT.ToString("###,##0.00");
        }
    }
}
