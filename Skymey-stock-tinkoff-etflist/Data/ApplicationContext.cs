using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Skymey_main_lib.Models.Bonds.Tinkoff;
using Skymey_main_lib.Models.Currencies.Tinkoff;
using Skymey_main_lib.Models.ETF.Tinkoff;
using Skymey_main_lib.Models.Tickers.Polygon;
using System.Collections.Generic;
using System.Reflection.Emit;
using Tinkoff.InvestApi.V1;

namespace Skymey_stock_tinkoff_etflist.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<TickerList> TickerList { get; init; }
        public DbSet<TinkoffETFInstrument> Etfs { get; init; }
        public static ApplicationContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<ApplicationContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TickerList>().ToCollection("stock_tickerlist");
            modelBuilder.Entity<TinkoffETFInstrument>().ToCollection("stock_etflist");
        }
    }
}
