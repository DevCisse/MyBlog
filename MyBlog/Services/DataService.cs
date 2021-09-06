using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Models;
using MyBlog.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Services
{
    public class DataService
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly UserManager<BlogUser> userManager;

       

      



        public DataService(RoleManager<IdentityRole> roleManager,ApplicationDbContext applicationDbContext, UserManager<BlogUser> userManager)
        {
            this.roleManager = roleManager;
            this.applicationDbContext = applicationDbContext;
            this.userManager = userManager;
        }




        public async Task ManageDataAsync()
        {
            await applicationDbContext.Database.MigrateAsync(); 

            //Task 1 : Seed a few Roles into the system
            await SeedRolesAsync();



            //Task 2 : Seed a few users into the system
            await SeedUsersAsync();
        }



        private async Task SeedRolesAsync()
        {

            if(applicationDbContext.Roles.Any())
            {
                return;
            }


            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = role.ToString()
                });
            };

        }

        private async Task SeedUsersAsync()
        {
            if(applicationDbContext.Users.Any())
            {
                return;
            }


            //create a admin user
            var adminUser = new BlogUser
            {
                Email = "Hassancisse2000@gmail.com",
                UserName = "Hassancisse2000@gmail.com",
                FirstName = "Hassan",
                LastName = "Cisse",
                EmailConfirmed = true
            };


            //add user to the system


            await userManager.CreateAsync(adminUser,"Abc&123!");

            // add user to admin

           await  userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());


            //create a  user
            var moderatorUserUser = new BlogUser
            {
                Email = "alhazzan10@yahoo.com",
                UserName = "alhazzan10@yahoo.com",
                FirstName = "Hazzan",
                LastName = "Cisse",
                EmailConfirmed = true
            };


            //add user to the system


            await userManager.CreateAsync(moderatorUserUser,"Abc&123!");

            // add user to mod

            await userManager.AddToRoleAsync(moderatorUserUser, BlogRole.Moderator.ToString());


        }


    }
}
