using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinktecture.IdentityServer.AccessTokenValidation;


[assembly: OwinStartup(typeof(ExpenseTracker.API.Startup))]

namespace ExpenseTracker.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseIdentityServerBearerTokenAuthentication(new
            IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = ExpenseTrackerConstants.IdSrv,
                RequiredScopes = new[] { "expensetrackerapi" }
            });
            
            app.UseWebApi(WebApiConfig.Register()); 
             
        }
    }
}