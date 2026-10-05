using IndustrialTraceabilitySystem.Api.Domain.Entities;
using IndustrialTraceabilitySystem.Api.Domain.Enums;
using IndustrialTraceabilitySystem.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndustrialTraceabilitySystem.Api.Infrastructure.Seeding;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context) 
    {
        if (await context.Products.AnyAsync()) 
        {
            return; // [B. G. L.] 05/10/2026 Ya no hay datos, no hacemos nada
        }

        // [B. G. L.] 05/10/2026 1. Roles
        var adminRole = new Role { Name = "Admin", Description = "System administrator" };
        var supervisorRole = new Role { Name = "Supervisor", Description = "Production administrator" };
        var operatorRole = new Role { Name = "Operator", Description = "Production operator" };
        var qualityRole = new Role { Name = "Quality", Description = "Quality inspector" };

        context.Roles.AddRange(adminRole, supervisorRole, operatorRole, qualityRole);
        await context.SaveChangesAsync();

        // [B. G. L.] 05/10/2026 2. Usuario admin
        var hasher = new PasswordHasher<User>();
        var adminUser = new User 
        {
            Username = "admin",
            DisplayName = "System Administrator",
            IsActive = true
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();

        context.UserRoles.Add(new UserRole 
        {
            UserId = adminUser.Id,
            RoleId = adminRole.Id
        });
        await context.SaveChangesAsync();

        // [B. G. L.] 05/10/2026 3. Producto
        var product = new Product
        {
            Code = "CTRL-BOARD-A",
            Name = "Control Board A",
            Description = "Fictitious control board used for demos."
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // [B. G. L.] 05/10/2026 4. Linea de produccion
        var line = new ProductionLine
        {
            Code = "LINE-SMT-01",
            Name = "SMT Line 01"
        };
        context.ProductionLines.Add(line);
        await context.SaveChangesAsync();

        // [B. G. L.] 05/10/2026 5. Estaciones
        var printer = new Station { ProductionLineId = line.Id, Code = "PRINTER", Name = "Printer" };
        var spi = new Station { ProductionLineId = line.Id, Code = "SPI", Name = "Solder Paste Inspection" };
        var aoi = new Station { ProductionLineId = line.Id, Code = "AOI", Name = "Automated Optical Inspection" };
        var rework = new Station { ProductionLineId = line.Id, Code = "REWORK", Name = "Rework" };
        var finalInspection = new Station { ProductionLineId = line.Id, Code = "FINAL-INSP", Name = "Final Inspection" };

        context.Stations.AddRange(printer, spi, aoi, rework, finalInspection);
        await context.SaveChangesAsync();

        // [B. G. L.] 05/10/2026 6. Ruta del producto
        var route = new ProductionRoute
        {
            ProductId = product.Id,
            Name = "Standard SMT Route"
        };
        context.Routes.Add(route);
        await context.SaveChangesAsync();

        context.RouteSteps.AddRange(
            new RouteStep { RouteId = route.Id, StationId = printer.Id, StepOrder = 1 },
            new RouteStep { RouteId = route.Id, StationId = spi.Id, StepOrder = 2 },
            new RouteStep { RouteId = route.Id, StationId = aoi.Id, StepOrder = 3 },
            new RouteStep { RouteId = route.Id, StationId = rework.Id, StepOrder = 4, IsMandatory = false },
            new RouteStep { RouteId = route.Id, StationId = finalInspection.Id, StepOrder = 5}
        );
        await context.SaveChangesAsync();

        // [B. G. L.] 05/10/2026 7. Orden de produccion
        var order = new ProductionOrder
        {
            OrderNumber = "OP-2026-0001",
            ProductId = product.Id,
            ProductionLineId = line.Id,
            PlannedQuantity = 100,
            Status = ProductionOrderStatus.InProgress
        };
        context.ProductionOrders.Add( order );
        await context.SaveChangesAsync();

        // [B. G. L.] 05/10/2026 8. Unidades
        var unit1 = new Unit { SerialNumber = "PCB000001", ProductionOrderId = order.Id, Status = UnitStatus.Passed };
        var unit2 = new Unit { SerialNumber = "PCB000002", ProductionOrderId = order.Id, Status = UnitStatus.InProgress };
        var unit3 = new Unit { SerialNumber = "PCB000003", ProductionOrderId = order.Id, Status = UnitStatus.InProgress };

        context.Units.AddRange(unit1, unit2, unit3);
        await context.SaveChangesAsync();

        // [B. G. L.] 05/10/2026 9. Eventos de ejemplo
        var now = DateTime.UtcNow;
        var events = new List<ProductionEvent>
        {
            // [B. G. L.] 05/10/2026 Unidad 1 - paso completo con un retrabajo
            new() { UnitId = unit1.Id, StationId = printer.Id, UserId = adminUser.Id, EventType = ProductionEventType.Pass, Result = ProductionEventResult.Pass, OccurredAt = now.AddMinutes(-50) },
            new() { UnitId = unit1.Id, StationId = spi.Id, UserId = adminUser.Id, EventType = ProductionEventType.Pass, Result = ProductionEventResult.Pass, OccurredAt = now.AddMinutes(-40) },
            new() { UnitId = unit1.Id, StationId = aoi.Id, UserId = adminUser.Id, EventType = ProductionEventType.Fail, Result = ProductionEventResult.Fail, Notes = "Solder bridge detected.", OccurredAt = now.AddMinutes(-30) },
            new() { UnitId = unit1.Id, StationId = rework.Id, UserId = adminUser.Id, EventType = ProductionEventType.Rework, Result = ProductionEventResult.Complete, OccurredAt = now.AddMinutes(-20) },
            new() { UnitId = unit1.Id, StationId = aoi.Id, UserId = adminUser.Id, EventType = ProductionEventType.Pass, Result = ProductionEventResult.Pass, OccurredAt = now.AddMinutes(-10) },
            new() { UnitId = unit1.Id, StationId = finalInspection.Id, UserId = adminUser.Id, EventType = ProductionEventType.Pass, Result = ProductionEventResult.Pass, OccurredAt = now },

            // [B. G. L.] 05/10/2026 Unidad 2 - en proceso
            new() { UnitId = unit2.Id, StationId = printer.Id, UserId = adminUser.Id, EventType = ProductionEventType.Pass, Result = ProductionEventResult.Pass, OccurredAt = now.AddMinutes(-15) },
            new() { UnitId = unit2.Id, StationId = spi.Id, UserId = adminUser.Id, EventType = ProductionEventType.Pass, Result = ProductionEventResult.Pass, OccurredAt = now.AddMinutes(-5) },

            // [B. G. L.] 05/10/2026 Unidad 3 - en espera
            new() { UnitId = unit3.Id, StationId = printer.Id, UserId = adminUser.Id, EventType = ProductionEventType.Hold, Result = ProductionEventResult.Released, Notes = "Waiting for material.", OccurredAt = now.AddMinutes(-2) }
        };

        context.ProductionEvents.AddRange(events);
        await context.SaveChangesAsync();
    }
}