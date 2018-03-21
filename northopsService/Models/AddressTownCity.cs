namespace northopsService.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AddressTownCity
    {
        [Key]
        public int TownCityId { get; set; }

        public string Name { get; set; }

        public int StateProvinceId { get; set; }

        public int SortOrder { get; set; }

        public virtual AddressStateProvince AddressStateProvince { get; set; }
    }
}
