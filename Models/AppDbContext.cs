using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Absen> Absens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Absen>()
            .HasOne(a => a.Employee)
            .WithMany(e => e.Absens)
            .HasForeignKey(a => a.Employee_Id);
    }
}
