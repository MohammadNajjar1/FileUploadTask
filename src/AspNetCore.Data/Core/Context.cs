using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AspNetCore.Components.Mvc;
using AspNetCore.Objects;
using AspNetCore.Objects.Models;

namespace AspNetCore.Data;

public class Context : DbContext
{
    public DbSet<UploadedFile> Files { get; set; } = null!;

    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileUploadModel>().Ignore(f => f.File);
        base.OnModelCreating(modelBuilder);
        Type[] models = typeof(AModel)
            .Assembly
            .GetTypes()
            .Where(type =>
                !type.IsAbstract &&
                typeof(AModel).IsAssignableFrom(type))
            .ToArray();

        foreach (Type model in models)
            if (modelBuilder.Model.FindEntityType(model.FullName!) == null)
                modelBuilder.Model.AddEntityType(model);

        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            foreach (PropertyInfo property in entity.ClrType.GetProperties())
                if (typeof(Decimal?).IsAssignableFrom(property.PropertyType))
                    if (property.GetCustomAttribute<NumericAttribute>(false) is NumericAttribute numeric)
                        modelBuilder.Entity(entity.ClrType).Property(property.Name).HasPrecision(numeric.Precision, numeric.Scale);
                    else
                        throw new Exception($"Decimal numbers have to have {nameof(NumericAttribute)} specified. Default [{nameof(NumericAttribute)[..^9]}(15, 2)]");

        foreach (IMutableForeignKey key in modelBuilder.Model.GetEntityTypes().SelectMany(entity => entity.GetForeignKeys()))
            key.DeleteBehavior = DeleteBehavior.Restrict;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseLazyLoadingProxies(); 
        }
    }
}
