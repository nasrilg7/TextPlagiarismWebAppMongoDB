namespace TextPlagiarismWebApp.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "TextPlagiarismWebApp.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            if(!context.Users.Any(u => u.Email == "nasri.yatim.95@gmail.com"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var user = new ApplicationUser
                {
                    UserName ="Nasri",
                    Email = "nasri.yatim.95@gmail.com"
                };

                userManager.Create(user, "NasriYatim1995..");
                userManager.AddToRole(user.Id, "admin");
            }

        }
    }
}
