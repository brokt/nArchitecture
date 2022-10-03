using Core.Security.Entities;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Contexts
{
    public class BaseDbContext : DbContext
    {
        protected IConfiguration Configuration { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
       

        public BaseDbContext(DbContextOptions dbContextOptions, IConfiguration configuration) : base(dbContextOptions)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //    base.OnConfiguring(
            //        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SomeConnectionString")));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>(a =>
            {
                a.ToTable("Brands").HasKey(k => k.Id);
                a.Property(p => p.Id).HasColumnName("Id");
                a.Property(p => p.Name).HasColumnName("Name");
                a.HasMany(p => p.Models);
            }); 
            
            modelBuilder.Entity<Model>(a =>
            {
                a.ToTable("Models").HasKey(k => k.Id);
                a.Property(p => p.Id).HasColumnName("Id");
                a.Property(p => p.Name).HasColumnName("Name");
                a.Property(p => p.BrandId).HasColumnName("BrandId");
                a.Property(p => p.DailyPrice).HasColumnName("DailyPrice");
                a.Property(p => p.ImageUrl).HasColumnName("ImageUrl");
                a.HasOne(p => p.Brand);
            });

            #region Core Security

            modelBuilder.Entity<User>(p =>
            {
                p.ToTable("Users").HasKey(k => k.Id);
                p.Property(p => p.Id).HasColumnName("Id");
                p.Property(p => p.FirstName).HasColumnName("FirstName");
                p.Property(p => p.LastName).HasColumnName("LastName");
                p.Property(p => p.Email).HasColumnName("Email");
                p.Property(p => p.PasswordSalt).HasColumnName("PasswordSalt");
                p.Property(p => p.PasswordHash).HasColumnName("PasswordHash");
                p.Property(p => p.Status).HasColumnName("Status");
                p.Property(p => p.AuthenticatorType).HasColumnName("AuthenticatorType");

                p.HasMany(p => p.UserOperationClaims);
                p.HasMany(p => p.RefreshTokens);
            });

            modelBuilder.Entity<OperationClaim>(p =>
            {
                p.ToTable("OperationClaims").HasKey(k => k.Id);
                p.Property(p => p.Id).HasColumnName("Id");
                p.Property(p => p.Name).HasColumnName("Name");
            }); 
            
            modelBuilder.Entity<RefreshToken>(p =>
            {
                p.ToTable("RefreshTokens").HasKey(k => k.Id);
                p.Property(p => p.Id).HasColumnName("Id");
                p.Property(p => p.UserId).HasColumnName("UserId");
                p.Property(p => p.ReplacedByToken).HasColumnName("ReplacedByToken");
                p.Property(p => p.RevokedByIp).HasColumnName("RevokedByIp");
                p.Property(p => p.Created).HasColumnName("Created");
                p.Property(p => p.CreatedByIp).HasColumnName("CreatedByIp");
                p.Property(p => p.Expires).HasColumnName("Expires");
                p.Property(p => p.ReasonRevoked).HasColumnName("ReasonRevoked");
                p.Property(p => p.Revoked).HasColumnName("Revoked");
                p.Property(p => p.Token).HasColumnName("Token");

                p.HasOne(p => p.User);
            });

            modelBuilder.Entity<UserOperationClaim>(p =>
            {
                p.ToTable("UserOperationClaims").HasKey(k => k.Id);
                p.Property(p => p.Id).HasColumnName("Id");
                p.Property(p => p.UserId).HasColumnName("UserId");
                p.Property(p => p.OperationClaimId).HasColumnName("OperationClaimId");

                p.HasOne(p => p.User);
                p.HasOne(p => p.OperationClaim);
            });
            #endregion


            Brand[] brandEntitySeeds = { new(1, "BMW"), new(2, "Mercedes"),new(3,"Honda") };
            modelBuilder.Entity<Brand>().HasData(brandEntitySeeds);
            
            Model[] modelEntitySeeds = { new(1,1,"Series 4",1500,""), new(2, 1, "Series 3", 1200, ""), new(3, 2, "A180", 1000, "") };
            modelBuilder.Entity<Model>().HasData(modelEntitySeeds);

           
        }
    }
}
