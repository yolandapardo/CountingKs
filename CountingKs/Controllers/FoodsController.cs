using CacheCow.Server.WebApi;
using CountingKs.ActionResults;
using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Filters;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace CountingKs.Controllers
{
    
    public class FoodsController : BaseApiController
    {
        
        public FoodsController(ICountingKsRepository repo):base(repo)
        {
            
        }
        const int PAGE_SIZE= 50;
        [HttpCache(DefaultExpirySeconds = 300)]
        public object Get(bool includeMeasures=true, int page=0)
        {
            IQueryable<Food> query;
            if (includeMeasures)
            {
                query = TheRepo.GetAllFoodsWithMeasures();
            }
            else
                query = TheRepo.GetAllFoods();

            var baseQuery = query.
                OrderBy(f => f.Description);
             
             var totalCount = baseQuery.Count();
            var totalPages = Math.Ceiling((double)totalCount / PAGE_SIZE);
           
           var url = new UrlHelper(Request);
            var prevUrl = page>0?url.Link("Food", new { page = page - 1 }):"";
            var nextUrl = page<totalPages-1?url.Link("Food", new { page = page + 1 }):"";

            var results = baseQuery
                .Skip(PAGE_SIZE*page)
                .Take(PAGE_SIZE)
                .ToList()
                .Select(f=>
                TheModelFactory.Create(f));
              return new {
                  TotalCount= totalCount,
                  TotalPages=totalPages,
                  PrevPageUrl=prevUrl,
                  NextPageUrl=nextUrl,
                  Results=results };

           

        }
        public IHttpActionResult Get(int foodid) 
        {
            return Versioned(TheModelFactory.Create(TheRepo.GetFood(foodid)),"V1");
           // return new VersionedActionResult<FoodModel>(this.Request, "V2", TheModelFactory.Create(TheRepo.GetFood(foodid)));
        }
    }
}
