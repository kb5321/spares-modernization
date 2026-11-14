using Microsoft.EntityFrameworkCore;
using SpaRes.Api.Models;

namespace SpaRes.Api.Data
{
    public class SpaResDbContext : DbContext
    {
        public SpaResDbContext(
            DbContextOptions<SpaResDbContext> options)
            : base(options)
        {
        }
        /*
                 SpaResDbContext : DbContext
                │
                └── inherits EF Core database machinery

                DbContextOptions<SpaResDbContext> options
                │
                └── receives SpaRes database configuration

                : base(options)
                │
                └── gives that configuration to EF Core
         */

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ResLog> ResLogs { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<LocationService> LocationServices { get; set; }
        public DbSet<StaffService> StaffServices { get; set; }
        public DbSet<StaffSchedule> StaffSchedule { get; set; }
        public DbSet<StaffBreak> StaffBreak { get; set; }

        public DbSet<StaffAbsence> StaffAbsence { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }



    }
}