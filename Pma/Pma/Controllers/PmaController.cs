using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using PmaEntities;
using Ppa.Models;
using Ppa.Services;

namespace Ppa.Controllers
{
    public class PmaController : ApiController
    {
        private readonly PmaQueryRepository _mPmaQueryRepository;

        public PmaController()
        {
            _mPmaQueryRepository = new PmaQueryRepository();
        }

        public PmaRawEntity[] Get()
        {
            return _mPmaQueryRepository.GetAll();
        }

        public HttpResponseMessage Post(List<PmaRawEntity> pmaList)
        {
            _mPmaQueryRepository.SetData(pmaList);

            var response = Request.CreateResponse(System.Net.HttpStatusCode.Created, pmaList);

            return response;
        }
    }
}
