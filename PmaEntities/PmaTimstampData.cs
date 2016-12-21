using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PmaEntities
{
    [DataContract]
    public class PmaTimstampData
    {
        [DataMember]
        public List<PmaRawFieldData> PmaRawFieldList { get; set; }
        [DataMember]
        public int IsValid { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
    }
}
