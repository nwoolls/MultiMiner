using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace MultiMiner.MobileMiner
{
    public class WebBrowserProvider
    {
        public const string DashboardController = "dashboard";
        public const string HistoryController = "history";

        private const string EmbedAction = "embed";
        private const string MobileMinerWebUrl = "http://web.mobileminerapp.com";
        private const string EmailParameter = "email";
        private const string AppKeyParameter = "key";
        private static Dictionary<string, WebBrowser> embeddedBrowsers = new Dictionary<string, WebBrowser>();
        private static bool embeddedBrowserAuthenticated = false;
        private const string FormUrlEncodedHeader = "Content-Type: application/x-www-form-urlencoded\r\n";

        public static WebBrowser GetWebBrowser(string controller, string emailAddress, string applicationKey)
        {
            string url = MobileMinerWebUrl + "/" + controller + "/" + EmbedAction;
            WebBrowser embeddedBrowser;

            //maintain 1-browser-per-controller
            //for now we only have 2 and it provides a nicer UX
            //you don't see a different controller's contents
            if (embeddedBrowsers.ContainsKey(controller))
                embeddedBrowser = embeddedBrowsers[controller];
            else
            {
                embeddedBrowser = new WebBrowser();
                embeddedBrowsers[controller] = embeddedBrowser;

                embeddedBrowser.Navigated += HandleBrowserNavigated;
            }

            bool postCredentials = !embeddedBrowserAuthenticated && !String.IsNullOrEmpty(emailAddress);
            if (postCredentials)
            {
                NameValueCollection values = HttpUtility.ParseQueryString(String.Empty);
                values[EmailParameter] = emailAddress;
                values[AppKeyParameter] = applicationKey;
                byte[] data = Encoding.UTF8.GetBytes(values.ToString());

                //use POST-Redirect-GET to login without leaving credentials in history
                embeddedBrowser.Navigate(url, null, data, FormUrlEncodedHeader);
            }
            else
            {
                //we're authenticated, just use GET for better performance
                embeddedBrowser.Navigate(url);
            }

            return embeddedBrowser;
        }

        private static void HandleBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.Segments.Last().Equals(EmbedAction, StringComparison.OrdinalIgnoreCase))
                //assume that we're authenticated, we made it to the embed URL via GET
                embeddedBrowserAuthenticated = true;
        }
    }
}
