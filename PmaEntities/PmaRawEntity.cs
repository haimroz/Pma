using System;
using System.Diagnostics.Contracts;

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
        [Obsolete]
        public double JournalSizeMB { get; set; }
        public double ApplyRateMBs { get; set; }

        public readonly static int NumOfMesaurments = 12;

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
                    return ProtectedIOsInDriverMBs;
                case 1:
                    return ProtectedVolumeWriteRateMBs;
                case 2:
                    return ProtectedVolumeCompressedWriteRateMBs;
                case 3:
                    return ProtectedCpuPerc;
                case 4:
                    return ProtectedVraBufferUsagePerc;
                case 5:
                    return ProtectedTcpBufferUsagePerc;
                case 6:
                    return NetworkOutgoingRateMBs;
                case 7:
                    return RecoveryTcpBufferUsagePerc;
                case 8:
                    return RecoveryCpuPerc;
                case 9:
                    return RecoveryVraBufferUsagePerc;
                case 10:
                    return HardeningRateMBs;
                case 11:
                    return ApplyRateMBs;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public void SetMesaurment(int index, double value)
        {
            switch (index)
            {
                case 0:
                    ProtectedIOsInDriverMBs = value;
                    break;
                case 1:
                    ProtectedVolumeWriteRateMBs = value;
                    break;
                case 2:
                    ProtectedVolumeCompressedWriteRateMBs = value;
                    break;
                case 3:
                    ProtectedCpuPerc = Convert.ToInt32(value);
                    break;
                case 4:
                    ProtectedVraBufferUsagePerc = Convert.ToInt32(value);
                    break;
                case 5:
                    ProtectedTcpBufferUsagePerc = Convert.ToInt32(value);
                    break;
                case 6:
                    NetworkOutgoingRateMBs = value;
                    break;
                case 7:
                    RecoveryTcpBufferUsagePerc = Convert.ToInt32(value);
                    break;
                case 8:
                    RecoveryCpuPerc = Convert.ToInt32(value);
                    break;
                case 9:
                    RecoveryVraBufferUsagePerc = Convert.ToInt32(value);
                    break;
                case 10:
                    HardeningRateMBs = value;
                    break;
                case 11:
                    ApplyRateMBs = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
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
