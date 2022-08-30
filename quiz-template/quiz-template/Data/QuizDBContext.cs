using Microsoft.EntityFrameworkCore;

public class QuizDBContext : DbContext
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlite(@"Data Source=QuizDatabase.sqlite;");
    }
}