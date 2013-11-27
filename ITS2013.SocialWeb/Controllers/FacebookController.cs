using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ITS2013.Social;
using Newtonsoft.Json;

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

        public ContentResult FriendsOfMine()
        {
            var social_state = JsonConvert.DeserializeObject<SocialState>(
                Request.Cookies["social_state"].Value
            );

            var url = string.Format(
                "https://graph.facebook.com/me/friends?{0}"
                , social_state.AccessTokenQuery
            );

            var client = new WebClient();

            var friends_json = client.DownloadString(url);

            return Content(friends_json, "application/json");
        }

        public ActionResult Logout()
        {
            var url_referrer = Url.Action("Index", "Home");

            try
            {
                var social_state = JsonConvert.DeserializeObject<SocialState>(
                    Request.Cookies["social_state"].Value
                );

                var url = string.Format(
                    "https://graph.facebook.com/me/permissions?method=delete&{0}"
                    , social_state.AccessTokenQuery
                );

                var client = new WebClient();

                var result_json = client.DownloadString(url);
            }
            finally
            {
                Response.Cookies["social_state"].Expires = DateTime.Now.AddDays(-1);
            }

            return Redirect(url_referrer);
        }

        public FileResult Picture(string id = "me")
        {
            var social_state = JsonConvert.DeserializeObject<SocialState>(
                Request.Cookies["social_state"].Value
            );

            var client = new WebClient();
            var url = string.Format(
                "https://graph.facebook.com/{1}/picture?{0}"
                , social_state.AccessTokenQuery
                , id
            );

            var picture_data = client.DownloadData(url);

            return new FileContentResult(picture_data, "image/png");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.Cookies.AllKeys.Contains("social_state"))
            {
                ViewBag.is_authenticated = true;
                ViewBag.social_state =
                    JsonConvert.DeserializeObject<SocialState>(
                        Request.Cookies["social_state"].Value
                    );
            }
            else if (Request.QueryString.AllKeys.Contains("code"))
            {
                var request_url = Request.Url.ToString();
                request_url = request_url.Substring(0, request_url.LastIndexOf("code=") - 1);
                var code = Request.QueryString["code"];

                var url = string.Format("https://graph.facebook.com/oauth/access_token"
                    + "?client_id={0}"
                    + "&redirect_uri={1}"
                    + "&client_secret={2}"
                    + "&code={3}"
                    , "219248301582245"
                    , Url.Encode(request_url)
                    , "ee5868e178e023771682a108667da991"
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

                var social_state = new SocialState
                {
                    ClientId = "219248301582245"
                    ,
                    AccessTokenQuery = access_token_query
                    ,
                    Me = JsonConvert.DeserializeObject<SocialUser>(me_json)
                };

                Response.AppendCookie(
                    new HttpCookie(
                        "social_state"
                        , JsonConvert.SerializeObject(social_state)
                    )
                    {
                        Expires = DateTime.Now.AddDays(7)
                    });
                ViewBag.social_state = social_state;
                ViewBag.is_authenticated = true;

                Response.Redirect(request_url);
            }
            else
            {
                ViewBag.is_authenticated = false;
            }
        }
    }
}
