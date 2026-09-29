using System.Text.Json;
using CC_Demo.Models;
using CC_Demo.Models.Gcd;
using CC_Demo.Models.Marvel;
using Microsoft.EntityFrameworkCore;

namespace CC_Demo.Repository;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Creator> Creator => Set<Creator>();
    public DbSet<MarvelRecord> MarvelRecords => Set<MarvelRecord>();
    public DbSet<GcdRecord> GcdRecords => Set<GcdRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Creator>(entity =>
        {
            entity.ToTable("Creators");

            entity.Property(c => c.Id)
                .HasConversion(id => id.ToString(), value => Ulid.Parse(value));

            entity.OwnsMany(c => c.Urls, url => url.WithOwner().HasForeignKey("CreatorId"));

            entity.Property(c => c.Thumbnail)
                .HasConversion(
                    thumbnail => JsonSerializer.Serialize(thumbnail, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<ThumbnailMarvel>(json, (JsonSerializerOptions?)null) ?? new ThumbnailMarvel())
                .HasColumnType("jsonb");

            entity.OwnsOne(c => c.Comics, comics =>
            {
                comics.ToTable("CreatorComics");
                comics.OwnsMany(c => c.Items, item =>
                {
                    item.ToTable("CreatorComicsItems");
                    item.WithOwner().HasForeignKey("CreatorId");
                });
            });

            entity.OwnsOne(c => c.Events, events =>
            {
                events.ToTable("CreatorEvents");
                events.OwnsMany(e => e.Items, item =>
                {
                    item.ToTable("CreatorEventsItems");
                    item.WithOwner().HasForeignKey("CreatorId");
                });
            });

            entity.OwnsOne(c => c.Series, series =>
            {
                series.ToTable("CreatorSeries");
                series.OwnsMany(s => s.Items, item =>
                {
                    item.ToTable("CreatorSeriesItems");
                    item.WithOwner().HasForeignKey("CreatorId");
                });
            });

            entity.OwnsOne(c => c.Stories, stories =>
            {
                stories.ToTable("CreatorStories");
                stories.OwnsMany(s => s.Items, item =>
                {
                    item.ToTable("CreatorStoriesItems");
                    item.WithOwner().HasForeignKey("CreatorId");
                });
            });
        });

        modelBuilder.Entity<MarvelRecord>(entity =>
        {
            // cc_demo pre-exists and is managed outside EF; migrations must never create/alter it.
            entity.ToTable("cc_demo", t => t.ExcludeFromMigrations());
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Id).HasColumnName("id")
                .HasConversion(id => id.ToString(), value => Ulid.Parse(value));
            entity.Property(m => m.MarvelId).HasColumnName("marvel_id");
            entity.Property(m => m.AttributionHtml).HasColumnName("attribution_html");
            entity.Property(m => m.AttributionText).HasColumnName("attribution_text");
            entity.Property(m => m.Copyright).HasColumnName("copyright");
            entity.Property(m => m.Data).HasColumnName("data").HasColumnType("jsonb");
            entity.Property(m => m.Resource).HasColumnName("resource");
            entity.Property(m => m.ResourceUri).HasColumnName("resource_uri");
        });

        modelBuilder.Entity<GcdRecord>(entity =>
        {
            // Raw GCD landing table, populated by an external Python scraper. Unlike cc_demo,
            // this table's schema IS owned by EF migrations, so it stays under version control.
            entity.ToTable("Raw_GCD_Data");
            entity.HasKey(g => g.Id);

            entity.Property(g => g.Id).HasColumnName("id")
                .HasConversion(id => id.ToString(), value => Ulid.Parse(value));
            entity.Property(g => g.GcdId).HasColumnName("gcd_id");
            entity.Property(g => g.Resource).HasColumnName("resource");
            entity.Property(g => g.Data).HasColumnName("data").HasColumnType("jsonb");
            entity.Property(g => g.DateTimeIngested).HasColumnName("datetime_ingested");

            entity.HasIndex(g => new { g.Resource, g.GcdId });
        });
    }
}
