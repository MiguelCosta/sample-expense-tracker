using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Models;

namespace ExpenseTracker.IdSrv.Config
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
             {
                new Client 
                {
                     Enabled = true,
                     ClientId = "mvc",
                     ClientName = "ExpenseTracker MVC Client (Hybrid Flow)",
                     Flow = Flows.Hybrid, 
                     RequireConsent = true,  
      
                    // redirect = URI of MVC app
                    RedirectUris = new List<string>
                    {
                        ExpenseTrackerConstants.ExpenseTrackerClient
                    },

                   
                 }
                    

             };

        }
    }
}