using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace JWTAuthDemo.Models
{
    public class NotificationModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Send User Id is required")]
        public long SendUserId { get; set; }
        [Required(ErrorMessage = "Receive User Id is required")]
        public long ReceivedUserId { get; set; }
        public string Body { get; set; }
        [JsonProperty("type")]
        [Required(ErrorMessage = "Type is required")]
        public Type Type { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public Status Status { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Type { [EnumMember(Value = "offer")] Offer, 
        [EnumMember(Value = "Decline")] Decline, [EnumMember(Value = "Accept")] Accept }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status { [EnumMember(Value = "Accepted")] Accepted, 
        [EnumMember(Value = "Declined")] Declined, [EnumMember(Value = "Pending")] Pending }
}
