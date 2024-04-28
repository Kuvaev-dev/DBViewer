namespace CloudERPDBViewer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            treeView = new TreeView();
            dataGridViewData = new DataGridView();
            comboBoxSort = new ComboBox();
            comboBoxSortOrder = new ComboBox();
            flowLayoutPanelParameters = new FlowLayoutPanel();
            comboBoxSortData = new ComboBox();
            textBoxSearch = new TextBox();
            logoutBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewData).BeginInit();
            SuspendLayout();
            // 
            // treeView
            // 
            treeView.Location = new Point(12, 210);
            treeView.Name = "treeView";
            treeView.Size = new Size(303, 509);
            treeView.TabIndex = 0;
            treeView.AfterSelect += treeView_AfterSelect;
            // 
            // dataGridViewData
            // 
            dataGridViewData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewData.Location = new Point(334, 210);
            dataGridViewData.Name = "dataGridViewData";
            dataGridViewData.RowTemplate.Height = 25;
            dataGridViewData.Size = new Size(747, 509);
            dataGridViewData.TabIndex = 1;
            // 
            // comboBoxSort
            // 
            comboBoxSort.FormattingEnabled = true;
            comboBoxSort.Location = new Point(12, 8);
            comboBoxSort.Name = "comboBoxSort";
            comboBoxSort.Size = new Size(121, 23);
            comboBoxSort.TabIndex = 2;
            comboBoxSort.SelectedIndexChanged += comboBoxSort_SelectedIndexChanged;
            // 
            // comboBoxSortOrder
            // 
            comboBoxSortOrder.FormattingEnabled = true;
            comboBoxSortOrder.Location = new Point(152, 8);
            comboBoxSortOrder.Name = "comboBoxSortOrder";
            comboBoxSortOrder.Size = new Size(121, 23);
            comboBoxSortOrder.TabIndex = 4;
            comboBoxSortOrder.SelectedIndexChanged += comboBoxSortOrder_SelectedIndexChanged;
            // 
            // flowLayoutPanelParameters
            // 
            flowLayoutPanelParameters.Location = new Point(334, 8);
            flowLayoutPanelParameters.Name = "flowLayoutPanelParameters";
            flowLayoutPanelParameters.Size = new Size(747, 187);
            flowLayoutPanelParameters.TabIndex = 5;
            // 
            // comboBoxSortData
            // 
            comboBoxSortData.FormattingEnabled = true;
            comboBoxSortData.Location = new Point(12, 49);
            comboBoxSortData.Name = "comboBoxSortData";
            comboBoxSortData.Size = new Size(121, 23);
            comboBoxSortData.TabIndex = 6;
            comboBoxSortData.SelectedIndexChanged += comboBoxSortData_SelectedIndexChanged;
            // 
            // textBoxSearch
            // 
            textBoxSearch.Location = new Point(12, 87);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.Size = new Size(261, 23);
            textBoxSearch.TabIndex = 7;
            textBoxSearch.TextChanged += textBoxSearch_TextChanged;
            // 
            // logoutBtn
            // 
            logoutBtn.Location = new Point(12, 127);
            logoutBtn.Name = "logoutBtn";
            logoutBtn.Size = new Size(75, 23);
            logoutBtn.TabIndex = 8;
            logoutBtn.Text = "Logout";
            logoutBtn.UseVisualStyleBackColor = true;
            logoutBtn.Click += logoutBtn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1093, 731);
            Controls.Add(logoutBtn);
            Controls.Add(textBoxSearch);
            Controls.Add(comboBoxSortData);
            Controls.Add(flowLayoutPanelParameters);
            Controls.Add(comboBoxSortOrder);
            Controls.Add(comboBoxSort);
            Controls.Add(dataGridViewData);
            Controls.Add(treeView);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewData).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView treeView;
        private DataGridView dataGridViewData;
        private ComboBox comboBoxSort;
        private ComboBox comboBoxSortOrder;
        private FlowLayoutPanel flowLayoutPanelParameters;
        private ComboBox comboBoxSortData;
        private TextBox textBoxSearch;
        private Button logoutBtn;
    }
}