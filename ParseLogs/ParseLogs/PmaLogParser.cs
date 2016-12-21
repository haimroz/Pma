using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using PmaEntities;

namespace ParseLogs
{
    public class PmaLogParser
    {
        private const string TimeStampHeader = "Time";
        private const string ProtectedVolumeWriteRateMBsHeader = "Write IOs rate (MBps) (zvm)";
        private const string ProtectedVolumeCompressedWriteRateMBsHeader = "Write IOs compressed rate (MBps) (zvm)";
        private const string VraCpuPercHeader = "Vra CPU";
        private const string ProtectedVraBufferUsagePercHeader = "Local buf usage (%)";
        private const string ProtectedTcpBufferUsageMBsHeader = "OS net TX queue (MB)";
        private const string NetworkOutgoingRateMBsHeader = "Network outgoing rate (MBps) (zvm)";
        private const string RecoveryTcpBufferUsageMBsHeader = "OS net RX queue (MB)";
        private const string RecoveryVraBufferUsagePercHeader = "Remote buf usage (%)";
        private const string HardeningRateMBsHeader = "Harden rate (MBps) (zvm)";
        private const string JournalSizeMBHeader = "";
        private const string ApplyRateMBsHeader = "Apply rate (MBps) (zvm)";

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
            return GenerateDummyPmaData(100);

            //List<PmaRawEntity> pmaRawEntities = new List<PmaRawEntity>();
            //for (int dataIndex = 0; dataIndex < logJObject["data"].Count(); dataIndex++)
            //{
            //    List<string> pmaLogData = (List<string>)logJObject["data"][dataIndex].ToObject(typeof(List<string>));

            //}

            //return pmaRawEntities;
        }

        private PmaRawEntity BuildPmaEntity(string[] pmaLogData, Dictionary<string, int> headers)
        {
            PmaRawEntity pmaRawEntity = new PmaRawEntity();
            pmaRawEntity.TimeStamp = Convert.ToDateTime(pmaLogData[headers[TimeStampHeader]]);
         //   pmaRawEntity.
            return pmaRawEntity;
        }




        private List<PmaRawEntity> GenerateDummyPmaData(int numberOfRows)
        {
            DateTime currentDateTime = DateTime.Now.AddDays(-2);
            List<PmaRawEntity> pmaRawEntity = new List<PmaRawEntity>();
            for (int i = 0; i < numberOfRows; i++)
            {
                currentDateTime = currentDateTime.AddSeconds(1);
                pmaRawEntity.Add(CreatePmaRow(currentDateTime, i));
            }
            return pmaRawEntity;
        }

        private PmaRawEntity CreatePmaRow(DateTime dateTime, int index)
        {
            Random random = new Random(index);

            PmaRawEntity pmaRawEntity = new PmaRawEntity
            {
                TimeStamp = dateTime,
                ProtectedVolumeWriteRateMbs = Math.Abs(random.NextDouble()),
                ProtectedVolumeCompressedWriteRateMBs = Math.Abs(random.NextDouble()),
                ProtectedCpuPerc = random.Next(0, 100),
                ProtectedVraBufferUsagePerc = random.Next(0, 100),
                ProtectedTcpBufferUsagePerc = random.Next(0, 100),
                NetworkOutgoingRateMBs = Math.Abs(random.NextDouble()),
                RecoveryTcpBufferUsagePerc = random.Next(0, 100),
                RecoveryCpuPerc = random.Next(0, 100),
                RecoveryVraBufferUsagePerc = random.Next(0, 100),
                HardeningRateMBs = Math.Abs(random.NextDouble()),
                JournalSizeMB = Math.Abs(random.NextDouble()),
                ApplyRateMBs = Math.Abs(random.NextDouble())
            };

            return pmaRawEntity;
        }
    }
}
