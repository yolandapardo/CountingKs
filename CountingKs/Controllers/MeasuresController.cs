using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CountingKs.Controllers
{
    public class MeasuresController : BaseApiController
    {
        

        public MeasuresController(ICountingKsRepository repo):base(repo)
        {
           

        }

        public IEnumerable<MeasureModel> Get(int foodid)
        {
            var results = TheRepo.GetMeasuresForFood(foodid)
                .ToList()
                .Select(m=>TheModelFactory.Create(m));
            return results;

        }
        public MeasureModel Get(int foodid,int id)
        {
            var results = TheRepo.GetMeasure(id);
            if (results.Food.Id == foodid)
                return TheModelFactory.Create(results);
            return null;
        }
    }
}