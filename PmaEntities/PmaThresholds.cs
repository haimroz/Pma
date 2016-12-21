namespace PmaEntities
{
    public class PmaThresholds
    {
        public int ProtectedCpuPerc { get { return 80; }}
        public int ProtectedVraBufferUsagePerc { get { return 80; } }
        public int ProtectedTcpBufferUsagePerc { get { return 80; } }
        public int RecoveryTcpBufferUsagePerc { get { return 80; } }
        public int RecoveryCpuPerc { get { return 80; } }
        public int RecoveryVraBufferUsagePerc { get { return 80; } }
    }
}
