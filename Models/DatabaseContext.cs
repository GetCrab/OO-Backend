using System;
using System.Collections.Generic;
using System.Linq;
using JWTAuthDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthDemo.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<DogModel> Dogs { get; set; }
        public DbSet<OfferServicesAdModel> OfferServicesAds { get; set; }
        public DbSet<RequestServicesAdModel> RequestServicesAds { get; set; }
        public DbSet<RatingModel> Ratings { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<NotificationModel>()
                .Property(p => p.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (Type)Enum.Parse(typeof(Type), v));
            modelBuilder
                .Entity<NotificationModel>()
                .Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (Status)Enum.Parse(typeof(Status), v));
        }

        public List<UserModel> GetUsers() => Users.ToList();
        public List<DogModel> GetDogs() => Dogs.ToList();
        public List<OfferServicesAdModel> GetOfferServicesAds() => OfferServicesAds.ToList();
        public List<RequestServicesAdModel> GetRequestServicesAds() => RequestServicesAds.ToList();
        public List<RatingModel> GetRatings() => Ratings.ToList();
        public List<NotificationModel> GetNotifications() => Notifications.ToList();


        public void AddUser(UserModel user)
        {
            Users.Add(user);
            this.SaveChanges();
            return;
        }

        public void AddDog(DogModel dog)
        {
            Dogs.Add(dog);
            this.SaveChanges();
            return;
        }

        public void AddOfferServicesAd(OfferServicesAdModel ad)
        {
            OfferServicesAds.Add(ad);
            this.SaveChanges();
            return;
        }

        public void AddRequestServicesAd(RequestServicesAdModel ad)
        {
            RequestServicesAds.Add(ad);
            this.SaveChanges();
            return;
        }

        public void AddRating(RatingModel rating)
        {
            Ratings.Add(rating);
            this.SaveChanges();
            return;
        }
        public void AddNotification(NotificationModel notification)
        {
            Notifications.Add(notification);
            this.SaveChanges();
            return;
        }

        public void RemoveUser(UserModel user)
        {
            Users.Remove(user);
            this.SaveChanges();
            return;
        }

        public void RemoveDog(DogModel dog)
        {
            Dogs.Remove(dog);
            this.SaveChanges();
            return;
        }

        public void RemoveOfferServicesAd(OfferServicesAdModel ad)
        {
            OfferServicesAds.Remove(ad);
            this.SaveChanges();
            return;
        }

        public void RemoveRequestServicesAd(RequestServicesAdModel ad)
        {
            RequestServicesAds.Remove(ad);
            this.SaveChanges();
            return;
        }

        public void RemoveRating(RatingModel rating)
        {
            Ratings.Remove(rating);
            this.SaveChanges();
            return;
        }
        public void RemoveNotification(NotificationModel notification)
        {
            Notifications.Remove(notification);
            this.SaveChanges();
            return;
        }
    }
}