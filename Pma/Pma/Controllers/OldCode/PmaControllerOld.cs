using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using PmaEntities;
using Ppa.Services;
using Ppa.Services.OldCode;

namespace Ppa.Controllers.OldCode
{
    [EnableCors(origins: "http://localhost:8000", headers: " *", methods: "*")]
    public class PmaControllerOld : ApiController
    {
        private readonly PmaRepositoryOld m_pmaRepository;

        public PmaControllerOld()
        {
            m_pmaRepository = new PmaRepositoryOld();
        }

        // GET api/Pma/GetPmaData?returnUrl=%2F&generateState=true
        //        public PmaRawEntity[] GetOldOld()
        //        {
        //            return m_pmaRepository.GetFilteredData(DateTime.MinValue, DateTime.MaxValue);
        //            //return m_pmaRepository.GetAll();
        //        }

        //        public PmaRawEntityWithLimit[] GetOld()
        //        {
        //            return m_pmaRepository.GetFilteredData1(DateTime.MinValue, DateTime.MaxValue);
        //        }



        // Define new API named: Parse
        // Input parameters: 1. Direcory (Path) 2. Protected Vra Name 3. Recovery Vra name
        // Output: 1. parsed raw counts 2. from (minimum parse date) 3. to (maximum parse date) 4. handle to parsed filename/table name ...


        // We can add input parameter to get which consist: handle unique name
        public PmaTimstampData[] Get()
        {
            PmaTimstampData[] pmaData = m_pmaRepository.GetFilteredData2(DateTime.MinValue, DateTime.MaxValue);
            SetRangeOfInvalidDueToNetworkingIssue(pmaData);
            return pmaData;
        }

        
        public HttpResponseMessage Post(List<PmaRawEntity> pmaList)
        {
            m_pmaRepository.SetData(pmaList);
            var response = Request.CreateResponse(System.Net.HttpStatusCode.Created, pmaList);
            return response;
        }



        //        public PmaTimstampData[] Get([FromUri] string from, [FromUri] string to)
        //        {
        //            var fromDateTime = DateTime.Parse(@from);
        //            var toDateTime = DateTime.Parse(to);
        //            PmaTimstampData[] pmaData = m_pmaRepository.GetFilteredData2(fromDateTime, toDateTime);
        //            SetRangeOfInvalidDueToNetworkingIssue(pmaData);
        //            return pmaData;
        //        }

        private static void SetRangeOfInvalidDueToNetworkingIssue(PmaTimstampData[] pmaData)
        {
            int startOfInvalid = pmaData.Length/3;
            for (int i = 0; i < pmaData.Length; i++)
            {
                InvalidateValueIfBuffersAreAboveThreshold(pmaData, i);
                InvalidateValueIfNetworkBuffersAreAboveThreshold(pmaData, i);
            }
        }

        private static void InvalidateValueIfBuffersAreAboveThreshold(PmaTimstampData[] pmaData, int i)
        {
            PmaTimstampData pmaTimstampData = pmaData[i];
            string sProtectedVraBufferUsagePerc = pmaTimstampData.PmaRawFieldList[3].Value;
            int protectedVraBufferUsagePerc;
            int.TryParse(sProtectedVraBufferUsagePerc, out protectedVraBufferUsagePerc);

            string sProtectedVraBufferUsagePercThreshold = pmaTimstampData.PmaRawFieldList[3].Threshold;
            int protectedVraBufferUsagePercThreshold;
            int.TryParse(sProtectedVraBufferUsagePercThreshold, out protectedVraBufferUsagePercThreshold);

            string sRecoveryVraBufferUsagePerc = pmaTimstampData.PmaRawFieldList[8].Value;
            int recoveryVraBufferUsagePerc;
            int.TryParse(sRecoveryVraBufferUsagePerc, out recoveryVraBufferUsagePerc);

            string sRecoveryVraBufferUsagePercThreshold = pmaTimstampData.PmaRawFieldList[8].Threshold;
            int recoveryVraBufferUsagePercThreshold;
            int.TryParse(sRecoveryVraBufferUsagePercThreshold, out recoveryVraBufferUsagePercThreshold);

            if (protectedVraBufferUsagePerc > protectedVraBufferUsagePercThreshold ||
                recoveryVraBufferUsagePerc > recoveryVraBufferUsagePercThreshold)
                pmaTimstampData.IsValid = 0;
        }

        private static void InvalidateValueIfNetworkBuffersAreAboveThreshold(PmaTimstampData[] pmaData, int i)
        {
            PmaTimstampData pmaTimstampData = pmaData[i];
            string sProtectedTcpBufferUsagePerc = pmaTimstampData.PmaRawFieldList[4].Value;
            int protectedTcpBufferUsagePerc;
            int.TryParse(sProtectedTcpBufferUsagePerc, out protectedTcpBufferUsagePerc);

            string sRecoveryTcpBufferUsagePerc = pmaTimstampData.PmaRawFieldList[6].Value;
            int recoveryTcpBufferUsagePerc;
            int.TryParse(sRecoveryTcpBufferUsagePerc, out recoveryTcpBufferUsagePerc);

            if (protectedTcpBufferUsagePerc > 80 || recoveryTcpBufferUsagePerc > 80)
                pmaTimstampData.IsValid = 0;
        }

        
    }
}
