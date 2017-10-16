namespace VinhShopApi.Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Model.Models;
    using System;
    using System.Collections.Generic;
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
            //CreateUser(context);
            CreateFunction(context);
        }

        private void CreateFunction(VinhShopDbContext context)
        {
            if (context.Functions.Count() == 0)
            {
                context.Functions.AddRange(new List<Function>()
                {
                    new Function() {ID = "SYSTEM", Name = "Hệ thống",ParentId = null,DisplayOrder = 1,Status = true,URL = "/",IconCss = "fa-desktop"  },
                    new Function() {ID = "ROLE", Name = "Nhóm",ParentId = "SYSTEM",DisplayOrder = 1,Status = true,URL = "/main/role/index",IconCss = "fa-home"  },
                    new Function() {ID = "FUNCTION", Name = "Chức năng",ParentId = "SYSTEM",DisplayOrder = 2,Status = true,URL = "/main/function/index",IconCss = "fa-home"  },
                    new Function() {ID = "USER", Name = "Người dùng",ParentId = "SYSTEM",DisplayOrder =3,Status = true,URL = "/main/user/index",IconCss = "fa-home"  },
                    new Function() {ID = "ACTIVITY", Name = "Nhật ký",ParentId = "SYSTEM",DisplayOrder = 4,Status = true,URL = "/main/activity/index",IconCss = "fa-home"  },
                    new Function() {ID = "ERROR", Name = "Lỗi",ParentId = "SYSTEM",DisplayOrder = 5,Status = true,URL = "/main/error/index",IconCss = "fa-home"  },
                    new Function() {ID = "SETTING", Name = "Cấu hình",ParentId = "SYSTEM",DisplayOrder = 6,Status = true,URL = "/main/setting/index",IconCss = "fa-home"  },

                    new Function() {ID = "PRODUCT",Name = "Sản phẩm",ParentId = null,DisplayOrder = 2,Status = true,URL = "/",IconCss = "fa-chevron-down"  },
                    new Function() {ID = "PRODUCT_CATEGORY",Name = "Danh mục",ParentId = "PRODUCT",DisplayOrder =1,Status = true,URL = "/main/product-category/index",IconCss = "fa-chevron-down"  },
                    new Function() {ID = "PRODUCT_LIST",Name = "Sản phẩm",ParentId = "PRODUCT",DisplayOrder = 2,Status = true,URL = "/main/product/index",IconCss = "fa-chevron-down"  },
                    new Function() {ID = "ORDER",Name = "Hóa đơn",ParentId = "PRODUCT",DisplayOrder = 3,Status = true,URL = "/main/order/index",IconCss = "fa-chevron-down"  },

                    new Function() {ID = "CONTENT",Name = "Nội dung",ParentId = null,DisplayOrder = 3,Status = true,URL = "/",IconCss = "fa-table"  },
                    new Function() {ID = "POST_CATEGORY",Name = "Danh mục",ParentId = "CONTENT",DisplayOrder = 1,Status = true,URL = "/main/post-category/index",IconCss = "fa-table"  },
                    new Function() {ID = "POST",Name = "Bài viết",ParentId = "CONTENT",DisplayOrder = 2,Status = true,URL = "/main/post/index",IconCss = "fa-table"  },

                    new Function() {ID = "UTILITY",Name = "Tiện ích",ParentId = null,DisplayOrder = 4,Status = true,URL = "/",IconCss = "fa-clone"  },
                    new Function() {ID = "FOOTER",Name = "Footer",ParentId = "UTILITY",DisplayOrder = 1,Status = true,URL = "/main/footer/index",IconCss = "fa-clone"  },
                    new Function() {ID = "FEEDBACK",Name = "Phản hồi",ParentId = "UTILITY",DisplayOrder = 2,Status = true,URL = "/main/feedback/index",IconCss = "fa-clone"  },
                    new Function() {ID = "ANNOUNCEMENT",Name = "Thông báo",ParentId = "UTILITY",DisplayOrder = 3,Status = true,URL = "/main/announcement/index",IconCss = "fa-clone"  },
                    new Function() {ID = "CONTACT",Name = "Lien hệ",ParentId = "UTILITY",DisplayOrder = 4,Status = true,URL = "/main/contact/index",IconCss = "fa-clone"  },

                    new Function() {ID = "REPORT",Name = "Báo cáo",ParentId = null,DisplayOrder = 5,Status = true,URL = "/",IconCss = "fa-bar-chart-o"  },
                    new Function() {ID = "REVENUES",Name = "Báo cáo doanh thu",ParentId = "REPORT",DisplayOrder = 1,Status = true,URL = "/main/report/revenues",IconCss = "fa-bar-chart-o"  },
                    new Function() {ID = "ACCESS",Name = "Báo cáo truy cập",ParentId = "REPORT",DisplayOrder = 2,Status = true,URL = "/main/report/visitor",IconCss = "fa-bar-chart-o"  },
                    new Function() {ID = "READER",Name = "Báo cáo độc giả",ParentId = "REPORT",DisplayOrder = 3,Status = true,URL = "/main/report/reader",IconCss = "fa-bar-chart-o"  },

                });
                context.SaveChanges();
            }
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
