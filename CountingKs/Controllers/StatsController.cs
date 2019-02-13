using CountingKs.Data;
using Ninject.Activation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    [RoutePrefix("api/stats")]
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repo) : base(repo)
        {

        }

        [Route("")]
        public IHttpActionResult Get()
        {
            var results = new
            {
                NumFoods = TheRepo.GetAllFoods().Count(),
                NumUsers = TheRepo.GetApiUsers().Count()
            };
        return Ok(results);
        }

        [Route("~/api/stat/{id:int}")]
        public IHttpActionResult Get(int id)
        {

            if(id==1)
            {
                return Ok(new { NumFoods = TheRepo.GetAllFoods().Count() });
            }
            if (id == 2)
            {
                return Ok(new { NumUsers = TheRepo.GetApiUsers().Count() });
            }
            return NotFound();
        }

        [Route("~/api/stat/{name:alpha}")]
        public IHttpActionResult Get(string name)
        {

            if (name == "foods")
            {
                return Ok(new { NumFoods = TheRepo.GetAllFoods().Count() });
            }
            if (name == "users")
            {
                return Ok(new { NumUsers = TheRepo.GetApiUsers().Count() });
            }
            return NotFound();
        }
    }
}

