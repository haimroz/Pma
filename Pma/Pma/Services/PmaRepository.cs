using System;
using System.Collections.Generic;
using PmaEntities;
using Ppa.Models;

namespace Ppa.Services
{
    public class PmaRepository
    {
        public PmaRawEntity[] GetAll()
        {
            return GenerateDummyPmaData(1000);
        }
        public PmaRawEntity[] GetFilteredData(DateTime from, DateTime to)
        {
            return GenerateDummyPmaData(1000);
        }

//        public PmaData[] GetFilteredData(DateTime from, DateTime to)
//        {
//            return GenerateDummyPmaData(1000);
//        }


        private PmaRawEntity[] GenerateDummyPmaData(int numberOfRows)
        {
            DateTime currentDateTime = DateTime.Now.AddDays(-2);
            List<PmaRawEntity> pmaRawEntity = new List<PmaRawEntity>();
            for (int i = 0; i < numberOfRows; i++)
            {
                currentDateTime = currentDateTime.AddSeconds(1);
                pmaRawEntity.Add(CreatePmaRow(currentDateTime, i));
            }
            return pmaRawEntity.ToArray();
        }

        private PmaRawEntity CreatePmaRow(DateTime dateTime, int index)
        {
            Random random = new Random(index);
            
            PmaRawEntity pmaRawEntity = new PmaRawEntity();
            pmaRawEntity.TimeStamp = dateTime;
            pmaRawEntity.ProtectedVolumeWriteRateMbs = Math.Abs(random.NextDouble());
            pmaRawEntity.ProtectedVolumeCompressedWriteRateMBs = Math.Abs(random.NextDouble());
            pmaRawEntity.ProtectedCpuPerc = random.Next(0, 100);
            pmaRawEntity.ProtectedVraBufferUsagePerc = random.Next(0, 100);
            pmaRawEntity.ProtectedTcpBufferUsagePerc = random.Next(0, 100);
            pmaRawEntity.NetworkOutgoingRateMBs = Math.Abs(random.NextDouble());
            pmaRawEntity.RecoveryTcpBufferUsagePerc = random.Next(0, 100);
            pmaRawEntity.RecoveryCpuPerc = random.Next(0, 100);
            pmaRawEntity.RecoveryVraBufferUsagePerc = random.Next(0, 100);
            pmaRawEntity.HardeningRateMBs = Math.Abs(random.NextDouble());
            pmaRawEntity.JournalSizeMB = Math.Abs(random.NextDouble());
            pmaRawEntity.ApplyRateMBs = Math.Abs(random.NextDouble());

            //            {
            //                DateTime = dateTime,
            //                ProtectedVraBufferInPercent = random.Next(0, 100),
            //                ProtectedVraThresholdRaised = Convert.ToBoolean(random.Next(0, 1)),
            //                ProtectedTcpBufferInPercent = random.Next(0, 100),
            //                ProtectedTcpThresholdRaised = Convert.ToBoolean(random.Next(0, 1)),
            //                RecoveryVraBufferInPercent = random.Next(0, 100),
            //                RecoveryVraThresholdRaised = Convert.ToBoolean(random.Next(0, 1)),
            //                RecoveryTcpBufferInPercent = random.Next(0, 100),
            //                RecoveryTcpThresholdRaised = Convert.ToBoolean(random.Next(0, 1))
            //            };

            return pmaRawEntity;
        }
    }
}