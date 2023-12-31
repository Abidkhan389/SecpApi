namespace Paradigm.Data
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    using Microsoft.Extensions.Configuration;

    using Paradigm.Data.Model;

    public abstract class DbContextBase : DbContext
    {
        #region public
        public virtual DbSet<Screen> Screen { get; set; }
        public virtual DbSet<RoleScreen> RoleScreen { get; set; }
        public virtual DbSet<AppException> AppException { get; set; }
        #endregion
        // Role Schema
        public virtual DbSet<Provider> Provider { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleSecurityClaim> RoleSecurityClaim { get; set; }
        public virtual DbSet<SecurityClaim> SecurityClaim { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserDetail> UserDetail { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        #region Administration
        public virtual DbSet<Audit> Audit { get; set; }
        public virtual DbSet<AuditHistory> AuditHistory { get; set; }
        public virtual DbSet<StaticData> StaticData { get; set; }
        public virtual DbSet<StaticDataParent> StaticDataParent { get; set; }
        #endregion
        #region Views
        public virtual DbSet<VW_Role> VW_Role { get; set; }
        public virtual DbSet<VW_User> VW_User { get; set; }
        public virtual DbSet<VW_StaticData> VW_StaticData { get; set; }
        public virtual DbSet<VW_StaticDataParent> VW_StaticDataParent { get; set; }
        public virtual DbSet<VW_Audit> VW_Audit { get; set; }
        #endregion
        #region Category
        public virtual DbSet<Category> Category { get; set; }
        #endregion
        #region Course
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<CourseContent> CourseContent { get; set; }
        public virtual DbSet<Lecture> Lecture { get; set; }
        //  public virtual DbSet<Answer> Answer{get; set;}
        public virtual DbSet<Quiz> Quiz { get; set; }
        public virtual DbSet<Assessment> Assessment { get; set; }
        #endregion
        #region User
        public virtual DbSet<Enrollment> Enrollment { get; set; }
        public virtual DbSet<TempEnrollment> TempEnrollment { get; set; }
        public virtual DbSet<UserGrade> UserGrade { get; set; }
        public virtual DbSet<GradingCriteria> GradingCriteria { get; set; }
        public virtual DbSet<TempGrade> TempGrade { get; set; }
        #endregion


        public DbContextBase() : base() { }

        public DbContextBase(DbContextOptions options) : base(options) { }

        private Config designTimeConfig;
        protected Config DesignTimeConfig
        {
            get
            {
                if (designTimeConfig == null)
                {
                    try
                    {
                        DirectoryInfo info = new DirectoryInfo(AppContext.BaseDirectory);
                        DirectoryInfo dataProjectRoot = info.Parent.Parent.Parent.Parent;

                        string basePath = Path.Combine(dataProjectRoot.FullName, "data");

                        IConfigurationRoot config = new ConfigurationBuilder()
                            .SetBasePath(basePath)
                            .AddJsonFile("npgsql.json")
                            .Build();

                        designTimeConfig = config.GetSection("data").Get<Config>();
                    }
                    catch (Exception) { }
                }
                return designTimeConfig;
            }
        }

        protected virtual void BeforeModelCreated(ModelBuilder modelBuilder)
        {
            string schemaName = this.DesignTimeConfig?.SchemaName;

            if (!string.IsNullOrWhiteSpace(schemaName))
                modelBuilder.HasDefaultSchema(schemaName);
        }

        protected virtual void AfterModelCreated(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var prop in entity.GetProperties())
                {
                    if (prop.ClrType == typeof(string))
                        prop.IsUnicode();
                }

                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var relationship in entityType.GetForeignKeys())
                    {
                        relationship.DeleteBehavior = DeleteBehavior.Restrict;
                    }
                }
            }
        }
    }
}