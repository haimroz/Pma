﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using PmaEntities;
using System.Globalization;

namespace ParseLogs
{
    public class PmaLogParser
    {
        private const string TimeStampHeader = "Time";
        private const string ProtectedIOsInDriverMBsHeader = "Protected IOs in driver (MBps)";
        private const string ProtectedVolumeWriteRateMBsHeader = "Write IOs rate (MBps) (zvm)";
        private const string ProtectedVolumeCompressedWriteRateMBsHeader = "Write IOs compressed rate (MBps) (zvm)";
        private const string VraCpuPercHeader = "Vra CPU";
        private const string ProtectedVraBufferUsagePercHeader = "Local buf usage (%)";
        private const string ProtectedTcpBufferUsageMBsHeader = "OS net TX queue (MB)";
        private const string NetworkOutgoingRateMBsHeader = "Network outgoing rate (MBps) (zvm)";
        private const string RecoveryTcpBufferUsageMBsHeader = "OS net RX queue (MB)";
        private const string RecoveryVraBufferUsagePercHeader = "Remote buf usage (%)";
        private const string HardeningRateMBsHeader = "Harden rate (MBps) (zvm)";
        private const string ApplyRateMBsHeader = "Apply rate (MBps) (zvm)";
        private const double MaxTcpBufferUsageMBs = 8;

        public List<PmaRawEntity> Parse(string logFileName)
        {
            JObject protectedLogJObject = JObject.Parse(File.ReadAllText(logFileName));
            List<string> headersList = (List<string>)protectedLogJObject["headers"].ToObject(typeof(List<string>));
            Dictionary<string, int> headers = BuildHeadersDictionary(headersList);

            List<PmaRawEntity> pmaEntities = BuildPmaEntities(protectedLogJObject, headers);

            return pmaEntities;
        }

        private Dictionary<string, int> BuildHeadersDictionary(List<string> headers)
        {
            Dictionary<string, int> headersDictionary = new Dictionary<string, int>(headers.Count);

            int index = 0;
            foreach (string header in headers)
            {
                headersDictionary.Add(header, index++);
            }
            return headersDictionary;
        }

        private List<PmaRawEntity> BuildPmaEntities(JObject logJObject, Dictionary<string, int> headers)
        {
            List<PmaRawEntity> pmaRawEntities = new List<PmaRawEntity>();
            for (int dataIndex = 0; dataIndex < logJObject["data"].Count(); dataIndex++)
            {
                List<string> pmaLogData = (List<string>)logJObject["data"][dataIndex].ToObject(typeof(List<string>));
                pmaRawEntities.Add(BuildPmaEntity(pmaLogData.ToArray(), headers));
            }

            return pmaRawEntities;
        }

        private PmaRawEntity BuildPmaEntity(string[] pmaLogData, Dictionary<string, int> headers)
        {
            PmaRawEntity pmaRawEntity = new PmaRawEntity
            {
                TimeStamp = Convert.ToDateTime(pmaLogData[headers[TimeStampHeader]], CultureInfo.InvariantCulture),
                ProtectedIOsInDriverMBs = Convert.ToDouble(pmaLogData[headers[ProtectedIOsInDriverMBsHeader]]),
                ProtectedVolumeWriteRateMBs = Convert.ToDouble(pmaLogData[headers[ProtectedVolumeWriteRateMBsHeader]]),
                ProtectedVolumeCompressedWriteRateMBs = Convert.ToDouble(pmaLogData[headers[ProtectedVolumeCompressedWriteRateMBsHeader]]),
                ProtectedCpuPerc = Convert.ToInt32(Convert.ToDouble(pmaLogData[headers[VraCpuPercHeader]])),
                ProtectedVraBufferUsagePerc = Convert.ToInt32(Convert.ToDouble(pmaLogData[headers[ProtectedVraBufferUsagePercHeader]])),
                ProtectedTcpBufferUsagePerc = ConvertTcpBufferUsageMBsToPerc(Convert.ToDouble(pmaLogData[headers[ProtectedTcpBufferUsageMBsHeader]])),
                NetworkOutgoingRateMBs = Convert.ToDouble(pmaLogData[headers[NetworkOutgoingRateMBsHeader]]),
                RecoveryTcpBufferUsagePerc = ConvertTcpBufferUsageMBsToPerc(Convert.ToDouble(pmaLogData[headers[RecoveryTcpBufferUsageMBsHeader]])),
                RecoveryCpuPerc = Convert.ToInt32(Convert.ToDouble(pmaLogData[headers[VraCpuPercHeader]])),
                RecoveryVraBufferUsagePerc = Convert.ToInt32(Convert.ToDouble(pmaLogData[headers[RecoveryVraBufferUsagePercHeader]])),
                HardeningRateMBs = 512 * Convert.ToDouble(pmaLogData[headers[HardeningRateMBsHeader]]),//compensate for zvm/logparser bug that treats the LBs as if were bytes
                ApplyRateMBs = Convert.ToDouble(pmaLogData[headers[ApplyRateMBsHeader]])
            };
            return pmaRawEntity;
        }

        private int ConvertTcpBufferUsageMBsToPerc(double tcpBufferUsageMBs)
        {
            if (tcpBufferUsageMBs > 0)
            return Convert.ToInt32(tcpBufferUsageMBs*100.0/MaxTcpBufferUsageMBs);
            else
                return Convert.ToInt32(tcpBufferUsageMBs);
        }

        public List<string> GetPmaHeaders()
        {
            List<string> headers = new List<string>();
            headers.Add(TimeStampHeader);
            headers.Add(ProtectedIOsInDriverMBsHeader);
            headers.Add(ProtectedVolumeWriteRateMBsHeader);
            headers.Add(ProtectedVolumeCompressedWriteRateMBsHeader);
            headers.Add("Protected " + VraCpuPercHeader);
            headers.Add(ProtectedVraBufferUsagePercHeader);
            headers.Add(ProtectedTcpBufferUsageMBsHeader);
            headers.Add(NetworkOutgoingRateMBsHeader);
            headers.Add(RecoveryTcpBufferUsageMBsHeader);
            headers.Add("Recovery " + VraCpuPercHeader);
            headers.Add(RecoveryVraBufferUsagePercHeader);
            headers.Add(HardeningRateMBsHeader);
            headers.Add(ApplyRateMBsHeader);

            return headers;
        }
    }
}
