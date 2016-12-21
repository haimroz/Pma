using System;
using System.Collections.Generic;
using Ppa.Models;

namespace Ppa.Services
{
    public class PmaRepository
    {
        public PmaData[] GetAll()
        {
            return GenerateDummyPmaData(1000);
        }
        public PmaData[] GetFilteredData(DateTime from, DateTime to)
        {
            return GenerateDummyPmaData(1000);
        }

//        public PmaData[] GetFilteredData(DateTime from, DateTime to)
//        {
//            return GenerateDummyPmaData(1000);
//        }


        private PmaData[] GenerateDummyPmaData(int numberOfRows)
        {
            DateTime currentDateTime = DateTime.Now.AddDays(-2);
            List<PmaData> pmaData = new List<PmaData>();
            for (int i = 0; i < numberOfRows; i++)
            {
                currentDateTime = currentDateTime.AddSeconds(1);
                pmaData.Add(CreatePmaRow(currentDateTime, i));
            }
            return pmaData.ToArray();
        }

        private PmaData CreatePmaRow(DateTime dateTime, int index)
        {
            Random random = new Random(index);
            PmaData pmaData = new PmaData
            {
                DateTime = dateTime,
                ProtectedVraBufferInPercent = random.Next(0, 100),
                ProtectedVraThresholdRaised = Convert.ToBoolean(random.Next(0, 1)),
                ProtectedTcpBufferInPercent = random.Next(0, 100),
                ProtectedTcpThresholdRaised = Convert.ToBoolean(random.Next(0, 1)),
                RecoveryVraBufferInPercent = random.Next(0, 100),
                RecoveryVraThresholdRaised = Convert.ToBoolean(random.Next(0, 1)),
                RecoveryTcpBufferInPercent = random.Next(0, 100),
                RecoveryTcpThresholdRaised = Convert.ToBoolean(random.Next(0, 1))
            };

            return pmaData;
        }
    }
}