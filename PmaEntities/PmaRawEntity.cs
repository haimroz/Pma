using System;

namespace PmaEntities
{
    public class PmaRawEntity
    {
        public DateTime TimeStamp { get; set; }
        public double ProtectedIOsInDriverMBs { get; set; }
        public double ProtectedVolumeWriteRateMBs { get; set; }
        public double ProtectedVolumeCompressedWriteRateMBs { get; set; }
        public int ProtectedCpuPerc { get; set; }
        public int ProtectedVraBufferUsagePerc { get; set; }
        public int ProtectedTcpBufferUsagePerc { get; set; }
        public double NetworkOutgoingRateMBs { get; set; }  
        public int RecoveryTcpBufferUsagePerc { get; set; }
        public int RecoveryCpuPerc { get; set; }
        public int RecoveryVraBufferUsagePerc { get; set; }
        public double HardeningRateMBs { get; set; }
        public double JournalSizeMB { get; set; }
        public double ApplyRateMBs { get; set; }


        public PmaRawEntity()
        {
        }
        public PmaRawEntity(PmaRawEntity obj)
        {
            TimeStamp = obj.TimeStamp;
            ProtectedIOsInDriverMBs = obj.ProtectedIOsInDriverMBs;
            ProtectedVolumeWriteRateMBs = obj.ProtectedVolumeWriteRateMBs;
            ProtectedVolumeCompressedWriteRateMBs = obj.ProtectedVolumeCompressedWriteRateMBs;
            ProtectedCpuPerc = obj.ProtectedCpuPerc;
            ProtectedVraBufferUsagePerc = obj.ProtectedVraBufferUsagePerc;
            ProtectedTcpBufferUsagePerc = obj.ProtectedTcpBufferUsagePerc;
            NetworkOutgoingRateMBs = obj.NetworkOutgoingRateMBs;
            RecoveryTcpBufferUsagePerc = obj.RecoveryTcpBufferUsagePerc;
            RecoveryCpuPerc = obj.RecoveryCpuPerc;
            RecoveryVraBufferUsagePerc = obj.RecoveryVraBufferUsagePerc;
            HardeningRateMBs = obj.HardeningRateMBs;
            ApplyRateMBs = obj.ApplyRateMBs;
        }

        public double GetMesaurment(int index)
        {
            switch (index)
            {
                case 0:
                    return ProtectedVolumeWriteRateMBs;
                    break;
                case 1:
                    return ProtectedVolumeCompressedWriteRateMBs;
                    break;
                default:
                    throw new IndexOutOfRangeException();
                    break;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", 
                TimeStamp.ToShortDateString(),
                ProtectedIOsInDriverMBs,
                ProtectedVolumeWriteRateMBs,
                ProtectedVolumeCompressedWriteRateMBs,
                ProtectedCpuPerc,
                ProtectedVraBufferUsagePerc,
                ProtectedTcpBufferUsagePerc,
                NetworkOutgoingRateMBs,
                RecoveryTcpBufferUsagePerc,
                RecoveryCpuPerc,
                RecoveryVraBufferUsagePerc,
                HardeningRateMBs,
                ApplyRateMBs);
        }
    }
}
