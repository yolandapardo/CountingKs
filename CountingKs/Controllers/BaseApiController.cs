using CountingKs.ActionResults;
using CountingKs.Data;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CountingKs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        ModelFactory _modelFactory;
        ICountingKsRepository _repo;

        public BaseApiController(ICountingKsRepository repo)
        {
            _repo = repo;
        }

        protected ICountingKsRepository TheRepo { get
            {
                return _repo;
            }
        }

        protected ModelFactory TheModelFactory
        {
            get {
                if (_modelFactory == null)
                    _modelFactory = new ModelFactory(this.Request, TheRepo);
                return _modelFactory;
            }
        }

        protected IHttpActionResult Versioned<T>(T body, string version="V1") where T : class
        {
            return new VersionedActionResult<T>(this.Request,version,body);
        }

    }
}