namespace ContinuumProcessRunner
{
    partial class ProcessRunnerUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelHeader = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelDivider1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comboBoxExePathField = new System.Windows.Forms.ComboBox();
            this.labelExePath = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.textBoxStdOutField = new System.Windows.Forms.TextBox();
            this.labelStdOutField = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.textBoxRetCodeField = new System.Windows.Forms.TextBox();
            this.labelRetCodeField = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.textBoxExceptionField = new System.Windows.Forms.TextBox();
            this.labelExceptionField = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel9.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelVersion);
            this.panel1.Controls.Add(this.labelHeader);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 30);
            this.panel1.TabIndex = 0;
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.Location = new System.Drawing.Point(170, 11);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(38, 12);
            this.labelVersion.TabIndex = 1;
            this.labelVersion.Text = "(v1.0.1)";
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.Location = new System.Drawing.Point(6, 2);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(160, 25);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "ProcessRunner";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelDivider1);
            this.panel2.Location = new System.Drawing.Point(0, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(300, 30);
            this.panel2.TabIndex = 1;
            // 
            // labelDivider1
            // 
            this.labelDivider1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelDivider1.Location = new System.Drawing.Point(8, 14);
            this.labelDivider1.Name = "labelDivider1";
            this.labelDivider1.Size = new System.Drawing.Size(284, 2);
            this.labelDivider1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.comboBoxExePathField);
            this.panel3.Controls.Add(this.labelExePath);
            this.panel3.Location = new System.Drawing.Point(0, 60);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(300, 30);
            this.panel3.TabIndex = 2;
            // 
            // comboBoxExePathField
            // 
            this.comboBoxExePathField.FormattingEnabled = true;
            this.comboBoxExePathField.Location = new System.Drawing.Point(120, 4);
            this.comboBoxExePathField.Name = "comboBoxExePathField";
            this.comboBoxExePathField.Size = new System.Drawing.Size(170, 21);
            this.comboBoxExePathField.TabIndex = 1;
            // 
            // labelExePath
            // 
            this.labelExePath.AutoSize = true;
            this.labelExePath.Location = new System.Drawing.Point(8, 8);
            this.labelExePath.Name = "labelExePath";
            this.labelExePath.Size = new System.Drawing.Size(53, 13);
            this.labelExePath.TabIndex = 0;
            this.labelExePath.Text = "Exe Path:";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.textBoxStdOutField);
            this.panel7.Controls.Add(this.labelStdOutField);
            this.panel7.Location = new System.Drawing.Point(0, 90);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(300, 30);
            this.panel7.TabIndex = 6;
            // 
            // textBoxStdOutField
            // 
            this.textBoxStdOutField.Location = new System.Drawing.Point(120, 4);
            this.textBoxStdOutField.Name = "textBoxStdOutField";
            this.textBoxStdOutField.Size = new System.Drawing.Size(170, 20);
            this.textBoxStdOutField.TabIndex = 1;
            // 
            // labelStdOutField
            // 
            this.labelStdOutField.AutoSize = true;
            this.labelStdOutField.Location = new System.Drawing.Point(8, 8);
            this.labelStdOutField.Name = "labelStdOutField";
            this.labelStdOutField.Size = new System.Drawing.Size(46, 13);
            this.labelStdOutField.TabIndex = 0;
            this.labelStdOutField.Text = "Std Out:";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.textBoxRetCodeField);
            this.panel8.Controls.Add(this.labelRetCodeField);
            this.panel8.Location = new System.Drawing.Point(0, 120);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(300, 30);
            this.panel8.TabIndex = 7;
            // 
            // textBoxRetCodeField
            // 
            this.textBoxRetCodeField.Location = new System.Drawing.Point(120, 4);
            this.textBoxRetCodeField.Name = "textBoxRetCodeField";
            this.textBoxRetCodeField.Size = new System.Drawing.Size(170, 20);
            this.textBoxRetCodeField.TabIndex = 1;
            // 
            // labelRetCodeField
            // 
            this.labelRetCodeField.AutoSize = true;
            this.labelRetCodeField.Location = new System.Drawing.Point(8, 8);
            this.labelRetCodeField.Name = "labelRetCodeField";
            this.labelRetCodeField.Size = new System.Drawing.Size(70, 13);
            this.labelRetCodeField.TabIndex = 0;
            this.labelRetCodeField.Text = "Return Code:";
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.textBoxExceptionField);
            this.panel9.Controls.Add(this.labelExceptionField);
            this.panel9.Location = new System.Drawing.Point(0, 150);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(300, 30);
            this.panel9.TabIndex = 8;
            // 
            // textBoxExceptionField
            // 
            this.textBoxExceptionField.Location = new System.Drawing.Point(120, 4);
            this.textBoxExceptionField.Name = "textBoxExceptionField";
            this.textBoxExceptionField.Size = new System.Drawing.Size(170, 20);
            this.textBoxExceptionField.TabIndex = 1;
            // 
            // labelExceptionField
            // 
            this.labelExceptionField.AutoSize = true;
            this.labelExceptionField.Location = new System.Drawing.Point(8, 8);
            this.labelExceptionField.Name = "labelExceptionField";
            this.labelExceptionField.Size = new System.Drawing.Size(62, 13);
            this.labelExceptionField.TabIndex = 0;
            this.labelExceptionField.Text = "Exceptions:";
            // 
            // ProcessRunnerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.panel9);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ProcessRunnerUserControl";
            this.Size = new System.Drawing.Size(330, 210);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelDivider1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label labelExePath;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.ComboBox comboBoxExePathField;
        private System.Windows.Forms.TextBox textBoxStdOutField;
        private System.Windows.Forms.Label labelStdOutField;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.TextBox textBoxRetCodeField;
        private System.Windows.Forms.Label labelRetCodeField;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.TextBox textBoxExceptionField;
        private System.Windows.Forms.Label labelExceptionField;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
