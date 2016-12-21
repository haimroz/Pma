using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using PmaEntities;

namespace Ppa.Services
{
    public class PmaRepository
    {

        private const string ConnectionString =@"Server=zdb\SQLSERVER2012;Database=Pma;User Id=pma;Password=zertodata;";

        public PmaRawEntity[] GetAll()
        {
            return GenerateDummyPmaData(1000);
        }
        //        public PmaRawEntity[] Del(DateTime from, DateTime to)
        //        {
        //
        //            SqlConnection sqlConnection1 = new SqlConnection(ConnectionString);
        //            SqlCommand cmd = new SqlCommand();
        //            SqlDataReader reader;
        //
        //            cmd.CommandText = "SELECT * FROM PmaRawData";
        //            cmd.CommandType = CommandType.Text;
        //            cmd.Connection = sqlConnection1;
        //
        //            sqlConnection1.Open();
        //
        //            reader = cmd.ExecuteReader();
        //            // Data is accessible through the DataReader object here.
        //
        //            sqlConnection1.Close();
        //
        //            return GenerateDummyPmaData(1000);
        //        }
        private readonly DateTime m_minimumSqlUtcDateTime = new DateTime(SqlDateTime.MinValue.Value.Ticks, DateTimeKind.Utc);
        private readonly DateTime m_maximumSqlUtcDateTime = new DateTime(SqlDateTime.MaxValue.Value.Ticks, DateTimeKind.Utc);

        private DateTime ConvertMinMaxSqlDateTimeToDateTime(DateTime dbDateTime)
        {
            DateTime result = dbDateTime;
            if (dbDateTime<m_minimumSqlUtcDateTime)
            {
                result = m_minimumSqlUtcDateTime;
            }
            if (dbDateTime>m_maximumSqlUtcDateTime)
            {
                result = m_maximumSqlUtcDateTime;
            }
            return result;
        }
        public PmaRawEntity[] GetFilteredData(DateTime from, DateTime to)
        {
            List<PmaRawEntity> pmaRawEntities = new List<PmaRawEntity>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string queryString = "SELECT distinct * FROM PmaRawData WHERE TimeStamp Between @from and @to ";
                //string queryString = "SELECT * FROM PmaRawData";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@from", ConvertMinMaxSqlDateTimeToDateTime(from));
                command.Parameters.AddWithValue("@to", ConvertMinMaxSqlDateTimeToDateTime(to));
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        var pmaRawEntity = new PmaRawEntity();
                        pmaRawEntity.TimeStamp = (DateTime)reader["TimeStamp"];
                        pmaRawEntity.ApplyRateMBs = GetDouble(reader, "ApplyRateMBs");
                        pmaRawEntity.HardeningRateMBs = GetDouble(reader, "HardeningRateMBs");
                        pmaRawEntity.JournalSizeMB = GetDouble(reader, "JournalSizeMB");
                        pmaRawEntity.NetworkOutgoingRateMBs = GetDouble(reader, "NetworkOutgoingRateMBs");
                        pmaRawEntity.ProtectedCpuPerc= GetInt(reader, "ProtectedCpuPerc");
                        pmaRawEntity.ProtectedTcpBufferUsagePerc= GetInt(reader, "ProtectedTcpBufferUsagePerc");
                        pmaRawEntity.ProtectedVolumeCompressedWriteRateMBs= GetDouble(reader, "ProtectedVolumeCompressedWriteRateMBs");
                        pmaRawEntity.ProtectedVolumeWriteRateMbs = GetDouble(reader, "ProtectedVolumeWriteRateMbs");
                        pmaRawEntity.ProtectedVraBufferUsagePerc= GetInt(reader, "ProtectedVraBufferUsagePerc");
                        pmaRawEntity.RecoveryCpuPerc= GetInt(reader, "RecoveryCpuPerc");
                        pmaRawEntity.RecoveryVraBufferUsagePerc = GetInt(reader, "RecoveryVraBufferUsagePerc");
                        pmaRawEntity.RecoveryTcpBufferUsagePerc = GetInt(reader, "RecoveryTcpBufferUsagePerc");
                        pmaRawEntities.Add(pmaRawEntity);
                    }
                    return pmaRawEntities.ToArray();
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
        }

        private double GetDouble(SqlDataReader reader, string columnName)
        {
            return (reader[columnName] != DBNull.Value ? reader.GetDouble(reader.GetOrdinal(columnName)) : 0.0);
        }

        private int GetInt(SqlDataReader reader, string columnName)
        {
            return (reader[columnName] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal(columnName)) : 0);
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