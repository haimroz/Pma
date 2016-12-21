using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ppa.Models
{
    public class PmaData
    {
        public DateTime DateTime { get; set; }

        public int ProtectedVraBufferInPercent { get; set; }

        public bool ProtectedVraThresholdRaised { get; set; }

        public int ProtectedTcpBufferInPercent { get; set; }

        public bool ProtectedTcpThresholdRaised { get; set; }

        public int RecoveryVraBufferInPercent { get; set; }

        public bool RecoveryVraThresholdRaised { get; set; }

        public int RecoveryTcpBufferInPercent { get; set; }

        public bool RecoveryTcpThresholdRaised { get; set; }

    }
}