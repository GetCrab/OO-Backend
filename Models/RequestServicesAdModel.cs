using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemo.Models
{
    public class RequestServicesAdModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "User Id is required")]
        public long UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
    }
}
