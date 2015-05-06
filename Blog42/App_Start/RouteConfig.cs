using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Blog42
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Error",
                url: "Error",
                defaults: new { controller = "Error", action = "Index" }
            );

            routes.MapRoute(
                name: "Home",
                url: "{action}",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "User",
                url: "Admin/User/{action}/{id}",
                defaults: new { controller = "User", action = "All", id = 0 },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                 name: "NewComment",
                 url: "NewComment/{id}",
                 defaults: new { controller = "Comment", action = "New", id = 0 },
                 constraints: new { id = @"\d+" }
             );

            routes.MapRoute(
                name: "Comment",
                url: "Admin/Comment/{action}/{id}",
                defaults: new { controller = "Comment", action = "All", id = 0 },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                 name: "ShowPost",
                 url: "Post/{id}",
                 defaults: new { controller = "Post", action = "Show", id = 0 },
                 constraints: new { id = @"\d+" }
             );

            routes.MapRoute(
                name: "Post",
                url: "Admin/Post/{action}/{id}",
                defaults: new { controller = "Post", action = "All", id = 0 },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}