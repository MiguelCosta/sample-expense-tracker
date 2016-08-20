using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;
using Marvin.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ExpenseTracker.API.Helpers;

namespace ExpenseTracker.API.Controllers
{

 
    public class ExpenseGroupsController : ApiController
    {
         
        IExpenseTrackerRepository _repository;
        ExpenseGroupFactory _expenseGroupFactory = new ExpenseGroupFactory();

        const int maxPageSize = 10;

        public ExpenseGroupsController()
        {
            _repository = new ExpenseTrackerEFRepository(new Repository.Entities.ExpenseTrackerContext());
        }    

        public ExpenseGroupsController(IExpenseTrackerRepository repository)
        {
            _repository = repository;
        }



        public IHttpActionResult Get(string sort = "id")
        {
            try
            {
                // get expensegroups from repository
                var expenseGroups = _repository.GetExpenseGroups();

                // return them after mapping them to DTO's, with statuscode 200
                 return Ok(expenseGroups 
                    .ApplySort(sort)
                    .ToList()
                    .Select(eg => _expenseGroupFactory.CreateExpenseGroup(eg)));
                 
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        public IHttpActionResult Get(int id)
        {
            try
            { 
                var expenseGroup = _repository.GetExpenseGroup(id);
 
                if (expenseGroup != null)
                {
                    return Ok(_expenseGroupFactory.CreateExpenseGroup(expenseGroup));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


  
        [HttpPost]
        public IHttpActionResult Post([FromBody]DTO.ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                {
                    return BadRequest();
                }
                                
                // try mapping & saving
                var eg = _expenseGroupFactory.CreateExpenseGroup(expenseGroup);

                var result = _repository.InsertExpenseGroup(eg);
                if (result.Status == RepositoryActionStatus.Created)
                {
                    // map to dto
                    var newExpenseGroup = _expenseGroupFactory.CreateExpenseGroup(result.Entity);
                    return Created<DTO.ExpenseGroup>(Request.RequestUri
                        + "/" + newExpenseGroup.Id.ToString(), newExpenseGroup);
                }

                return BadRequest();

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }



        public IHttpActionResult Put(int id, [FromBody]DTO.ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                    return BadRequest();

                // map
                var eg = _expenseGroupFactory.CreateExpenseGroup(expenseGroup);

                var result = _repository.UpdateExpenseGroup(eg);
                if (result.Status == RepositoryActionStatus.Updated)
                {
                    // map to dto
                    var updatedExpenseGroup = _expenseGroupFactory
                        .CreateExpenseGroup(result.Entity);
                    return Ok(updatedExpenseGroup);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }



        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody]JsonPatchDocument<DTO.ExpenseGroup> expenseGroupPatchDocument)
        {
            try
            {               
                if (expenseGroupPatchDocument == null)
                {
                    return BadRequest();
                } 

                var expenseGroup = _repository.GetExpenseGroup(id);
                if (expenseGroup == null)
                {
                    return NotFound();
                }
                
                // map
                var eg = _expenseGroupFactory.CreateExpenseGroup(expenseGroup);

                // apply changes to the DTO
                expenseGroupPatchDocument.ApplyTo(eg);

                // map the DTO with applied changes to the entity, & update
                var result = _repository.UpdateExpenseGroup(_expenseGroupFactory.CreateExpenseGroup(eg));

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    // map to dto
                    var patchedExpenseGroup = _expenseGroupFactory.CreateExpenseGroup(result.Entity);
                    return Ok(patchedExpenseGroup);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }



        public IHttpActionResult Delete(int id)
        {
            try
            {
                            
                var result = _repository.DeleteExpenseGroup(id);

                if (result.Status == RepositoryActionStatus.Deleted)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


    }
}
