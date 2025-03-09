using Microsoft.EntityFrameworkCore;
using GeekStore.API.Models.Domains;

namespace GeekStore.API.Data
{
    public class GeekStoreDbContext : DbContext
    {
        public GeekStoreDbContext(DbContextOptions<GeekStoreDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Tier> Tiers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var tiers = new List<Tier>()
            {
                new Tier
                {
                    Id = Guid.Parse("05547253-358b-4923-b34f-9abf8b96fb61"),
                    Name = "Low end"
                },
                new Tier
                {
                    Id = Guid.Parse("d43469ab-503e-453e-a35a-075752fe84d6"),
                    Name = "Mid end"
                },
                new Tier
                {
                    Id = Guid.Parse("bec04c25-6ba3-46f9-9dd5-273a042cba80"),
                    Name = "High end"
                }
            };

            modelBuilder.Entity<Tier>().HasData(tiers);

            var categories = new List<Category>()
            {
                new Category
                {
                    Id = Guid.Parse("6a3fb4b3-2c2b-4f0e-8cbb-9b4d914729b1"),
                    Name = "CPU"
                },
                new Category
                {
                    Id = Guid.Parse("1992b5e0-7888-476b-a46d-ce812e8d7b6d"),
                    Name = "GPU"
                },
                new Category
                {
                    Id = Guid.Parse("9e336f6c-e645-49a7-bd6f-38f79cdf548a"),
                    Name = "PSU"
                },
                new Category
                {
                    Id = Guid.Parse("8499e196-2cb1-45ad-b7bd-a82a0bb48745"),
                    Name = "Motherboard"
                },
                new Category
                {
                    Id = Guid.Parse("5ec4a3f7-b00a-47a3-aa3d-d946030ca55c"),
                    Name = "Ram"
                },
                new Category
                {
                    Id = Guid.Parse("be730ab1-9f45-41ab-a094-bc1b8a301a03"),
                    Name = "Graphics card"
                },
                new Category
                {
                    Id = Guid.Parse("a24ad4ff-ad4a-4dd7-8ac0-53a6216ab93f"),
                    Name = "Miscellaneous"
                },
            };

            modelBuilder.Entity<Category>().HasData(categories);
        }
    }
}
