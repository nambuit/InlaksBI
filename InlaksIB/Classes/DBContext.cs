using System;
using System.Collections.Generic;
using MySql.Data.Entity;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Data.Entity.ModelConfiguration.Conventions;
using BackBone;

namespace InlaksIB
{
  
    public class InlaksBIContext : DbContext
    {
        public InlaksBIContext()
            : base("InlaksIB.Properties.Settings.DBConstr")
        {

            //Database.CreateIfNotExists();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Either entirely
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

        }

       public DbSet<User> Users { get; set; }

        public DbSet<AuthSetup>  AuthConfig{get;set;}

        public DbSet<Module> Modules { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<RoleResource> RolesResources { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Company> Comapanies { get; set; }

        public DbSet<SubMenu> SubMenus { get; set; }

        public DbSet<SubSubMenu> SubSubMenus { get; set; }

        public DbSet<SubSubSubMenu> SubSubSubMenus { get; set; }

        public DbSet<Industry> Industries { get; set; }

        public DbSet<Migration_Schedule> MigrationSchedules { get; set; }

        public DbSet<ResourceReport> ResourceReports { get; set; }

        public DbSet<DataSetFilter> DataSetFilters { get; set; }


        public DbSet<UserReportState> UserReportStates { get; set; }

        public DbSet<DataSetDetail> DataSets { get; set; }

        public void PopulateDefaultData()
        {
            IPasswordHasher phasher = new BasicHash();

            var role = new Role() { RoleID=1, RoleName = "Administrator" };
            Roles.Add(role);
            var hashed = phasher.HashPassword("adminpassword");


            Users.Add(new User() { Name = "Administrator", Password = hashed, RePassword = hashed, UserID = "admin", UserRole = role, RoleID=1  });
            AuthConfig.Add(new AuthSetup() { AuthType = "DATABASE", Server = "" });

            //  var industry = new Industry() { IndustryID = "FPCPENCOM", IndustryName = "FIRST PENSION" };

            // var module = new Module() { IconClass = "glyphicon glyphicon-list", ModuleID = 1, value = "amcom", ModuleName = "PENCOM Reports", Industry = industry };

            // Modules.Add(module);

            // Resources.Add(new Resource() { ResourceID = 1, ResourceName = "Forfeited Items Report", Url = "ReportLauncher/amcom_forfeit", value = "amcom_forfeit", Module = module });

            //  Resources.Add(new Resource() { ResourceID = 2, ResourceName = "Released Items Report", Url = "ReportLauncher/amcom_release", value = "amcom_release", Module = module });






            SaveChanges();



        }

        public WarehouseInterface getWarehouse(string dbtype)
        {
            WarehouseInterface warehouse;

            switch (dbtype.CleanUp())
            {
                case "mysql":
                    warehouse = new MySqlWarehouse();
                    break;

                case "mongodb":
                    warehouse = new MongoWarehouse();
                    break;

                case "pgsql":
                    warehouse = new PostgreSqlWarehouse();
                    break;
                default:
                    warehouse = new SqlServerWarehouse();
                    break;
            }

            return warehouse;
        }



        public DBInterface getDBInterface(string dbtype, string constr)
        {
            DBInterface dbinterface;

            switch (dbtype.CleanUp())
            {
                case "mysql":
                    dbinterface = new MySQLDBInterface(constr);
                    break;

                case "mongodb":
                    dbinterface = null;
                    break;

                case "pgsql":
                    dbinterface = new PostgreSQLDBInterface(constr);
                    break;
                default:
                    dbinterface = new SQLServerDBInterfac(constr);
                    break;
            }

            return dbinterface;
        }

    }

  

   
    public struct AgeDistribution
    {
        public string AgeGroup;

        public int Count;
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UnlikeAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The value of {0} cannot be the same as the value of the {1}.";

        public string OtherPropertyDisplayName { get; private set; }
        public string OtherProperty { get; private set; }

        public UnlikeAttribute(string otherProperty)
            : base(DefaultErrorMessage)
        {
            if (string.IsNullOrEmpty(otherProperty))
            {
                throw new ArgumentNullException("otherProperty");
            }

            OtherProperty = otherProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherPropertyDisplayName);
        }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectInstance.GetType()
                    .GetProperty(OtherProperty);

                var otherPropertyValue = otherProperty
                    .GetValue(validationContext.ObjectInstance, null);

                if (value.Equals(otherPropertyValue))
                {
                    OtherPropertyDisplayName = otherProperty.GetCustomAttribute<DisplayAttribute>().Name;
                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

    }

}
