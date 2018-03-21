namespace northopsService.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class NorthopsContext : DbContext
    {
        public virtual DbSet<AddressStateProvince> AddressStateProvinces { get; set; }
        public virtual DbSet<AddressTownCity> AddressTownCities { get; set; }
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
               
    }
}
