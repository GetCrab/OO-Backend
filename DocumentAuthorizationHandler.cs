using JWTAuthDemo.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemo
{
    public class DocumentAuthorizationHandler :
    AuthorizationHandler<SameAuthorRequirement, UserModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       SameAuthorRequirement requirement,
                                                       UserModel resource)
        {
            Console.WriteLine("Context name: ");
            Console.WriteLine("Resource name: ");
            Console.WriteLine(resource.UserName);
            if (context.User.Identity?.Name == resource.UserName)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class SameAuthorRequirement : IAuthorizationRequirement { }
}
