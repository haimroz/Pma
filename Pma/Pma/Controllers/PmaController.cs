using System;
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
        private readonly PmaRepository m_pmaRepository;

        public PmaController()
        {
            m_pmaRepository = new PmaRepository();
        }

        // GET api/Pma/GetPmaData?returnUrl=%2F&generateState=true
        public PmaRawEntity[] Get()
        {
            return m_pmaRepository.GetFilteredData(DateTime.MinValue, DateTime.MaxValue);
            //return m_pmaRepository.GetAll();
        }

        public HttpResponseMessage Post(List<PmaRawEntity> pmaList)
        {
            m_pmaRepository.SetData(pmaList);

            var response = Request.CreateResponse(System.Net.HttpStatusCode.Created, pmaList);

            return response;
        }
    }
}
