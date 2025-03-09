using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeekStore.API.Data
{
    public class GeekStoreAuthDbContext : IdentityDbContext
    {
        public GeekStoreAuthDbContext(DbContextOptions<GeekStoreAuthDbContext> options) : base(options)
        {
        }

        // Data Seeding
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerRoleId = "cf330825-ba85-43bc-9d1b-3d52286bc775";
            var writerRoleId = "d3f66143-a96c-4f19-950d-a3b99f20a35b";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },
                new IdentityRole
                {
                    Id = writerRoleId,
                    ConcurrencyStamp = writerRoleId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

        }
    }
}
