using System;
using System.Collections.Generic;

namespace PmaEntities
{
    public class PmaTimstampData
    {
        public List<PmaRawFieldData> PmaRawFieldList { get; set; }

        public int IsValid { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
