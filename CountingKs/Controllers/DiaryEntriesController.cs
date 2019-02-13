using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CountingKs.Controllers
{
    public class DiaryEntriesController : BaseApiController
    {
        private ICountingKsIdentityService _identityService;

        public DiaryEntriesController(ICountingKsRepository repo, ICountingKsIdentityService identityService): base(repo)
        {
            _identityService = identityService;
        }

        public IEnumerable<DiaryEntryModel> Get(DateTime diaryid)
        {
            var results = TheRepo.GetDiaryEntries(_identityService.CurrentUser, diaryid.Date)
                                .ToList()
                                .Select(e => TheModelFactory.Create(e));
            return results;
        }

        public HttpResponseMessage Get(DateTime diaryid, int id)
        {
            var result = TheRepo.GetDiaryEntry(_identityService.CurrentUser, diaryid.Date, id);
            if (result == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(result));
        }

       public HttpResponseMessage Post(DateTime diaryid, [FromBody]DiaryEntryModel model)
        {
            try
            {
                var entity = TheModelFactory.Parse(model);
                if (entity == null)
                    Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read diary entry in body");
                var diary = TheRepo.GetDiary(_identityService.CurrentUser, diaryid);
                if (diary == null)
                    Request.CreateResponse(HttpStatusCode.NotFound);

                //Make sure it's not duplicate
                if (diary.Entries.Any(e => e.Measure.Id == entity.Measure.Id))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duplicate Measure not allowed");

                //Save the new Entry
                diary.Entries.Add(entity);
                if (TheRepo.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, TheModelFactory.Create(entity));
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save in the database");

               
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        public HttpResponseMessage Delete(DateTime diaryid, int id)
        {
            try
            {
                if (TheRepo.GetDiaryEntries(_identityService.CurrentUser, diaryid).Any(e => e.Id == id) == false)
                {
                  return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (TheRepo.DeleteDiaryEntry(id) && TheRepo.SaveAll())
                {
                 return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                  return Request.CreateResponse(HttpStatusCode.BadRequest);

            }
            catch(Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save in the database");
            }
        }

        public HttpResponseMessage Patch(DateTime diaryid, int id, [FromBody]DiaryEntryModel model) 
        {
            try
            {
                var entity = TheRepo.GetDiaryEntry(_identityService.CurrentUser, diaryid, id);
                if (entity == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                var parsedValue = TheModelFactory.Parse(model);
                if (parsedValue == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                if (entity.Quantity != parsedValue.Quantity)
                {
                    entity.Quantity = parsedValue.Quantity;
                    if(TheRepo.SaveAll())
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                   
                }
                
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}