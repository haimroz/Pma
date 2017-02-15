using System;
using System.Collections.Generic;
using PmaEntities;
using Repository;

namespace ParseLogs
{
    public class PmaLogProcessor
    {
        private const int ChunckSize = 5000;
        private readonly RequestedBundleInfo m_bundleInfo;

        public PmaLogProcessor(RequestedBundleInfo bundleInfo)
        {
            m_bundleInfo = bundleInfo;
        }
        public void ProcessLogs()
        {
            //TODO: Yaniv - calculate the requested fileds from m_bundleInfo
            string protectedLogFileName = string.Empty;
            string recoveryLogFileName = string.Empty;
            PmaLogParser logParser = new PmaLogParser();
            
            List<PmaRawEntity> protectedRawList = logParser.Parse(protectedLogFileName);
            List<PmaRawEntity> recoveryRawList = logParser.Parse(recoveryLogFileName);
            PmaRawEntitiesInterpulator interpolator = new PmaRawEntitiesInterpulator();
            List<PmaRawEntity> protectedPmaRawEntities = interpolator.ProcessRawList(protectedRawList);
            List<PmaRawEntity> recoveryPmaRawEntities = interpolator.ProcessRawList(recoveryRawList);

            List<PmaRawEntity> mergedPmaRawEntities = interpolator.MergeLists(protectedPmaRawEntities, recoveryPmaRawEntities);

            SendDataToFile(logParser.GetPmaHeaders(), mergedPmaRawEntities);
          //  SendDataToServerInChuncks(mergedPmaRawEntities);
        }

        private void SendDataToFile(List<string> headers, List<PmaRawEntity> pmaEntities)
        {
            using (var streamWriter = new StreamWriter("C:\\pma\\output.txt"))
            {
                streamWriter.WriteLine(string.Join(",", headers.ToArray()));
                foreach (PmaRawEntity pmaEntity in pmaEntities)
                {
                    streamWriter.WriteLine(pmaEntity.ToString());
                }
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        private void SendDataToServerInChuncks(List<PmaRawEntity> pmaEntities)
        {
            int startInd = 0;
            while (startInd < pmaEntities.Count)
            {
                SendDataToServer(pmaEntities.GetRange(startInd, Math.Min(pmaEntities.Count - startInd, ChunckSize)));
                startInd += ChunckSize;
            }
        }

        private void SendDataToServer(List<PmaRawEntity> pmaEntities)
        {
            PmaRepository pmaRepository = new PmaRepository();
            pmaRepository.SetData(m_bundleInfo, pmaEntities);
        }
    }
}
