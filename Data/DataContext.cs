using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    //In this class we define the abstrations of our databases 
    public class DataContext : DbContext
    {
        //Inside DataContext function we can define the some options related
        //to our database
        public DataContext(DbContextOptions<DataContext> options) 
            : base(options)
        { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
    }
}