using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [StringLength(maximumLength: 50)]
        public string Name { get; set; }

        public string UserId { get; set; }
    }
}