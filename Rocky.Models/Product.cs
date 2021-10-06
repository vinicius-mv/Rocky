 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models
{
    public class Product
    {
        public Product()
        {
            TempSqft = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        [Range(1, double.MaxValue)]
        public double Price { get; set; }

        public string Image { get; set; }

        [NotMapped]
        [Range(1, 10_0000)]
        public int TempSqft { get; set; }

        // nav props
        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public int ApplicationTypeId { get; set; }

        [ForeignKey("ApplicationTypeId")]
        public virtual ApplicationType ApplicationType { get; set; }
    }
}
