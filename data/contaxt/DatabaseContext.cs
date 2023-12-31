namespace Paradigm.Data
{
    using System;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    using Paradigm.Common;
    using Paradigm.Data.Model;

    public sealed class DatabaseContext : DbContextBase, IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext() : base()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            optionsBuilder.UseNpgsql(this.DesignTimeConfig?.ConnectionString, o =>
            {
                o.MigrationsAssembly(typeof(DatabaseContext).GetAssemblyName());
            });

            return new DatabaseContext(optionsBuilder.Options);
        }

        protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.BeforeModelCreated(modelBuilder);
            CreateModel(modelBuilder, this.DesignTimeConfig?.SchemaName);
            base.AfterModelCreated(modelBuilder);
        }

        private static void CreateModel(ModelBuilder modelBuilder, string schemaName)
        {
            modelBuilder.Entity<Provider>(entity =>
            {
                entity.HasKey(e => e.ProviderId)
                    .HasName("PK_Provider");

                entity.Property(e => e.ProviderId)
                    .HasMaxLength(64);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.Enabled);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK_RoleId");

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.ParentRoleId)
                    .HasMaxLength(32);

                entity.Property(e => e.Enabled);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64);
            });

            modelBuilder.Entity<SecurityClaim>(entity =>
            {
                entity.HasKey(e => e.SecurityClaimId)
                    .HasName("PK_SecurityClaim");

                entity.Property(e => e.SecurityClaimId)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.Enabled)
                    .IsRequired();

                entity.Property(e => e.Origin);

                entity.Property(e => e.ValidationPattern);
            });

            modelBuilder.Entity<RoleSecurityClaim>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.SecurityClaimId })
                    .HasName("PK_RoleSecurityClaim");

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.SecurityClaimId)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Value);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_User_UserId");

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CultureName)
                    .IsRequired()
                    .HasMaxLength(5);                

                entity.Property(e => e.Enabled)
                    .IsRequired();                

                entity.Property(e => e.PasswordSalt)
                    .HasMaxLength(128);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(256);

                entity.Property(e => e.ProviderId)
                    .HasMaxLength(64);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.UserId })
                    .HasName("PK_UserRole");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_UserRole_UserId");

                entity.Property(e => e.RoleId)
                    .HasMaxLength(32);
            });
        }
    }
}