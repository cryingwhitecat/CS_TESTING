namespace InternTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Sales
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        [StringLength(50)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        [Required]
        [StringLength(50)]
        public string Manager { get; set; }

        public int Amount { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal SalesSum { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
