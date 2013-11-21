using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ITS2013.SocialWeb.Controllers
{
    public class FacebookController : Controller
    {
        public ActionResult Login()
        {
            var url_referrer = Request.UrlReferrer.ToString();
            var url =
                string.Format(
                    "https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&state={2}&scope={3}"
                    , "219248301582245"
                    , Url.Encode(url_referrer)
                    , string.Empty
                    , "user_about_me,read_stream,publish_actions"
                );
            return Redirect(url);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.QueryString.AllKeys.Contains("code"))
            {
                var request_url = Request.Url.ToString();
                request_url = request_url.Substring(0, request_url.LastIndexOf("code=")-1);
                var code = Request.QueryString["code"];

                var url = string.Format("https://graph.facebook.com/oauth/access_token"
                    + "?client_id={0}"
                    + "&redirect_uri={1}"
                    + "&client_secret={2}"
                    + "&code={3}"
                    , "219248301582245"
                    , Url.Encode(request_url)
                    , "5dbebbcf7ad19f70e8b25946a09d2bce"
                    , code
                );

                var client = new WebClient();
                var access_token_query = client.DownloadString(url);

                url = string.Format(
                    "https://graph.facebook.com/me?{0}"
                    , access_token_query
                );
                var me_json =
                    client.DownloadString(url);

                Response.Redirect(request_url);
            }
        }
    }
}
