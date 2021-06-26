using System;
using System.ComponentModel.DataAnnotations;

namespace Songify.Simple.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(300)]
        public string Name { get; set; }

        public string Origin { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool? IsActive { get; set; }
    }
}