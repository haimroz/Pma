using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PmaEntities
{
    [DataContract]
    public class PmaInfo
    {
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public List<PmaTimstampData> PmaData { get; set; }

        public PmaInfo(int count, List<PmaTimstampData> pmaData)
        {
            Count = count;
            PmaData = pmaData;
        }
    }
}
