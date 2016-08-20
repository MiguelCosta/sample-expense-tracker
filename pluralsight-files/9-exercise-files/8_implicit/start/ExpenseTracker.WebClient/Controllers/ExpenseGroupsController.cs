using ExpenseTracker.DTO;
using ExpenseTracker.WebClient.Helpers;
using ExpenseTracker.WebClient.Models;
using Marvin.JsonPatch;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;

namespace ExpenseTracker.WebClient.Controllers
{
 

    public class ExpenseGroupsController : Controller
    {

        [ResourceAuthorize("Read", "ExpenseGroup")]
        public async Task<ActionResult> Index(int? page = 1)
        {
         
            var client = ExpenseTrackerHttpClient.GetClient();

            var model = new ExpenseGroupsViewModel();

            HttpResponseMessage egsResponse = await client.GetAsync("api/expensegroupstatusses");
           
            if (egsResponse.IsSuccessStatusCode)
            {
                string egsContent = await egsResponse.Content.ReadAsStringAsync();
                var lstExpenseGroupStatusses = JsonConvert.DeserializeObject<IEnumerable<ExpenseGroupStatus>>(egsContent);
                model.ExpenseGroupStatusses = lstExpenseGroupStatusses;
            }
            else
            {
                return Content("An error occurred.");
            }

            string userId = (this.User.Identity as ClaimsIdentity).FindFirst("unique_user_key").Value;

            HttpResponseMessage response = await client.GetAsync("api/expensegroups?sort=expensegroupstatusid"
                + ",title&page=" + page + "&pagesize=5&userid=" + userId);


            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                // get the paging info from the header
                var pagingInfo = HeaderParser.FindAndParsePagingInfo(response.Headers);

                var lstExpenseGroups = JsonConvert.DeserializeObject<IEnumerable<ExpenseGroup>>(content);

                var pagedExpenseGroupsList = new StaticPagedList<ExpenseGroup>(lstExpenseGroups, pagingInfo.CurrentPage, 
                    pagingInfo.PageSize, pagingInfo.TotalCount);
                
                model.ExpenseGroups = pagedExpenseGroupsList;
                model.PagingInfo = pagingInfo;
            }
            else
            {
                return Content("An error occurred.");
            }


            return View(model);

        }

 
        // GET: ExpenseGroups/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var client = ExpenseTrackerHttpClient.GetClient();

            HttpResponseMessage response = await client.GetAsync("api/expensegroups/" + id
                                + "?fields=id,description,title,expenses");
            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<ExpenseGroup>(content);
                return View(model);
            }

            return Content("An error occurred");
        }




        // GET: ExpenseGroups/Create
        public ActionResult Create()
        {
            return View();  
        }

        // POST: ExpenseGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ExpenseGroup expenseGroup)
        {
             try
            {
               var client = ExpenseTrackerHttpClient.GetClient();

               var claimsIdentity = this.User.Identity as ClaimsIdentity;
               var userId = claimsIdentity.FindFirst("unique_user_key").Value;

               // an expensegroup is created with status "Open", for the current user
               expenseGroup.ExpenseGroupStatusId = 1;
               expenseGroup.UserId = userId;

               var serializedItemToCreate = JsonConvert.SerializeObject(expenseGroup);

               var response = await client.PostAsync("api/expensegroups",
                 new StringContent(serializedItemToCreate,
                 System.Text.Encoding.Unicode, "application/json"));

               if (response.IsSuccessStatusCode)
               {
                   return RedirectToAction("Index");
               }
               else
               {
                   return Content("An error occurred");
               }

            }
            catch  
            {
               return Content("An error occurred.");
            }

        }


         [ResourceAuthorize("Write", "ExpenseGroup")]
        // GET: ExpenseGroups/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var client = ExpenseTrackerHttpClient.GetClient();

            HttpResponseMessage response = await client.GetAsync("api/expensegroups/" + id 
                + "?fields=id,title,description");
            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<ExpenseGroup>(content);
                return View(model);
            }

            return Content("An error occurred: " + content);

        }

        // POST: ExpenseGroups/Edit/5   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ExpenseGroup expenseGroup)
        {
            try
            {
                var client = ExpenseTrackerHttpClient.GetClient();

                // create a JSON Patch Document
                JsonPatchDocument<DTO.ExpenseGroup> patchDoc = new JsonPatchDocument<DTO.ExpenseGroup>();
                patchDoc.Replace(eg => eg.Title, expenseGroup.Title);
                patchDoc.Replace(eg => eg.Description, expenseGroup.Description);

                // serialize and PATCH
                var serializedItemToUpdate = JsonConvert.SerializeObject(patchDoc);

                var response = await client.PatchAsync("api/expensegroups/" + id,
                    new StringContent(serializedItemToUpdate,
                    System.Text.Encoding.Unicode, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("An error occurred");
                }

            }
            catch
            {
                return Content("An error occurred");
            }
        }

         

        // POST: ExpenseGroups/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var client = ExpenseTrackerHttpClient.GetClient();

                var response = await client.DeleteAsync("api/expensegroups/" + id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("An error occurred");
                }

            }
            catch
            {
                return Content("An error occurred");
            }
        }


    }
}
