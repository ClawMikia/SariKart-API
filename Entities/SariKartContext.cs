using Microsoft.EntityFrameworkCore;

namespace SariKartAPIV2.Entities
{
    public class SariKartContext : DbContext
    {
        public SariKartContext(DbContextOptions<SariKartContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShopOrder> ShopOrders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserStore> UserStores { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<StoreBranch> StoreBranches { get; set; }
        public DbSet<UserType> UserTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<ShopOrder>().ToTable("ShopOrder");
            modelBuilder.Entity<OrderLine>().ToTable("OrderLine");
            modelBuilder.Entity<Delivery>().ToTable("Delivery");
            modelBuilder.Entity<AppUser>().ToTable("AppUser");
            modelBuilder.Entity<UserStore>().ToTable("UserStore");
            modelBuilder.Entity<OrderStatus>().ToTable("OrderStatus");
            modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
            modelBuilder.Entity<StoreBranch>().ToTable("StoreBranch");
            modelBuilder.Entity<UserType>().ToTable("UserType");

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(6,2)");

            modelBuilder.Entity<ShopOrder>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(6,2)");
            modelBuilder.Entity<ShopOrder>()
                .Property(o => o.AmountPaid)
                .HasColumnType("decimal(6,2)");
            modelBuilder.Entity<ShopOrder>()
                .Property(o => o.Change)
                .HasColumnType("decimal(6,2)");

            modelBuilder.Entity<ShopOrder>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShopOrder>()
                .HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey(o => o.OrderStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLines)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Product)
                .WithMany()
                .HasForeignKey(ol => ol.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Order)
                .WithMany()
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserStore>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserStores)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.UserType)
                .WithMany()
                .HasForeignKey(u => u.UserTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
