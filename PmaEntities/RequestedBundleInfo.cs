namespace PmaEntities
{
    public class RequestedBundleInfo
    {
        public string ProtectedVraFilePath { get; }
        public string RecoveryVraFilePath { get; }

        public RequestedBundleInfo(string protectedVraFilePath, string recoveryVraFilePath)
        {
            ProtectedVraFilePath = protectedVraFilePath;
            RecoveryVraFilePath = recoveryVraFilePath;
        }
    }
}
