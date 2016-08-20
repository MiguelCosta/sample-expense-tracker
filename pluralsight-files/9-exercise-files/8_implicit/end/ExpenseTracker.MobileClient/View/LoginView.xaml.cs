using ExpenseTracker.MobileClient.Common;
using ExpenseTracker.MobileClient.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Thinktecture.IdentityModel.Client;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Security.Authentication.Web;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace ExpenseTracker.MobileClient.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : Page, IWebAuthenticationContinuable
    {
        private NavigationHelper navigationHelper;
 

        public LoginView()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
             
        }

        
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

   
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // for reference: if you ever need to get the correct callbackUri
            // for your app
            // var callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();


            var client = new OAuth2Client(
                         new Uri(ExpenseTrackerConstants.IdSrvAuthorize));

            string nonce = Guid.NewGuid().ToString() + DateTime.Now.Ticks.ToString();

            var startUrl = client.CreateAuthorizeUrl(
                "native",
                "id_token token",
                 "openid roles",
                 ExpenseTrackerConstants.ExpenseTrackerMobile,
                 "state", nonce);


            WebAuthenticationBroker.AuthenticateAndContinue
                (
                 new Uri(startUrl),
                 new Uri(ExpenseTrackerConstants.ExpenseTrackerMobile),
             null,
             WebAuthenticationOptions.None);

          
        } 

        public async void ContinueWebAuthentication(Windows.ApplicationModel.Activation.WebAuthenticationBrokerContinuationEventArgs args)
        {
            if (args.WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var authResponse = new AuthorizeResponse(args.WebAuthenticationResult.ResponseData);

                // create & save the information we need related to the user, and the access token
                App.ExpenseTrackerIdentity = await ExpenseTrackerIdentity.Create(authResponse);

                // redirect to the first page
                Frame.Navigate(typeof(ExpenseGroupsView)); 
 
            }
            
        }
 
 
    }
}
