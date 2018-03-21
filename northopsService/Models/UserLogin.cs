namespace northopsService.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserLogin
    {
        [Required]
        [StringLength(128)]
        public string LoginProvider { get; set; }

        [Required]
        [StringLength(128)]
        public string ProviderKey { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public int Id { get; set; }

        public virtual User User { get; set; }
    }
}
