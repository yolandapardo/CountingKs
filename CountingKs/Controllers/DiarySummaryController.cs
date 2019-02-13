using CountingKs.Data;
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
    public class DiarySummaryController : BaseApiController
    {
        ICountingKsIdentityService _identityService;

        public DiarySummaryController(ICountingKsRepository repo,
                                 ICountingKsIdentityService identityService):base(repo)
        {
            _identityService = identityService;
        }

        public object Get(DateTime diaryid)
        {
            try
            {
                var diary = TheRepo.GetDiary(_identityService.CurrentUser, diaryid);
                if(diaryid==null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                return TheModelFactory.CreateSummary(diary);
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}