using AutoPartsStore.Model;
using Microsoft.EntityFrameworkCore;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<ProductTypes> ProductTypes { get; set; }
    public DbSet<Products> Products { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrderItems> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>()
         .HasKey(o => o.CustomerId);

        modelBuilder.Entity<Orders>()
        .HasKey(o => o.OrderId);

        modelBuilder.Entity<OrderItems>()
        .HasKey(o => o.OrderItemId);

        modelBuilder.Entity<Products>()
       .HasKey(o => o.ProductId);

        modelBuilder.Entity<ProductTypes>()
        .HasKey(o => o.ProductTypeId);

        modelBuilder.Entity<OrderItems>()
      .HasOne(o => o.Orders)
      .WithMany(o => o.OrderItems)
      .HasForeignKey(o => o.OrderId);

        modelBuilder.Entity<OrderItems>()
          .HasOne(o => o.Products)
          .WithMany(o => o.OrderItems)
          .HasForeignKey(o => o.ProductId);

        modelBuilder.Entity<Orders>()
        .HasOne(o => o.Customer) 
        .WithMany(c => c.Orders) 
        .HasForeignKey(o => o.CustomerId);




    }
}
