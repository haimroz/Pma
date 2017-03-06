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

        public override bool Equals(object obj)
        {
            var item = obj as RequestedBundleInfo;

            if (item == null)
            {
                return false;
            }

            return Equals(item);
        }

        public bool Equals (RequestedBundleInfo bundleInfo)
        {
            return bundleInfo.ProtectedVraFilePath.Equals(ProtectedVraFilePath) &&
                bundleInfo.RecoveryVraFilePath.Equals(RecoveryVraFilePath);
        }

        public override int GetHashCode()
        {
            return ProtectedVraFilePath.GetHashCode() ^ RecoveryVraFilePath.GetHashCode();
        }
    }
}
