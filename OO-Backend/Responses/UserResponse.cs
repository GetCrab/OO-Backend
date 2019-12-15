using JWTAuthDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemo.Responses
{
    public class UserResponse
    {
        public UserResponse(long id, string userName, string emailAddress, 
            List<DogModel> dogs, List<RatingModel> ratings, List<NotificationModel> notifications)
        {
            Id = id;
            UserName = userName;
            EmailAddress = emailAddress;
            Dogs = dogs;
            Ratings = ratings;
            Notifications = notifications;
        }

        public long Id { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public List<DogModel> Dogs { get; set; }
        public List<RatingModel> Ratings { get; set; }
        public List<NotificationModel> Notifications { get; set; }
    }
}
