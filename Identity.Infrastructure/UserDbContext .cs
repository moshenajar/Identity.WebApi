using Identity.Domain.AggregatesModel.IdentityAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Identity.Infrastructure
{
    public class UserDbContext : IdentityDbContext<AppIdentityUser, AppIdentityRole, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, AppIdentityUserToken>//, IApplicationDbContext
    {
        public UserDbContext()
        {
        }

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }


        public virtual DbSet<AppIdentityUserToken> AppIdentityUserTokens { get; set; }
        public virtual DbSet<AppIdentityRole> AppIdentityRoles { get; set; }

        public virtual DbSet<AppIdentityUserRefreshToken> AppIdentityUserRefreshTokens { get; set; }


        /// <summary>
        /// Change deafult table name
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<AppIdentityUser>().ToTable("AppIdentityUsers");
            modelBuilder.Entity<AppIdentityUserToken>().ToTable("AppIdentityUserTokens");
            modelBuilder.Entity<AppIdentityUserRefreshToken>().ToTable("AppIdentityUserRefreshToken");
            modelBuilder.Entity<AppIdentityUserRefreshToken>().HasKey(ur => ur.UserId);
            modelBuilder.Entity<AppIdentityRole>().ToTable("AppIdentityRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AppIdentityUserClaims");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AppIdentityUserRoles");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AppIdentityUserLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppIdentityRoleClaims");

            modelBuilder.Seed();

        }
    }
}
