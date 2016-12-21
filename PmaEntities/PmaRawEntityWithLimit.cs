using System;

namespace PmaEntities
{
    public class PmaRawEntityWithLimit
    {
        public int IsTimeStampValid { get; set; }

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


        public int ProtectedVolumeWriteRateMbs_PassThres { get; set; }
        public int ProtectedVolumeCompressedWriteRateMBs_PassThres { get; set; }
        public int ProtectedCpuPerc_PassThres { get; set; }
        public int ProtectedVraBufferUsagePerc_PassThres { get; set; }
        public int ProtectedTcpBufferUsagePerc_PassThres { get; set; }
        public int NetworkOutgoingRateMBs_PassThres { get; set; }
        public int RecoveryTcpBufferUsagePerc_PassThres { get; set; }
        public int RecoveryCpuPerc_PassThres { get; set; }
        public int RecoveryVraBufferUsagePerc_PassThres { get; set; }
        public int HardeningRateMBs_PassThres { get; set; }
        public int JournalSizeMB_PassThres { get; set; }
        public int ApplyRateMBs_PassThres { get; set; }
    }
}
