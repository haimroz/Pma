using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ParseLogs;

namespace ParseLogsTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            PmaLogProcessor logProcessor = new PmaLogProcessor("http://localhost:57904/api/pma");
            logProcessor.ProcessLogs(txtProtectedLogFile.Text, txtRecoveryLogFile.Text);
        }

        private void btnTestMergeEntities_Click(object sender, EventArgs e)
        {
            PmaRawEntitiesInterpolaorTest.MergeLists_ProtectedListEarlier_ProtectedListElementsFirst();
            PmaRawEntitiesInterpolaorTest.MergeLists_ProtectedListEarlier_ProtectedListElementsFirst();
        }
    }
}
