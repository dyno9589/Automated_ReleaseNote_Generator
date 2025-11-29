namespace ReleaseNoteBuilder
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
            this.btnBuildRN = new System.Windows.Forms.Button();
            this.txtBaseFolder = new System.Windows.Forms.TextBox();
            this.BRFolder = new System.Windows.Forms.Label();
            this.ExpPath = new System.Windows.Forms.Label();
            this.txtExportFolder = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseBase = new System.Windows.Forms.Button();
            this.btnBrowseExport = new System.Windows.Forms.Button();
            this.cbClient = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbRNContr = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbReleaseType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbDataptch = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPrps = new System.Windows.Forms.RichTextBox();
            this.txtRCA = new System.Windows.Forms.RichTextBox();
            this.txtResolution = new System.Windows.Forms.RichTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtImpctRls = new System.Windows.Forms.RichTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtFunImpct = new System.Windows.Forms.RichTextBox();
            this.txtModuleImpct = new System.Windows.Forms.RichTextBox();
            this.txtDeploySteps = new System.Windows.Forms.RichTextBox();
            this.txtRollbckSteps = new System.Windows.Forms.RichTextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBuildRN
            // 
            this.btnBuildRN.Location = new System.Drawing.Point(42, 457);
            this.btnBuildRN.Name = "btnBuildRN";
            this.btnBuildRN.Size = new System.Drawing.Size(100, 29);
            this.btnBuildRN.TabIndex = 0;
            this.btnBuildRN.Text = "Build RN";
            this.btnBuildRN.UseVisualStyleBackColor = true;
            this.btnBuildRN.Click += new System.EventHandler(this.btnBuildRN_Click);
            // 
            // txtBaseFolder
            // 
            this.txtBaseFolder.Location = new System.Drawing.Point(146, 51);
            this.txtBaseFolder.Name = "txtBaseFolder";
            this.txtBaseFolder.Size = new System.Drawing.Size(550, 20);
            this.txtBaseFolder.TabIndex = 1;
            // 
            // BRFolder
            // 
            this.BRFolder.AutoSize = true;
            this.BRFolder.Location = new System.Drawing.Point(39, 54);
            this.BRFolder.Name = "BRFolder";
            this.BRFolder.Size = new System.Drawing.Size(102, 13);
            this.BRFolder.TabIndex = 2;
            this.BRFolder.Text = "Base Release folder";
            // 
            // ExpPath
            // 
            this.ExpPath.AutoSize = true;
            this.ExpPath.Location = new System.Drawing.Point(39, 92);
            this.ExpPath.Name = "ExpPath";
            this.ExpPath.Size = new System.Drawing.Size(69, 13);
            this.ExpPath.TabIndex = 3;
            this.ExpPath.Text = "Export Folder";
            // 
            // txtExportFolder
            // 
            this.txtExportFolder.Location = new System.Drawing.Point(146, 89);
            this.txtExportFolder.Name = "txtExportFolder";
            this.txtExportFolder.Size = new System.Drawing.Size(550, 20);
            this.txtExportFolder.TabIndex = 4;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(39, 507);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Status";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(297, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Release Note Builder";
            // 
            // btnBrowseBase
            // 
            this.btnBrowseBase.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBrowseBase.Location = new System.Drawing.Point(702, 51);
            this.btnBrowseBase.Name = "btnBrowseBase";
            this.btnBrowseBase.Size = new System.Drawing.Size(26, 20);
            this.btnBrowseBase.TabIndex = 7;
            this.btnBrowseBase.Text = "...";
            this.btnBrowseBase.UseVisualStyleBackColor = true;
            this.btnBrowseBase.Click += new System.EventHandler(this.btnBrowseBase_Click);
            // 
            // btnBrowseExport
            // 
            this.btnBrowseExport.Location = new System.Drawing.Point(702, 89);
            this.btnBrowseExport.Name = "btnBrowseExport";
            this.btnBrowseExport.Size = new System.Drawing.Size(26, 20);
            this.btnBrowseExport.TabIndex = 8;
            this.btnBrowseExport.Text = "...";
            this.btnBrowseExport.UseVisualStyleBackColor = true;
            this.btnBrowseExport.Click += new System.EventHandler(this.btnBrowseExport_Click);
            // 
            // cbClient
            // 
            this.cbClient.FormattingEnabled = true;
            this.cbClient.Items.AddRange(new object[] {
            "IDFC First Bank",
            "Kotak Securities",
            "IndusInd Bank Ltd",
            "Cosmos Bank",
            "JSB",
            "NKGSB"});
            this.cbClient.Location = new System.Drawing.Point(146, 131);
            this.cbClient.Name = "cbClient";
            this.cbClient.Size = new System.Drawing.Size(121, 21);
            this.cbClient.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Client Name";
            // 
            // cbRNContr
            // 
            this.cbRNContr.FormattingEnabled = true;
            this.cbRNContr.Items.AddRange(new object[] {
            "Dinesh Prajapati",
            "Manoj Kadam",
            "Mayur Dhokne",
            "Nauman Shaikh",
            "Siddhesh Gundal"});
            this.cbRNContr.Location = new System.Drawing.Point(146, 165);
            this.cbRNContr.Name = "cbRNContr";
            this.cbRNContr.Size = new System.Drawing.Size(121, 21);
            this.cbRNContr.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Release Co-ordinator";
            // 
            // cbReleaseType
            // 
            this.cbReleaseType.FormattingEnabled = true;
            this.cbReleaseType.Items.AddRange(new object[] {
            "UAT",
            "PRODUCTION"});
            this.cbReleaseType.Location = new System.Drawing.Point(146, 200);
            this.cbReleaseType.Name = "cbReleaseType";
            this.cbReleaseType.Size = new System.Drawing.Size(121, 21);
            this.cbReleaseType.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 203);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "UAT / PROD";
            // 
            // cbDataptch
            // 
            this.cbDataptch.FormattingEnabled = true;
            this.cbDataptch.Items.AddRange(new object[] {
            "No",
            "Yes"});
            this.cbDataptch.Location = new System.Drawing.Point(146, 230);
            this.cbDataptch.Name = "cbDataptch";
            this.cbDataptch.Size = new System.Drawing.Size(121, 21);
            this.cbDataptch.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 233);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Data Patch";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(333, 139);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Purpose of Release";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(333, 221);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Root Cause Analysis";
            // 
            // txtPrps
            // 
            this.txtPrps.Location = new System.Drawing.Point(439, 131);
            this.txtPrps.Name = "txtPrps";
            this.txtPrps.Size = new System.Drawing.Size(349, 72);
            this.txtPrps.TabIndex = 16;
            this.txtPrps.Text = "";
            // 
            // txtRCA
            // 
            this.txtRCA.Location = new System.Drawing.Point(439, 209);
            this.txtRCA.Name = "txtRCA";
            this.txtRCA.Size = new System.Drawing.Size(349, 78);
            this.txtRCA.TabIndex = 17;
            this.txtRCA.Text = "";
            // 
            // txtResolution
            // 
            this.txtResolution.Location = new System.Drawing.Point(439, 294);
            this.txtResolution.Name = "txtResolution";
            this.txtResolution.Size = new System.Drawing.Size(349, 69);
            this.txtResolution.TabIndex = 18;
            this.txtResolution.Text = "";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(376, 307);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Resolution";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(39, 285);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(93, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Impact of Release";
            // 
            // txtImpctRls
            // 
            this.txtImpctRls.Location = new System.Drawing.Point(146, 267);
            this.txtImpctRls.Name = "txtImpctRls";
            this.txtImpctRls.Size = new System.Drawing.Size(184, 53);
            this.txtImpctRls.TabIndex = 19;
            this.txtImpctRls.Text = "";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(39, 350);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 13);
            this.label10.TabIndex = 10;
            this.label10.Text = "Functional Impact";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(39, 408);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(94, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Modules Impacted";
            // 
            // txtFunImpct
            // 
            this.txtFunImpct.Location = new System.Drawing.Point(146, 330);
            this.txtFunImpct.Name = "txtFunImpct";
            this.txtFunImpct.Size = new System.Drawing.Size(184, 52);
            this.txtFunImpct.TabIndex = 20;
            this.txtFunImpct.Text = "";
            // 
            // txtModuleImpct
            // 
            this.txtModuleImpct.Location = new System.Drawing.Point(146, 390);
            this.txtModuleImpct.Name = "txtModuleImpct";
            this.txtModuleImpct.Size = new System.Drawing.Size(184, 55);
            this.txtModuleImpct.TabIndex = 21;
            this.txtModuleImpct.Text = "";
            // 
            // txtDeploySteps
            // 
            this.txtDeploySteps.Location = new System.Drawing.Point(439, 369);
            this.txtDeploySteps.Name = "txtDeploySteps";
            this.txtDeploySteps.Size = new System.Drawing.Size(349, 66);
            this.txtDeploySteps.TabIndex = 22;
            this.txtDeploySteps.Text = "";
            // 
            // txtRollbckSteps
            // 
            this.txtRollbckSteps.Location = new System.Drawing.Point(439, 441);
            this.txtRollbckSteps.Name = "txtRollbckSteps";
            this.txtRollbckSteps.Size = new System.Drawing.Size(349, 61);
            this.txtRollbckSteps.TabIndex = 23;
            this.txtRollbckSteps.Text = "";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(367, 395);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 26);
            this.label12.TabIndex = 10;
            this.label12.Text = "Deployment \r\nSteps";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(358, 457);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(79, 26);
            this.label13.TabIndex = 10;
            this.label13.Text = "Release \r\nRollback Steps";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 529);
            this.Controls.Add(this.txtRollbckSteps);
            this.Controls.Add(this.txtDeploySteps);
            this.Controls.Add(this.txtModuleImpct);
            this.Controls.Add(this.txtFunImpct);
            this.Controls.Add(this.txtImpctRls);
            this.Controls.Add(this.txtResolution);
            this.Controls.Add(this.txtRCA);
            this.Controls.Add(this.txtPrps);
            this.Controls.Add(this.cbDataptch);
            this.Controls.Add(this.cbReleaseType);
            this.Controls.Add(this.cbRNContr);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbClient);
            this.Controls.Add(this.btnBrowseExport);
            this.Controls.Add(this.btnBrowseBase);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtExportFolder);
            this.Controls.Add(this.ExpPath);
            this.Controls.Add(this.BRFolder);
            this.Controls.Add(this.txtBaseFolder);
            this.Controls.Add(this.btnBuildRN);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBuildRN;
        private System.Windows.Forms.TextBox txtBaseFolder;
        private System.Windows.Forms.Label BRFolder;
        private System.Windows.Forms.Label ExpPath;
        private System.Windows.Forms.TextBox txtExportFolder;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseBase;
        private System.Windows.Forms.Button btnBrowseExport;
        private System.Windows.Forms.ComboBox cbClient;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbRNContr;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbReleaseType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbDataptch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RichTextBox txtPrps;
        private System.Windows.Forms.RichTextBox txtRCA;
        private System.Windows.Forms.RichTextBox txtResolution;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RichTextBox txtImpctRls;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RichTextBox txtFunImpct;
        private System.Windows.Forms.RichTextBox txtModuleImpct;
        private System.Windows.Forms.RichTextBox txtDeploySteps;
        private System.Windows.Forms.RichTextBox txtRollbckSteps;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
    }
}

