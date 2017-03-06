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
        public int Index { get; set; }

        [DataMember]
        public DateTime TimeStamp { get; set; }

        public void ComputeValidity()
        {
            foreach (PmaRawFieldData field in PmaRawFieldList)
            {
                if (field.FieldName.Equals("RecoveryVraBufferUsagePerc") && field.IsValid == 0)
                {
                    IsValid = 0;
                    return;
                }
            }
            IsValid = 1;
        }
    }
}
