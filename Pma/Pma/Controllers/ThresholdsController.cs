using System.Collections.Generic;
using System.Web.Http;
using PmaEntities;

namespace Ppa.Controllers
{
    public class ThresholdsController : ApiController
    {

        public ThresholdsController()
        {
        }

        public Dictionary<string, double> Get()
        {
            Dictionary<string, double> thresholds = new Dictionary<string, double>
            {
                {"ProtectedCpuPerc", 0.8},
                {"ProtectedVraBufferUsagePerc", 0.8},
                {"ProtectedTcpBufferUsagePerc", 0.8},
                {"RecoveryTcpBufferUsagePerc", 0.8},
                {"RecoveryCpuPerc", 0.8},
                {"RecoveryVraBufferUsagePerc", 0.8}
            };
            return thresholds;
        }
        
//        public HttpResponseMessage Post(List<PmaRawEntity> pmaList)
//        {
//            m_pmaRepository.SetData(pmaList);
//
//            var response = Request.CreateResponse(System.Net.HttpStatusCode.Created, pmaList);
//
//            return response;
//        }
    }
}
