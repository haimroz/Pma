using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using PmaEntities;

namespace Ppa.Services
{
    public class PmaRepository
    {

        private const string ConnectionString =@"Server=zdb\SQLSERVER2012;Database=pma;User Id=pma;Password=zertodata;";

        public PmaRawEntity[] GetAll()
        {
            return GenerateDummyPmaData(1000);
        }
        public PmaRawEntity[] GetFilteredData(DateTime from, DateTime to)
        {
            return GenerateDummyPmaData(1000);
        }

        public void SetData(List<PmaRawEntity> pmaList)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BatchSize = 100;
                    bulkCopy.DestinationTableName = "dbo.PmaRawData";
                    try
                    {
                        bulkCopy.WriteToServer(ConvertListToDataTable(pmaList));
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        connection.Close();
                    }
                }
                transaction.Commit();
            }
        }

        public static DataTable ConvertListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }


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