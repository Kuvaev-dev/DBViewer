namespace CloudERPDBViewer
{
    partial class DatabaseConfigForm
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
            textBoxDataSource = new TextBox();
            textBoxInitialCatalog = new TextBox();
            textBoxUsername = new TextBox();
            textBoxPassword = new TextBox();
            testConnectionBtn = new Button();
            checkBoxIntegratedSecurity = new CheckBox();
            saveConnectionBtn = new Button();
            SuspendLayout();
            // 
            // textBoxDataSource
            // 
            textBoxDataSource.Location = new Point(223, 90);
            textBoxDataSource.Name = "textBoxDataSource";
            textBoxDataSource.Size = new Size(328, 23);
            textBoxDataSource.TabIndex = 0;
            // 
            // textBoxInitialCatalog
            // 
            textBoxInitialCatalog.Location = new Point(223, 138);
            textBoxInitialCatalog.Name = "textBoxInitialCatalog";
            textBoxInitialCatalog.Size = new Size(328, 23);
            textBoxInitialCatalog.TabIndex = 1;
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(223, 189);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(328, 23);
            textBoxUsername.TabIndex = 2;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(223, 241);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(328, 23);
            textBoxPassword.TabIndex = 3;
            // 
            // testConnectionBtn
            // 
            testConnectionBtn.Location = new Point(223, 326);
            testConnectionBtn.Name = "testConnectionBtn";
            testConnectionBtn.Size = new Size(155, 23);
            testConnectionBtn.TabIndex = 4;
            testConnectionBtn.Text = "Test Connection";
            testConnectionBtn.UseVisualStyleBackColor = true;
            testConnectionBtn.Click += buttonTestConnection_Click;
            // 
            // checkBoxIntegratedSecurity
            // 
            checkBoxIntegratedSecurity.AutoSize = true;
            checkBoxIntegratedSecurity.Location = new Point(346, 287);
            checkBoxIntegratedSecurity.Name = "checkBoxIntegratedSecurity";
            checkBoxIntegratedSecurity.Size = new Size(125, 19);
            checkBoxIntegratedSecurity.TabIndex = 5;
            checkBoxIntegratedSecurity.Text = "Integrated Security";
            checkBoxIntegratedSecurity.UseVisualStyleBackColor = true;
            // 
            // saveConnectionBtn
            // 
            saveConnectionBtn.Location = new Point(396, 326);
            saveConnectionBtn.Name = "saveConnectionBtn";
            saveConnectionBtn.Size = new Size(155, 23);
            saveConnectionBtn.TabIndex = 6;
            saveConnectionBtn.Text = "Save";
            saveConnectionBtn.UseVisualStyleBackColor = true;
            saveConnectionBtn.Click += buttonSave_Click;
            // 
            // DatabaseConfigForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(saveConnectionBtn);
            Controls.Add(checkBoxIntegratedSecurity);
            Controls.Add(testConnectionBtn);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxUsername);
            Controls.Add(textBoxInitialCatalog);
            Controls.Add(textBoxDataSource);
            Name = "DatabaseConfigForm";
            Text = "DatabaseConfigForm";
            Load += DatabaseConfigForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxDataSource;
        private TextBox textBoxInitialCatalog;
        private TextBox textBoxUsername;
        private TextBox textBoxPassword;
        private Button testConnectionBtn;
        private CheckBox checkBoxIntegratedSecurity;
        private Button saveConnectionBtn;
    }
}