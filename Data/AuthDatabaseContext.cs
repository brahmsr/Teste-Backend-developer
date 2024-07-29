using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APItesteInside.Data
{
    public class AuthDatabaseContext: IdentityDbContext
    {
        public AuthDatabaseContext(DbContextOptions<AuthDatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerRoleId = "34566c9e-d857-47d3-9f73-59ff4ad10ca6";
            var writerRoleId = "780d2deb-5bea-440f-95fa-c3737f36a182";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "1",
                    ConcurrencyStamp = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2",
                    ConcurrencyStamp = writerRoleId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
