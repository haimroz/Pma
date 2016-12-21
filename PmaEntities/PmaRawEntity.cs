namespace PmaEntities
{
    public class PmaRawEntity
    {
        public double ProtectedVolumeWriteRate { get; set; }
        public int ProtectedVraBufferUsage { get; set; }
        public int ProtectedTcpBufferUsage { get; set; }
        public double NetworkOutgoingRate { get; set; }
        public int RecoveryTcpBufferUsage { get; set; }
        public int RecoveryVraBufferUsage { get; set; }
        public double HardeningRate { get; set; }
        public double ApplyRate { get; set; }
    }
}
