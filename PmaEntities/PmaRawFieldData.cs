using System;
using System.Runtime.Serialization;

namespace PmaEntities
{
    [DataContract]
    public class PmaRawFieldData
    {
        [DataMember]
        public string FieldName { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string Threshold { get; set; }
        [DataMember]
        public int IsValid { get; set; }

        public PmaRawFieldData(string fieldName, string value, string threshold, int isValid)
        {
            FieldName = fieldName;
            Value = value;
            Threshold = threshold;
            IsValid = isValid;
        }
    }
}
