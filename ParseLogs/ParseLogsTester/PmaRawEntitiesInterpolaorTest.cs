using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PmaEntities;
using ParseLogs;

namespace ParseLogsTester
{
    public class PmaRawEntitiesInterpolaorTest
    {
        public static void MergeLists_SameTimeStamps_SameNumberList()
        {
            DateTime startTime = DateTime.Now;

            List<PmaRawEntity> protectedEntities = GenerateDummyPmaData(startTime, 10);
            List<PmaRawEntity> recoveryEntities = GenerateDummyPmaData(startTime, 10);
            PmaRawEntitiesInterpulator interpolator = new PmaRawEntitiesInterpulator();
            List<PmaRawEntity> mergedList = interpolator.MergeLists(protectedEntities, recoveryEntities);
            Debug.Assert(mergedList.Count == 10);
        }

        public static void MergeLists_ProtectedListEarlier_ProtectedListElementsFirst()
        {
            DateTime startTime = DateTime.Now;

            List<PmaRawEntity> protectedEntities = GenerateDummyPmaData(startTime.AddSeconds(5), 10);
            List<PmaRawEntity> recoveryEntities = GenerateDummyPmaData(startTime, 10);
            PmaRawEntitiesInterpulator interpolator = new PmaRawEntitiesInterpulator();
            List<PmaRawEntity> mergedList = interpolator.MergeLists(protectedEntities, recoveryEntities);
            Debug.Assert(mergedList.Count == 5);
        }

        private static List<PmaRawEntity> GenerateDummyPmaData(DateTime startTime, int numberOfRows)
        {
            DateTime currentDateTime = startTime;
            List<PmaRawEntity> pmaRawEntity = new List<PmaRawEntity>();
            for (int i = 0; i < numberOfRows; i++)
            {
                currentDateTime = currentDateTime.AddSeconds(1);
                pmaRawEntity.Add(CreatePmaRow(currentDateTime, i));
            }
            return pmaRawEntity;
        }

        private static PmaRawEntity CreatePmaRow(DateTime dateTime, int index)
        {
            Random random = new Random(index);

            PmaRawEntity pmaRawEntity = new PmaRawEntity
            {
                TimeStamp = dateTime,
                ProtectedIOsInDriverMBs = Math.Abs(random.NextDouble()),
                ProtectedVolumeWriteRateMBs = Math.Abs(random.NextDouble()),
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
