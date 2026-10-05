using IndustrialTraceabilitySystem.Api.Domain.Entities;
using IndustrialTraceabilitySystem.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IndustrialTraceabilitySystem.Api.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductionRoute> Routes => Set<ProductionRoute>();
    public DbSet<RouteStep> RouteSteps => Set<RouteStep>();
    public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<ProductionEvent> ProductionEvents => Set<ProductionEvent>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProduct(modelBuilder);
        ConfigureRoute(modelBuilder);
        ConfigureRouteStep(modelBuilder);
        ConfigureProductionLine(modelBuilder);
        ConfigureStation(modelBuilder);
        ConfigureProductionOrder(modelBuilder);
        ConfigureUnit(modelBuilder);
        ConfigureProductionEvent(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureUserRole(modelBuilder);
    }

    private static void ConfigureProduct(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
        });
    }

    private static void ConfigureRoute(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductionRoute>(entity =>
        {
            entity.ToTable("Routes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.Routes)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.ProductId, e.Name }).IsUnique();
        });
    }

    private static void ConfigureRouteStep(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RouteStep>(entity =>
        {
            entity.ToTable("RouteSteps");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StepOrder).IsRequired();
            entity.Property(e => e.IsMandatory).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Route)
                  .WithMany(r => r.Steps)
                  .HasForeignKey(e => e.RouteId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Station)
                  .WithMany(s => s.RouteSteps)
                  .HasForeignKey(e => e.StationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.RouteId, e.StepOrder }).IsUnique();
        });
    }

    private static void ConfigureProductionLine(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductionLine>(entity =>
        {
            entity.ToTable("ProductionLines");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
        });
    }

    private static void ConfigureStation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Station>(entity =>
        {
            entity.ToTable("Stations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.ProductionLine)
                  .WithMany(l => l.Stations)
                  .HasForeignKey(e => e.ProductionLineId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.ProductionLineId, e.Code }).IsUnique();
        });
    }

    private static void ConfigureProductionOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductionOrder>(entity =>
        {
            entity.ToTable("ProductionOrders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PlannedQuantity).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.ProductionOrders)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProductionLine)
                  .WithMany(l => l.ProductionOrders)
                  .HasForeignKey(e => e.ProductionLineId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.HasIndex(e => e.Status);
        });
    }

    private static void ConfigureUnit(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("Units");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SerialNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.ProductionOrder)
                  .WithMany(o => o.Units)
                  .HasForeignKey(e => e.ProductionOrderId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.SerialNumber).IsUnique();
            entity.HasIndex(e => e.Status);
        });
    }

    private static void ConfigureProductionEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductionEvent>(entity =>
        {
            entity.ToTable("ProductionEvents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(e => e.Result).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OccurredAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Unit)
                  .WithMany(u => u.ProductionEvents)
                  .HasForeignKey(e => e.UnitId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Station)
                  .WithMany(s => s.ProductionEvents)
                  .HasForeignKey(e => e.StationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.ProductionEvents)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.UnitId, e.OccurredAt });
            entity.HasIndex(e => new { e.StationId, e.OccurredAt });
            entity.HasIndex(e => e.EventType);
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(e => e.DisplayName).HasMaxLength(150).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
        });
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });
    }

    private static void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.Property(e => e.AssignedAt).IsRequired();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

