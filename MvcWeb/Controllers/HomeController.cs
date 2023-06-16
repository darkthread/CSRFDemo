using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MvcWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public class RegistrationEntry
        {
            public DateTime RegTime { get; set; }
            public string EventName { get; set; }
            public string UserId { get; set; }
        }
        static ConcurrentQueue<RegistrationEntry> _registrations = new ConcurrentQueue<RegistrationEntry>();

        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Query()
        {
            return Json(
                _registrations.Select(o =>
                    $"{o.RegTime:MM-dd HH:mm:ss} {o.UserId} - {o.EventName}"
                ).ToList());
        }

        [HttpPost]
        public ActionResult Register(string eventName)
        {
            try
            {
                // TODO: 可改寫成 IAuthorizationFilter 方便套用
                var token = Request.Headers["X-CSRF-TOKEN"];
                var p = token.Split(':');
                AntiForgery.Validate(p[0], p[1]);
            }
            catch
            {
                return Content("Invalid Token");
            }
            _registrations.Enqueue(new RegistrationEntry
            {
                RegTime = DateTime.Now,
                EventName = eventName,
                UserId = User.Identity.Name.Split('\\').Last()
            });
            return Content("Registered");
        }
    }
}