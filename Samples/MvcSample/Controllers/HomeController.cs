using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;

namespace GithubSharp.Samples.MvcSample.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string id)
        {
            var model = new Models.ViewModels.LoginViewModel { ReturnURL = id };
            if (Request.IsAjaxRequest())
                return PartialView("LoginControl", model);
            return View(GetBaseView(model));
        }

        [ValidateAntiForgeryToken()]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(string user, string apitoken, string returnURL)
        {
            var userAPI = new GithubSharp.Core.API.User { CacheProvider = WebCacher, LogProvider = LogProvider };
            userAPI.Authenticate(new GithubSharp.Core.Models.GithubUser { Name = user, APIToken = apitoken });
            try
            {
                var privateuser = userAPI.Get();
                if (privateuser != null)
                {
                    CurrentUser = new GithubSharp.Core.Models.GithubUser { Name = user, APIToken = apitoken };

                    SetTemporaryNotification("Login succeded");

                    if (Request.IsAjaxRequest())
                        return Json(new { success = true, Name = user });
                    if (string.IsNullOrEmpty(returnURL))
                        return View("Index");
                    return Redirect(returnURL);
                }
                else throw new Exception("Invalid user");
            }
            catch (Exception error)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = error.Message });
                return View(GetBaseView(new Models.ViewModels.LoginViewModel { Message = error.Message, ReturnURL = returnURL }));
            }
        }

    }
}
