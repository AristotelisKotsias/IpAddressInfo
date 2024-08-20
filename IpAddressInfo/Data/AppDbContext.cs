#region

using IpAddressInfo.Entities;
using Microsoft.EntityFrameworkCore;

#endregion

namespace IpAddressInfo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<IPAddress> IPAddresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IPAddress>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.IP)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamptz");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasColumnType("timestamptz");

            entity.HasOne(e => e.Country)
                .WithMany()
                .HasForeignKey(e => e.CountryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired();

            entity.Property(e => e.TwoLetterCode)
                .IsRequired()
                .HasMaxLength(2);

            entity.Property(e => e.ThreeLetterCode)
                .IsRequired()
                .HasMaxLength(3);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamptz");
        });


        modelBuilder.Entity<Country>().HasData(
            new Country
            {
                Id = 1, Name = "Greece", TwoLetterCode = "GR", ThreeLetterCode = "GRC",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T06:46:10.500000"), DateTimeKind.Utc)
            },
            new Country
            {
                Id = 2, Name = "Germany", TwoLetterCode = "DE", ThreeLetterCode = "DEU",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T06:46:10.500000"), DateTimeKind.Utc)
            },
            new Country
            {
                Id = 3, Name = "Cyprus", TwoLetterCode = "CY", ThreeLetterCode = "CYP",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T06:46:10.500000"), DateTimeKind.Utc)
            },
            new Country
            {
                Id = 4, Name = "United States", TwoLetterCode = "US", ThreeLetterCode = "USA",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T06:46:10.500000"), DateTimeKind.Utc)
            },
            new Country
            {
                Id = 6, Name = "Spain", TwoLetterCode = "ES", ThreeLetterCode = "ESP",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T06:46:10.500000"), DateTimeKind.Utc)
            },
            new Country
            {
                Id = 7, Name = "France", TwoLetterCode = "FR", ThreeLetterCode = "FRA",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T06:46:10.500000"), DateTimeKind.Utc)
            },
            new Country
            {
                Id = 8, Name = "Italy", TwoLetterCode = "IT", ThreeLetterCode = "ITA",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T06:46:10.500000"), DateTimeKind.Utc)
            },
            new Country
            {
                Id = 9, Name = "Japan", TwoLetterCode = "JP", ThreeLetterCode = "JPN",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T06:46:10.500000"), DateTimeKind.Utc)
            },
            new Country
            {
                Id = 10, Name = "China", TwoLetterCode = "CN", ThreeLetterCode = "CHN",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T06:46:10.500000"), DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<IPAddress>().HasData(
            new IPAddress
            {
                Id = 6, CountryId = 1, IP = "44.255.255.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 7, CountryId = 2, IP = "45.255.255.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 8, CountryId = 3, IP = "46.255.255.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 9, CountryId = 4, IP = "47.255.255.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 10, CountryId = 6, IP = "49.255.255.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 11, CountryId = 7, IP = "41.255.255.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 12, CountryId = 8, IP = "42.255.255.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 13, CountryId = 9, IP = "43.255.255.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 14, CountryId = 10, IP = "50.255.255.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:06.856666"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 15, CountryId = 1, IP = "44.25.55.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 16, CountryId = 2, IP = "45.25.55.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 17, CountryId = 3, IP = "46.25.55.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 18, CountryId = 4, IP = "47.25.55.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 19, CountryId = 6, IP = "49.25.55.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 20, CountryId = 7, IP = "41.25.55.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 21, CountryId = 8, IP = "42.25.55.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 22, CountryId = 9, IP = "43.25.55.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 23, CountryId = 10, IP = "50.25.55.254",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:33.380000"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 24, CountryId = 1, IP = "44.25.55.4",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 25, CountryId = 2, IP = "45.25.55.4",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 26, CountryId = 3, IP = "46.25.55.4",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 27, CountryId = 4, IP = "47.25.55.4",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 28, CountryId = 6, IP = "49.25.55.4",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 29, CountryId = 7, IP = "41.25.55.4",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 30, CountryId = 8, IP = "42.25.55.4",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 31, CountryId = 9, IP = "43.25.55.4",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 32, CountryId = 10, IP = "50.25.55.4",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T07:04:51.323333"), DateTimeKind.Utc)
            },
            new IPAddress
            {
                Id = 33, CountryId = 1, IP = "10.20.30.40",
                CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T08:41:37.310000"), DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2022-10-12T08:41:37.310000"), DateTimeKind.Utc)
            }
        );
    }
}