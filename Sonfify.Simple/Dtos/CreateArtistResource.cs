using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Songify.Simple.Dtos
{
    public class CreateArtistResource : IValidatableObject
    {
        [Required]
        [MaxLength(300)]
        public string Name { get; set; }

        public string Origin { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool? IsActive { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var property = new[] {"CreatedAt"};
            if (CreatedAt > DateTime.Now)
            {
                yield return new ValidationResult("CreatedAt cannot be later than current date time", property);
            }
        }
    }
}