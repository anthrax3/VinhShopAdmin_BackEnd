namespace VinhShopApi.Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Model.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<VinhShopApi.Data.VinhShopDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(VinhShopApi.Data.VinhShopDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            CreateUser(context);
        }

        private void CreateUser(VinhShopDbContext context)
        {
            var manager = new UserManager<AppUser>(new UserStore<AppUser>(new VinhShopDbContext()));
            if (manager.Users.Count() == 0)
            {
                var roleManager = new RoleManager<AppRole>(new RoleStore<AppRole>(new VinhShopDbContext()));

                var user = new AppUser()
                {
                    UserName = "admin",
                    Email = "quangvinh050293@gmail.com",
                    EmailConfirmed = true,
                    BirthDay = DateTime.Now,
                    FullName = "Đặng Quang Vinh",
                    Avatar = "/assets/images/img.jpg",
                    Gender = true,
                    Status = true
                };
                if (manager.Users.Count(x => x.UserName == "admin") == 0)
                {
                    manager.Create(user, "123654$");

                    if (!roleManager.Roles.Any())
                    {
                        roleManager.Create(new AppRole { Name = "Admin", Description = "Quản trị viên" });
                        roleManager.Create(new AppRole { Name = "Member", Description = "Người dùng" });
                    }

                    var adminUser = manager.FindByName("admin");

                    manager.AddToRoles(adminUser.Id, new string[] { "Admin", "Member" });
                }
            }
        }
    }
}
