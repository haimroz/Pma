namespace PmaEntities
{
    public class RequestedBundleInfo
    {
        public string BundleDirPath { get; }
        public string ProtectedVraName { get; }
        public string RecoveryVraName { get; }

        public RequestedBundleInfo(string bundleDirPath, string protectedVraName, string recoveryVraName)
        {
            BundleDirPath = bundleDirPath;
            ProtectedVraName = protectedVraName;
            RecoveryVraName = recoveryVraName;
        }
    }
}
