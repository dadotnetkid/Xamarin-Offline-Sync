//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace northopsService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AddressTownCity
    {
        public int TownCityId { get; set; }
        public string Name { get; set; }
        public int StateProvinceId { get; set; }
        public int SortOrder { get; set; }
    
        public virtual AddressStateProvince AddressStateProvince { get; set; }
    }
}
