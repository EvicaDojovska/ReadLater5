using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class Bookmark
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 500)]
        public string Url { get; set; }

        [Required]
        public string ShortDescription { get; set; }

        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public DateTime CreateDate { get; set; }

        public string UserId { get; set; }

        public string ShortCode { get; set; }
    }
}
