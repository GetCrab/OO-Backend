using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemo.Models
{
    public class DogModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Owner Id is required")]
        public long OwnerId { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
    }
}
