using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;

namespace ExpenseTracker.MobileClient.Helpers
{
    public class ExpenseTrackerIdentity
    {
 
        public List<Claim> Claims { get; private set; }
 
        public static async Task<ExpenseTrackerIdentity> Create(AuthorizeResponse authorizeResponse)
        {
          
            var claims = new List<Claim>();

            // call the userinfo endpoint to get identity information, 
            // using the access token.

            var userInfo = await EndpointAndTokenHelper.CallUserInfoEndpoint(authorizeResponse.AccessToken);

            // decode the token to get the issuer
            var decodedIdToken = EndpointAndTokenHelper.DecodeToken(authorizeResponse.IdentityToken);


            // extract the data that's useful for us
            JToken sub, iss;

            if (decodedIdToken.TryGetValue("sub", out sub)
                && decodedIdToken.TryGetValue("iss", out iss))
            {
                claims.Add(new Claim("unique_user_key", iss.ToString() + "_" + sub.ToString()));
            }

            var roles = userInfo.Value<JArray>("role").ToList();


            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role.ToString()));
            }
            
            // keep the access token - we'll need it afterwards for API access
            claims.Add(new Claim("access_token", authorizeResponse.AccessToken));
            
            return new ExpenseTrackerIdentity(claims);
        }

        public ExpenseTrackerIdentity(List<Claim> claims)
        {
            Claims = claims;
        }
    }


    public class Claim
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Claim(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
