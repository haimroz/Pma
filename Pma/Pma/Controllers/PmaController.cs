using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using ParseLogs;
using PmaEntities;
using Repository;

namespace Ppa.Controllers
{
    [EnableCors(origins: "http://localhost:8000", headers: " *", methods: "*")]
    public class PmaController : ApiController
    {
        private readonly PmaRepository m_pmaRepository;

        public PmaController()
        {
            m_pmaRepository = new PmaRepository();
        }

        // Define new API named: Parse
        // Input parameters: 1. Direcory (Path) 2. Protected Vra Name 3. Recovery Vra name
        // Output: 1. parsed raw counts 2. from (minimum parse date) 3. to (maximum parse date) 4. handle to parsed filename/table name ...


        // We can add input parameter to get which consist: handle unique name
        //        public PmaTimstampData[] GetOld()
        //        {
        //            PmaTimstampData[] pmaData = m_pmaRepository.GetFilteredData2(DateTime.MinValue, DateTime.MaxValue);
        //            SetRangeOfInvalidDueToNetworkingIssue(pmaData);
        //            return pmaData;
        //        }

        public PmaTimstampData[] GetPmaData([FromUri] string protectedVraFilePath, [FromUri] string recoveryVraFilePath)
        {
            if (!File.Exists(protectedVraFilePath))
            {
                throw new Exception($"Protected VRA file paht doesn't exist: {protectedVraFilePath}");
            }
            if (!File.Exists(recoveryVraFilePath))
            {
                throw new Exception($"Recovery VRA file path doesn't exist: {recoveryVraFilePath}");
            }

            RequestedBundleInfo bundleInfo = new RequestedBundleInfo(protectedVraFilePath, recoveryVraFilePath);
            List<PmaTimstampData> result = ParseBundleLogs(bundleInfo);
            return result.ToArray();
            //return result.Take(5000).ToArray();

//            PmaTimstampData[] pmaData = m_pmaRepository.GetFilteredData2(DateTime.MinValue, DateTime.MaxValue);
//            SetRangeOfInvalidDueToNetworkingIssue(pmaData);
//            return pmaData;
        }

        private List<PmaTimstampData> ParseBundleLogs(RequestedBundleInfo bundleInfo)
        {
            PmaLogProcessor processor = new PmaLogProcessor(bundleInfo);
            return processor.ProcessLogs();

//            PmaTimstampData[] pmaData = m_pmaRepository.GetFilteredData2(DateTime.MinValue, DateTime.MaxValue);
//            SetRangeOfInvalidDueToNetworkingIssue(pmaData);
//            return pmaData;
        }

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

        //        public PmaTimstampData[] Get([FromUri] string from, [FromUri] string to)
        //        {
        //            var fromDateTime = DateTime.Parse(@from);
        //            var toDateTime = DateTime.Parse(to);
        //            PmaTimstampData[] pmaData = m_pmaRepository.GetFilteredData2(fromDateTime, toDateTime);
        //            SetRangeOfInvalidDueToNetworkingIssue(pmaData);
        //            return pmaData;
        //        }
    }
}
