namespace northopsService.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AddressStateProvince
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AddressStateProvince()
        {
            AddressTownCities = new HashSet<AddressTownCity>();
        }

        [Key]
        public int StateProvinceId { get; set; }

        public string Name { get; set; }

        public int SortOrder { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AddressTownCity> AddressTownCities { get; set; }
    }
}
