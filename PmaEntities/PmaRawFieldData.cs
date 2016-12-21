using System;

namespace PmaEntities
{
    public class PmaRawFieldData
    {
        public string FieldName { get; set; }
        public string Value { get; set; }
        public string Threshold { get; set; }
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
