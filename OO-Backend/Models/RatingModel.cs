using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemo.Models
{
    public class RatingModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Send User Id is required")]
        public long SendUserId { get; set; }
        [Required(ErrorMessage = "Receive User Id is required")]
        public long ReceivedUserId { get; set; }
        public string Body { get; set; }
        [Required(ErrorMessage = "Mark is required")]
        [Range(1, 5, ErrorMessage = "Mark must be between 1 and 5")]
        public int Mark { get; set; }
    }
}
