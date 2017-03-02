using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        //private readonly PmaRepository m_pmaRepository;
        private static OrderedDictionary _mOrderedCache = _mOrderedCache = new OrderedDictionary(1);

        //private readonly Dictionary<RequestedBundleInfo, PmaInfo> m_cachedPma;
        private static readonly object _mLock = new object();

        

        public PmaController()
        {
            //m_pmaRepository = new PmaRepository();
            //_mOrderedCache = new OrderedDictionary(10);
        }

        public PmaInfo GetPmaData(
            [FromUri] string protectedVraFilePath,
            [FromUri] string recoveryVraFilePath, 
            [FromUri] int pageSize, 
            [FromUri] int pageNumber)
        {
            //int pageNumber = 0;

            Validate(protectedVraFilePath, recoveryVraFilePath, pageSize, pageNumber);

            RequestedBundleInfo bundleInfo = new RequestedBundleInfo(protectedVraFilePath, recoveryVraFilePath);

            lock (_mLock)
            {
                PmaInfo pmaInfo;
                if (_mOrderedCache.Contains(bundleInfo))
                {
                    pmaInfo = (PmaInfo)_mOrderedCache[bundleInfo];
                    return GetRequestedPage(pmaInfo, pageSize, pageNumber - 1);
                }
                _mOrderedCache.Clear();
                List<PmaTimstampData> allPmaData = ParseBundleLogs(bundleInfo);
                pmaInfo = new PmaInfo(allPmaData.Count, allPmaData);
                
                _mOrderedCache.Add(bundleInfo, pmaInfo);
                return GetRequestedPage(pmaInfo, pageSize, pageNumber - 1);
            }
        }

        private void Validate(
            string protectedVraFilePath,
            string recoveryVraFilePath,
            int pageSize,
            int pageNumber)
        {
            if (!File.Exists(protectedVraFilePath))
            {
                throw new Exception($"Protected VRA file paht doesn't exist: {protectedVraFilePath}");
            }
            if (!File.Exists(recoveryVraFilePath))
            {
                throw new Exception($"Recovery VRA file path doesn't exist: {recoveryVraFilePath}");
            }
            if (pageSize < 1 || pageSize > 36000)
            {
                throw new Exception($"PageSize value must be between 1 to 36000. requested page size: {pageSize}");
            }
            if (pageNumber < 1)
            {
                throw new Exception($"pageNumber value must be between 1 to 36000. requested pageNumber: {pageNumber}");
            }
        }

        private PmaInfo GetRequestedPage(PmaInfo pmaInfo, int pageSize, int pageNumber)
        {
            var allPmaData = pmaInfo.PmaData;
            List<PmaTimstampData> fetchedPage =
                allPmaData.Skip(pageNumber*pageSize).Take(pageSize).ToList();

            PmaInfo result = new PmaInfo(pmaInfo.Count, fetchedPage);
            return result;
        }

        private List<PmaTimstampData> ParseBundleLogs(RequestedBundleInfo bundleInfo)
        {
            PmaLogProcessor processor = new PmaLogProcessor(bundleInfo);
            return processor.ProcessLogs();
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
