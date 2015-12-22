using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Microsoft.AspNet.Identity.Owin;

namespace AAB.MVC.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Full name")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 50 characters")]
        [Required]
        public string FullName { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("AABContext", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class IdentityInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            
            string roleUser = "User";
            string roleAdmin = "Administrator";
            string adminUser = "gwoldeyesus@hotmail.com";
            string password = "123456";

            try
            {
                //Create initial Role and User
                RoleManager.Create(new IdentityRole(roleUser));
                //UserManager.Create(new ApplicationUser() { UserName = adminUser });

                //Create Role Admin if it does not exist
                if (!RoleManager.RoleExists(roleAdmin))
                {
                    var roleresult = RoleManager.Create(new IdentityRole(roleAdmin));
                }

                //Create User=Admin with password=123456
                var user = new ApplicationUser();
                user.UserName = adminUser;
                user.FullName = "Getaneh Woldemariam";
                user.Email = "gwoldeyesus@hotmail.com";
                var adminresult = UserManager.Create(user, password);
                //Add User Admin to Role 'Admin'
                if (adminresult.Succeeded)
                {
                    var result = UserManager.AddToRole(user.Id, roleAdmin);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}