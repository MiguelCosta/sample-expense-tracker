﻿using ExpenseTracker.DTO;
using ExpenseTracker.WebClient.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace ExpenseTracker.WebClient.Controllers
{
    public class ExpensesController : Controller
    {
 

        // GET: Expenses/Create
        public ActionResult Create(int expenseGroupId)
        {
            return View();
        }

        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Expense expense)
        {
            try
            {
                var client = ExpenseTrackerHttpClient.GetClient();


                // serialize & POST
                var serializedItemToCreate = JsonConvert.SerializeObject(expense);

                var response = await client.PostAsync("api/expenses",
                    new StringContent(serializedItemToCreate,
                    System.Text.Encoding.Unicode, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", "ExpenseGroups", new { id = expense.ExpenseGroupId });
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
 


        // GET: Expenses/Edit/5
        public async Task<ActionResult> Edit(int id)
        { 
            var client = ExpenseTrackerHttpClient.GetClient();
            
            HttpResponseMessage response = await client.GetAsync("api/expenses/" + id);

            

            if (response.IsSuccessStatusCode)
            {
		string content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<Expense>(content);
                return View(model);
            }

            return Content("An error occurred.");
 
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Expense expense)
        {
            try
            {
                var client = ExpenseTrackerHttpClient.GetClient();

                // serialize & PUT
                var serializedItemToUpdate = JsonConvert.SerializeObject(expense);

                var response = await client.PutAsync("api/expenses/" + id,
                    new StringContent(serializedItemToUpdate,
                    System.Text.Encoding.Unicode, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", "ExpenseGroups", new { id = expense.ExpenseGroupId });
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

 
        // GET: Expenses/Delete/5
        public async Task<ActionResult> Delete(int expenseGroupId, int id)
        {
            try
            {
                var client = ExpenseTrackerHttpClient.GetClient();
 
                var response = await client.DeleteAsync("api/expenses/" + id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", "ExpenseGroups", new { id = expenseGroupId });
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
