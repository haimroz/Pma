namespace ParseLogsTester
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnParse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtProtectedLogFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRecoveryLogFile = new System.Windows.Forms.TextBox();
            this.btnTestMergeEntities = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnParse
            // 
            this.btnParse.Location = new System.Drawing.Point(344, 82);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(75, 23);
            this.btnParse.TabIndex = 0;
            this.btnParse.Text = "Parse File";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Protected Log File:";
            // 
            // txtProtectedLogFile
            // 
            this.txtProtectedLogFile.Location = new System.Drawing.Point(105, 12);
            this.txtProtectedLogFile.Name = "txtProtectedLogFile";
            this.txtProtectedLogFile.Size = new System.Drawing.Size(671, 20);
            this.txtProtectedLogFile.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Recovery Log Files:";
            // 
            // txtRecoveryLogFile
            // 
            this.txtRecoveryLogFile.Location = new System.Drawing.Point(105, 43);
            this.txtRecoveryLogFile.Name = "txtRecoveryLogFile";
            this.txtRecoveryLogFile.Size = new System.Drawing.Size(671, 20);
            this.txtRecoveryLogFile.TabIndex = 4;
            // 
            // btnTestMergeEntities
            // 
            this.btnTestMergeEntities.Location = new System.Drawing.Point(396, 279);
            this.btnTestMergeEntities.Name = "btnTestMergeEntities";
            this.btnTestMergeEntities.Size = new System.Drawing.Size(75, 23);
            this.btnTestMergeEntities.TabIndex = 5;
            this.btnTestMergeEntities.Text = "button1";
            this.btnTestMergeEntities.UseVisualStyleBackColor = true;
            this.btnTestMergeEntities.Click += new System.EventHandler(this.btnTestMergeEntities_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 370);
            this.Controls.Add(this.btnTestMergeEntities);
            this.Controls.Add(this.txtRecoveryLogFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtProtectedLogFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnParse);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtProtectedLogFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRecoveryLogFile;
        private System.Windows.Forms.Button btnTestMergeEntities;
    }
}

