using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Skymey_stock_tinkoff_etflist.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.InvestApi.V1;
using Tinkoff.InvestApi;
using Skymey_main_lib.Models.ETF.Tinkoff;
using MongoDB.Bson;
using Google.Protobuf.WellKnownTypes;

namespace Skymey_stock_tinkoff_etflist.Actions.GetEtf
{
    public class GetEtf
    {
        private MongoClient _mongoClient;
        private ApplicationContext _db;
        private string _apiKey;
        public GetEtf()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            _apiKey = config.GetSection("ApiKeys:Tinkoff").Value;
            _mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            _db = ApplicationContext.Create(_mongoClient.GetDatabase("skymey"));
        }
        public void GetEtfFromTinkoff()
        {
            var client = InvestApiClientFactory.Create(_apiKey);
            var response = client.Instruments.Etfs();
            var ticker_finds = (from i in _db.Etfs select i);
            foreach (var item in response.Instruments)
            {
                Console.WriteLine(item.Ticker);
                var ticker_find = (from i in ticker_finds where i.ticker == item.Ticker && i.figi == item.Figi select i).FirstOrDefault();
                if (ticker_find == null)
                {
                    TinkoffETFInstrument tei = new TinkoffETFInstrument();
                    tei._id = ObjectId.GenerateNewId();
                    tei.figi = item.Figi;
                    if (tei.figi == null) tei.figi = "";
                    tei.ticker = item.Ticker;
                    if (tei.ticker == null) tei.ticker = "";
                    tei.classCode = item.ClassCode;
                    if (tei.classCode == null) tei.classCode = "";
                    tei.isin = item.Isin;
                    if (tei.isin == null) tei.isin = "";
                    tei.lot = item.Lot;
                    if (tei.lot == null) tei.lot = 0;
                    tei.currency = item.Currency;
                    if (tei.currency == null) tei.currency = "";
                    tei.shortEnabledFlag = item.ShortEnabledFlag;
                    if (tei.shortEnabledFlag == null) tei.shortEnabledFlag = false;
                    tei.name = item.Name;
                    if (tei.name == null) tei.name = "";
                    tei.exchange = item.Exchange;
                    if (tei.exchange == null) tei.exchange = "";
                    tei.focusType = item.FocusType;
                    if (tei.focusType == null) tei.focusType = "";
                    tei.countryOfRisk = item.CountryOfRisk;
                    if (tei.countryOfRisk == null) tei.countryOfRisk = "";
                    tei.countryOfRiskName = item.CountryOfRiskName;
                    if (tei.countryOfRiskName == null) tei.countryOfRiskName = "";
                    tei.sector = item.Sector;
                    if (tei.sector == null) tei.sector = "";
                    tei.rebalancingFreq = item.RebalancingFreq;
                    if (tei.rebalancingFreq == null) tei.rebalancingFreq = "";
                    tei.tradingStatus = item.TradingStatus.ToString();
                    if (tei.tradingStatus == null) tei.tradingStatus = "";
                    tei.otcFlag = item.OtcFlag;
                    if (tei.otcFlag == null) tei.otcFlag = false;
                    tei.buyAvailableFlag = item.BuyAvailableFlag;
                    if (tei.buyAvailableFlag == null) tei.buyAvailableFlag = true;
                    tei.sellAvailableFlag = item.SellAvailableFlag;
                    if(tei.sellAvailableFlag == null) tei.sellAvailableFlag =false;
                    if (item.MinPriceIncrement != null)
                    {
                        TinkoffETFMinPriceIncrement tempi = new TinkoffETFMinPriceIncrement();
                        tempi.units = item.MinPriceIncrement.Units;
                        tempi.nano = item.MinPriceIncrement.Nano;
                        tei.minPriceIncrement = tempi;
                    }
                    else
                    {
                        tei.minPriceIncrement = new TinkoffETFMinPriceIncrement();
                    }
                    tei.apiTradeAvailableFlag = item.ApiTradeAvailableFlag;
                    if (tei.apiTradeAvailableFlag == null) tei.apiTradeAvailableFlag = false; 
                    tei.uid = item.Uid;
                    if (tei.uid == null) tei.uid = "";
                    tei.realExchange = item.RealExchange.ToString();
                    if (tei.realExchange == null) tei.realExchange = "";
                    tei.positionUid = item.PositionUid;
                    if (tei.positionUid == null) tei.positionUid = "";
                    tei.forIisFlag = item.ForIisFlag;
                    if (tei.forIisFlag == null) tei.forIisFlag=false;
                    tei.forQualInvestorFlag = item.ForQualInvestorFlag;
                    if (tei.forQualInvestorFlag == null) tei.forQualInvestorFlag = false;
                    tei.weekendFlag = item.WeekendFlag;
                    if (tei.weekendFlag == null) tei.weekendFlag = false;
                    tei.blockedTcaFlag = item.BlockedTcaFlag;
                    if (tei.blockedTcaFlag != null) tei.blockedTcaFlag = false;
                    tei.liquidityFlag = item.LiquidityFlag;
                    if (tei.liquidityFlag != null) tei.liquidityFlag = false;
                    tei.first1minCandleDate = item.First1MinCandleDate;
                    if (tei.first1minCandleDate != null) tei.first1minCandleDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    tei.first1dayCandleDate = item.First1DayCandleDate;
                    if (tei.first1dayCandleDate == null) tei.first1dayCandleDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    if (item.FixedCommission != null)
                    {
                        TinkoffETFFixedCommission tefc = new TinkoffETFFixedCommission();
                        tefc.units = item.FixedCommission.Units;
                        tefc.nano = item.FixedCommission.Nano;
                        tei.fixedCommission = tefc;
                    }
                    else
                    {
                        tei.fixedCommission = new TinkoffETFFixedCommission();
                    }
                    tei.releasedDate = item.ReleasedDate;
                    if (tei.releasedDate == null) tei.releasedDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    if (item.NumShares != null)
                    {
                        TinkoffETFNumShares tems = new TinkoffETFNumShares();
                        tems.units = item.NumShares.Units;
                        tems.nano = item.NumShares.Nano;
                        tei.numShares = tems;
                    }
                    else
                    {
                        tei.numShares = new TinkoffETFNumShares();
                    }
                    tei.Update = DateTime.UtcNow;
                    _db.Etfs.Add(tei);
                }
            }
            _db.SaveChanges();
        }
    }
}
