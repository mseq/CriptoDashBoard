using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CriptoDashBoard
{
    public class Position
    {
        public string Pair { get; private set; }
        public decimal Qty { get; private set; }
        public decimal Average { get; private set; }
        public decimal Invested { get; private set; }

        public decimal Stop { get; private set; }
        public decimal StopPL { get; private set; }
        public decimal StopProfit { get; private set; }

        public decimal Price { get; private set; }
        public decimal PricePL { get; private set; }
        public decimal PriceProfit { get; private set; }

        public decimal Target { get; private set; }
        public decimal TargetPL { get; private set; }
        public decimal TargetProfit { get; private set; }

        public decimal PartialPerc { get; private set; }


        public Position(string pair, decimal qty, decimal average, decimal stop, decimal target, decimal partialPerc)
        {
            Pair = pair;
            Qty = qty;
            Average = average;
            Stop = stop;
            Target = target;
            PartialPerc = partialPerc;

            Invested = qty * average;
            StopPL = (qty * stop) - Invested;
            StopProfit = StopPL / Invested * 100;

            Price = BinanceManager.GetPrice(pair);
            PricePL = (qty * Price) - Invested;
            PriceProfit = PricePL / Invested * 100;

            TargetPL = (qty * partialPerc * TargetProfit) - Invested;
            TargetProfit = TargetPL / Invested * 100;
        }
    }
}
