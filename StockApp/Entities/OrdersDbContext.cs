using Microsoft.EntityFrameworkCore;

namespace ServiceContracts.DTO;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions Options) : base(Options)
    {

    }

    public DbSet<BuyOrder> BuyOrders { get; set; }
    public DbSet<SellOrder> SellOrders { get; set; }

}