using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Models; // Assuming Product is in this namespace

namespace ECommerceAPI.Data
{
public class ECommerceDbContext : DbContext
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) :
    base(options) { }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
}
}