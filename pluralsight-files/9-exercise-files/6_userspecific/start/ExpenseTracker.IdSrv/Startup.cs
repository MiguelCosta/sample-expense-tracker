using ExpenseTracker.IdSrv.Config;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Newtonsoft.Json.Linq;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using Thinktecture.IdentityServer.Core.Configuration;
 

[assembly: OwinStartup(typeof(ExpenseTracker.IdSrv.Startup))]

namespace ExpenseTracker.IdSrv
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",
                    IssuerUri = ExpenseTrackerConstants.IdSrvIssuerUri,
                    SigningCertificate = LoadCertificate(),

                    Factory = InMemoryFactory.Create(
                        users: Users.Get(),
                        clients: Clients.Get(),
                        scopes: Scopes.Get()),

                    AuthenticationOptions = new Thinktecture.IdentityServer.Core.Configuration.AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders
                    }

                });
            });
        }


        
        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                Caption = "Sign-in with Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "366449833533211",
                AppSecret = "b3989811ef90affeffd3d5002e625944",
                Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {

                        JToken lastName, firstName;
                        if (context.User.TryGetValue("last_name", out lastName))
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim(
                                Thinktecture.IdentityServer.Core.Constants.ClaimTypes.FamilyName,
                                lastName.ToString()));
                        }

                        if (context.User.TryGetValue("first_name", out firstName))
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim(
                                Thinktecture.IdentityServer.Core.Constants.ClaimTypes.GivenName,
                                firstName.ToString()));
                        }

                        context.Identity.AddClaim(new System.Security.Claims.Claim("role", "WebReadUser"));
                        context.Identity.AddClaim(new System.Security.Claims.Claim("role", "WebWriteUser"));

                        context.Identity.AddClaim(new System.Security.Claims.Claim("role", "MobileReadUser"));
                        context.Identity.AddClaim(new System.Security.Claims.Claim("role", "MobileWriteUser"));

                        return Task.FromResult(0);
                    }
                }


            });
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\idsrv3test.pfx",
                AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
 
    }
}