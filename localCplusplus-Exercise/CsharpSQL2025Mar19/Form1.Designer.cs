namespace WinFormsApp1SQL
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
            ButtonAdd = new Button();
            dataGridView1 = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            ButtonDelete = new Button();
            groupBox1 = new GroupBox();
            dataGridTransact = new DataGridView();
            groupBox3 = new GroupBox();
            textBoxSearch = new TextBox();
            groupBox2 = new GroupBox();
            btnRestore = new Button();
            btnBackup = new Button();
            textBox1 = new TextBox();
            btnEdit = new Button();
            btnFilter = new Button();
            comboBoxFilter = new ComboBox();
            btnSearch = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridTransact).BeginInit();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // ButtonAdd
            // 
            ButtonAdd.BackColor = Color.CornflowerBlue;
            ButtonAdd.FlatStyle = FlatStyle.Popup;
            ButtonAdd.ForeColor = Color.WhiteSmoke;
            ButtonAdd.Location = new Point(756, 190);
            ButtonAdd.Margin = new Padding(4);
            ButtonAdd.Name = "ButtonAdd";
            ButtonAdd.Size = new Size(96, 32);
            ButtonAdd.TabIndex = 0;
            ButtonAdd.Text = "Add";
            ButtonAdd.UseVisualStyleBackColor = false;
            ButtonAdd.Click += ButtonAdd_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridView1.BackgroundColor = SystemColors.Control;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(34, 27);
            dataGridView1.Margin = new Padding(4);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(860, 232);
            dataGridView1.TabIndex = 1;
            dataGridView1.CellMouseDown += dataGridView1_CellMouseDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(34, 2);
            label1.Name = "label1";
            label1.Size = new Size(226, 21);
            label1.TabIndex = 2;
            label1.Text = "Expense and Income Tracker";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(747, 9);
            label2.Name = "label2";
            label2.Size = new Size(146, 17);
            label2.TabIndex = 3;
            label2.Text = ": Firebird ISQL Tool DB ";
            // 
            // ButtonDelete
            // 
            ButtonDelete.BackColor = Color.Red;
            ButtonDelete.FlatStyle = FlatStyle.Popup;
            ButtonDelete.ForeColor = Color.WhiteSmoke;
            ButtonDelete.Location = new Point(693, 263);
            ButtonDelete.Margin = new Padding(4);
            ButtonDelete.Name = "ButtonDelete";
            ButtonDelete.Size = new Size(96, 32);
            ButtonDelete.TabIndex = 4;
            ButtonDelete.Text = "Delete";
            ButtonDelete.UseVisualStyleBackColor = false;
            ButtonDelete.Click += ButtonDelete_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(dataGridTransact);
            groupBox1.Controls.Add(ButtonAdd);
            groupBox1.Location = new Point(34, 353);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(860, 231);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Add Transactions";
            // 
            // dataGridTransact
            // 
            dataGridTransact.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridTransact.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridTransact.Location = new Point(7, 28);
            dataGridTransact.Name = "dataGridTransact";
            dataGridTransact.RowHeadersWidth = 56;
            dataGridTransact.Size = new Size(845, 155);
            dataGridTransact.TabIndex = 5;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnSearch);
            groupBox3.Controls.Add(textBoxSearch);
            groupBox3.Location = new Point(518, 298);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(376, 60);
            groupBox3.TabIndex = 7;
            groupBox3.TabStop = false;
            groupBox3.Text = "Search";
            // 
            // textBoxSearch
            // 
            textBoxSearch.Location = new Point(10, 22);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.Size = new Size(358, 29);
            textBoxSearch.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnRestore);
            groupBox2.Controls.Add(btnBackup);
            groupBox2.Location = new Point(604, 590);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(289, 67);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "DB Maintenance";
            // 
            // btnRestore
            // 
            btnRestore.FlatStyle = FlatStyle.Popup;
            btnRestore.Location = new Point(186, 25);
            btnRestore.Margin = new Padding(4);
            btnRestore.Name = "btnRestore";
            btnRestore.Size = new Size(96, 32);
            btnRestore.TabIndex = 6;
            btnRestore.Text = "Restore";
            btnRestore.UseVisualStyleBackColor = true;
            // 
            // btnBackup
            // 
            btnBackup.FlatStyle = FlatStyle.Popup;
            btnBackup.Location = new Point(23, 25);
            btnBackup.Margin = new Padding(4);
            btnBackup.Name = "btnBackup";
            btnBackup.Size = new Size(155, 32);
            btnBackup.TabIndex = 5;
            btnBackup.Text = "Backup and Export";
            btnBackup.UseVisualStyleBackColor = true;
            btnBackup.Click += btnBackup_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(34, 263);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(343, 29);
            textBox1.TabIndex = 8;
            textBox1.Text = "Total Income: #### || Total Expenses: ####";
            // 
            // btnEdit
            // 
            btnEdit.FlatStyle = FlatStyle.Popup;
            btnEdit.Location = new Point(797, 263);
            btnEdit.Margin = new Padding(4);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(96, 32);
            btnEdit.TabIndex = 9;
            btnEdit.Text = "Edit";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnFilter
            // 
            btnFilter.BackColor = Color.CornflowerBlue;
            btnFilter.FlatStyle = FlatStyle.Popup;
            btnFilter.ForeColor = Color.WhiteSmoke;
            btnFilter.Location = new Point(34, 302);
            btnFilter.Margin = new Padding(4);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(96, 29);
            btnFilter.TabIndex = 10;
            btnFilter.Text = "Filter";
            btnFilter.UseVisualStyleBackColor = false;
            btnFilter.Click += btnFilter_Click;
            // 
            // comboBoxFilter
            // 
            comboBoxFilter.FormattingEnabled = true;
            comboBoxFilter.Location = new Point(137, 302);
            comboBoxFilter.Name = "comboBoxFilter";
            comboBoxFilter.Size = new Size(240, 29);
            comboBoxFilter.TabIndex = 11;
            // 
            // btnSearch
            // 
            btnSearch.BackColor = Color.CornflowerBlue;
            btnSearch.FlatStyle = FlatStyle.Popup;
            btnSearch.ForeColor = Color.WhiteSmoke;
            btnSearch.Location = new Point(272, 22);
            btnSearch.Margin = new Padding(4);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(96, 29);
            btnSearch.TabIndex = 12;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += btnSearch_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(921, 662);
            Controls.Add(comboBoxFilter);
            Controls.Add(btnFilter);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(btnEdit);
            Controls.Add(ButtonDelete);
            Controls.Add(textBox1);
            Controls.Add(groupBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dataGridView1);
            Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Zaldy Financial Management System ";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridTransact).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ButtonAdd;
        private DataGridView dataGridView1;
        private Label label1;
        private Label label2;
        private Button ButtonDelete;
        private GroupBox groupBox1;
        private DataGridView dataGridTransact;
        private GroupBox groupBox2;
        private Button btnRestore;
        private Button btnBackup;
        private GroupBox groupBox3;
        private TextBox textBox1;
        private Button btnEdit;
        private TextBox textBoxSearch;
        private Button btnFilter;
        private ComboBox comboBoxFilter;
        private Button btnSearch;
    }
}
