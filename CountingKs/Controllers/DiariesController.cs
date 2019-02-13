using CountingKs.Data;
using CountingKs.Filters;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace CountingKs.Controllers
{
  
    public class DiariesController : BaseApiController
    {
        ICountingKsIdentityService _identityService;

        public DiariesController(ICountingKsRepository repo,
                                 ICountingKsIdentityService identityService):base(repo)
        {
            _identityService = identityService;
        }

        public IEnumerable<DiaryModel> Get()
        {
            var username = _identityService.CurrentUser;
            var results = TheRepo.GetDiaries(username)
                                 .OrderByDescending(d => d.CurrentDate)
                                 .Take(10)
                                 .ToList()
                                 .Select(d=>TheModelFactory.Create(d));
            return results;
        }

        public HttpResponseMessage Get(DateTime diaryid)
        {
            var username = _identityService.CurrentUser;
            var result = TheRepo.GetDiary(username, diaryid);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(result));
                
        }
    }
}