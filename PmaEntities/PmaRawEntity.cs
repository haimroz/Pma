using System;

namespace PmaEntities
{
    public class PmaRawEntity
    {
        public DateTime TimeStamp { get; set; }
        public double ProtectedVolumeWriteRateMbs { get; set; }
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
    }
}
