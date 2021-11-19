using Identity.Domain.AggregatesModel.IdentityAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Identity.Infrastructure
{
    public static class DatabaseSeed
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            ApplicationRole[] roles = (ApplicationRole[])Enum.GetValues(typeof(ApplicationRole));

            foreach (string role in roles.Select(r => r.GetRoleName()))
            {
                Guid g = Guid.NewGuid();
                modelBuilder.Entity<AppIdentityRole>().HasData(
                new AppIdentityRole
                {
                    Id = g,
                    Name = role,
                    NormalizedName = role.ToUpper(),
                    ConcurrencyStamp = g.ToString()
                }
            );
            }

        }
    }
}

