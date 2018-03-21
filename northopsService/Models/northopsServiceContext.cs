using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;

namespace northopsService.Models
{
    public partial class NorthopsContext : DbContext
    {

        public virtual DbSet<TodoItem> TodoItem { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //   Database.SetInitializer(new northopsInitializer());
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
            modelBuilder.Entity<User>()
               .HasMany(e => e.UserRoles)
               .WithMany(e => e.Users)
               .Map(m => m.ToTable("UsersInRoles").MapLeftKey("UserId").MapRightKey("RoleId"));
        }
        private const string connectionStringName = "Name=MS_TableConnectionString";

        public NorthopsContext() : base(connectionStringName)
        {
        }
    }
    
}
