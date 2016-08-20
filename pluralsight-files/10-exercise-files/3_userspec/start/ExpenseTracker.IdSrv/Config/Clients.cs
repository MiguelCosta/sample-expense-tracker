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

                   
                 },
                  new Client
                    {
                    ClientName = "Expense Tracker Native Client (Implicit Flow)",
                    Enabled = true,
                    ClientId = "native", 
                    Flow = Flows.Implicit,
                    RequireConsent = true,
                                        
                    RedirectUris = new List<string>
                    {
                             ExpenseTrackerConstants.ExpenseTrackerMobile
                    },

                    ScopeRestrictions = new List<string>
                    { 
                        Constants.StandardScopes.OpenId, 
                        "roles",
                        "expensetrackerapi"
                    },

                    
                    }

                    
                    

             };

        }
    }
}