using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinktecture.IdentityServer.Core.Models;

namespace ExpenseTracker.IdSrv.Config
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>
                {
 
                    // identity scopes

                    StandardScopes.OpenId,
                    StandardScopes.Profile,
                    new Scope
                    {
                        Enabled = true,
                        Name = "roles",
                        DisplayName = "Roles",
                        Description = "The roles you belong to.",
                        Type = ScopeType.Identity,
                        Claims = new List<ScopeClaim>
                        {
                            new ScopeClaim("role")
                        }
                    },
                    new Scope
                    {
                        Name = "expensetrackerapi",
                        DisplayName = "ExpenseTracker API Scope",
                        Type = ScopeType.Resource,
                        Emphasize = false,
                         Enabled = true
                    },

                    
                 };

            return scopes;
        }
 
    }
}