using System;
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

        public Dictionary<DateTime, Dictionary<string, int>> Get()
        {
            var pmaCtrl = new PmaController();
            PmaRawEntity[] pmaRawEntities = pmaCtrl.Get();
            var res = new Dictionary<DateTime, Dictionary<string, int>>();
            foreach (PmaRawEntity rawEntity in pmaRawEntities)
            {
                Dictionary<string, int> thresholds = new Dictionary<string, int>
                {
                    {"TimeStamp", 0},
                    {"ProtectedVolumeWriteRateMbs", 0},
                    {"ProtectedVolumeCompressedWriteRateMBs", 0},
                    {"ProtectedCpuPerc", rawEntity.ProtectedCpuPerc < 80 ? 0 : 1},
                    {"ProtectedVraBufferUsagePerc", rawEntity.ProtectedVraBufferUsagePerc < 80 ? 0 : 1},
                    {"ProtectedTcpBufferUsagePerc", rawEntity.ProtectedTcpBufferUsagePerc < 80 ? 0 : 1},
                    {"NetworkOutgoingRateMBs", 0},
                    {"RecoveryTcpBufferUsagePerc", rawEntity.RecoveryTcpBufferUsagePerc < 80 ? 0 : 1},
                    {"RecoveryCpuPerc", rawEntity.RecoveryCpuPerc < 80 ? 0 : 1},
                    {"RecoveryVraBufferUsagePerc", rawEntity.RecoveryVraBufferUsagePerc < 80 ? 0 : 1},
                    {"HardeningRateMBs", 0},
                    {"JournalSizeMB", 0},
                    {"ApplyRateMBs", 0}
                };
                if (!res.ContainsKey(rawEntity.TimeStamp))
                    res.Add(rawEntity.TimeStamp, thresholds);
            };
            return res;
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
