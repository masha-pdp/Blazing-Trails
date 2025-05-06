using Microsoft.EntityFrameworkCore;

public class BlazingTrailsContext : DbContext
{
    public DbSet<Trail> Trails { get; set; } = default!;
    public DbSet<RouteInstruction> RouteInstructions { get; set; } = default!;
    
    public BlazingTrailsContext(DbContextOptions<BlazingTrailsContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TrailConfig());
        modelBuilder.ApplyConfiguration(new RouteInstructionConfig());
    }
}