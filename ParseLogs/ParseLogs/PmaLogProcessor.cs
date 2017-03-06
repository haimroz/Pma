using System;
using System.Collections.Generic;
using System.IO;
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

        public List<PmaTimstampData> ProcessLogs()
        {
            List<PmaTimstampData> pmaData = new List<PmaTimstampData>();

            PmaLogParser logParser = new PmaLogParser();

            List<PmaRawEntity> protectedRawList = logParser.Parse(m_bundleInfo.ProtectedVraFilePath);
            List<PmaRawEntity> recoveryRawList = logParser.Parse(m_bundleInfo.RecoveryVraFilePath);
            PmaRawEntitiesInterpulator interpolator = new PmaRawEntitiesInterpulator();
            List<PmaRawEntity> protectedPmaRawEntities = interpolator.BuildInterpolatedList(protectedRawList);
            List<PmaRawEntity> recoveryPmaRawEntities = interpolator.BuildInterpolatedList(recoveryRawList);
            List<PmaRawEntity> mergedPmaRawEntities = interpolator.MergeLists(protectedPmaRawEntities, recoveryPmaRawEntities);

            for (int i = 0; i < mergedPmaRawEntities.Count; i++)
            {
                PmaRawEntity pmaRawEntity = mergedPmaRawEntities[i];
                pmaData.Add(PopulatePmaData(pmaRawEntity, i+1));
            }

            //TODO: Nathaniel Go Over PmaRepository.GetFilteredData2 and copy puplation logic and return PmaData
            //SendDataToFile(logParser.GetPmaHeaders(), mergedPmaRawEntities);
            //SendDataToServerInChuncks(mergedPmaRawEntities);
            return pmaData;
        }

        private static PmaTimstampData PopulatePmaData(PmaRawEntity pmaRawEntity, int index)
        {
            PmaTimstampData pmaTimstampData = new PmaTimstampData();

            pmaTimstampData.Index = index;
            pmaTimstampData.IsValid = 1;
            pmaTimstampData.TimeStamp = pmaRawEntity.TimeStamp;

            pmaTimstampData.PmaRawFieldList = new List<PmaRawFieldData>();

            //pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.ProtectedIOsInDriverMBs, "ProtectedIOsInDriverMBs"));// new field nir should support
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.ProtectedVolumeWriteRateMBs, "ProtectedVolumeWriteRateMbs"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.ProtectedVolumeCompressedWriteRateMBs, "ProtectedVolumeCompressedWriteRateMBs"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.ProtectedCpuPerc, "ProtectedCpuPerc"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.ProtectedVraBufferUsagePerc, "ProtectedVraBufferUsagePerc"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.ProtectedTcpBufferUsagePerc, "ProtectedTcpBufferUsagePerc"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.NetworkOutgoingRateMBs, "NetworkOutgoingRateMBs"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.RecoveryTcpBufferUsagePerc, "RecoveryTcpBufferUsagePerc"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.RecoveryCpuPerc, "RecoveryCpuPerc"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.RecoveryVraBufferUsagePerc, "RecoveryVraBufferUsagePerc"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.HardeningRateMBs, "HardeningRateMBs"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.JournalSizeMB, "JournalSizeMB"));
            pmaTimstampData.PmaRawFieldList.Add(ConstructFieldData(pmaRawEntity.ApplyRateMBs, "ApplyRateMBs"));
            pmaTimstampData.ComputeValidity();
            return pmaTimstampData;
        }

        private static PmaRawFieldData ConstructFieldData(object value, string fieldName)
        {
            int threshold = 1000;//make the threshold ineffective

            if (value.GetType() == typeof(double))
                if ((double)value <= 0)
                    value = 0.0;
            if (value.GetType() == typeof(int))
                if ((int)value <= 0)
                    value = 0;

            int isValid = 0;

            if (fieldName.Equals("ProtectedCpuPerc"))
                threshold = 85;
            if (fieldName.Equals("ProtectedVraBufferUsagePerc"))
                threshold = 80;
            if (fieldName.Equals("ProtectedTcpBufferUsagePerc"))
                threshold = 90;
            if (fieldName.Equals("RecoveryTcpBufferUsagePerc"))
                threshold = 80;
            if (fieldName.Equals("RecoveryCpuPerc"))
                threshold = 70;
            if (fieldName.Equals("RecoveryVraBufferUsagePerc"))
                threshold = 80;
            isValid = Convert.ToInt32(value) < threshold ? 1 : 0;

            return new PmaRawFieldData(fieldName, value.ToString(), threshold.ToString(), isValid);
        }

        public void ProcessLogsAndStoreIntoDb()
        {
            //TODO: Yaniv - calculate the requested fileds from m_bundleInfo
            PmaLogParser logParser = new PmaLogParser();
            
            List<PmaRawEntity> protectedRawList = logParser.Parse(m_bundleInfo.ProtectedVraFilePath);
            List<PmaRawEntity> recoveryRawList = logParser.Parse(m_bundleInfo.RecoveryVraFilePath);
            PmaRawEntitiesInterpulator interpolator = new PmaRawEntitiesInterpulator();
            List<PmaRawEntity> protectedPmaRawEntities = interpolator.BuildInterpolatedList(protectedRawList);
            List<PmaRawEntity> recoveryPmaRawEntities = interpolator.BuildInterpolatedList(recoveryRawList);

            List<PmaRawEntity> mergedPmaRawEntities = interpolator.MergeLists(protectedPmaRawEntities, recoveryPmaRawEntities);

            //SendDataToFile(logParser.GetPmaHeaders(), mergedPmaRawEntities);
            SendDataToServerInChuncks(mergedPmaRawEntities);
        }

        private void SendDataToFile(List<string> headers, List<PmaRawEntity> pmaEntities)
        {
            using (var streamWriter = new StreamWriter("C:\\pma\\output.csv"))
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
                //SendDataToLocalServer(pmaEntities.GetRange(startInd, Math.Min(pmaEntities.Count - startInd, ChunckSize)));

                startInd += ChunckSize;
            }
        }

        private void SendDataToServer(List<PmaRawEntity> pmaEntities)
        {
            PmaRepository pmaRepository = new PmaRepository();
            pmaRepository.SetData(m_bundleInfo, pmaEntities);
        }

        private void SendDataToLocalServer(List<PmaRawEntity> pmaEntities)
        {
            PmaLocalRepository pmalocalRepository = PmaLocalRepository.GetInstance();
            //pmalocalRepository.SetData(m_bundleInfo, pmaEntities);
        }
    }
}
