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
    public class MeasuresV2Controller : BaseApiController
    {
        

        public MeasuresV2Controller(ICountingKsRepository repo):base(repo)
        {
           

        }

        public IEnumerable<MeasureV2Model> Get(int foodid)
        {
            var results = TheRepo.GetMeasuresForFood(foodid)
                .ToList()
                .Select(m=>TheModelFactory.Create2(m));
            return results;

        }
        public MeasureV2Model Get(int foodid,int id)
        {
            var results = TheRepo.GetMeasure(id);
            if (results.Food.Id == foodid)
                return TheModelFactory.Create2(results);
            return null;
        }
    }
}