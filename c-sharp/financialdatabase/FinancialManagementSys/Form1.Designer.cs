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
            btnSearch = new Button();
            textBoxSearch = new TextBox();
            groupBox2 = new GroupBox();
            btnRestore = new Button();
            btnBackup = new Button();
            textBox1TotalExpense = new TextBox();
            btnEdit = new Button();
            btnFilter = new Button();
            comboBoxFilter = new ComboBox();
            textBox1TotalIncome = new TextBox();
            textBox1Total = new TextBox();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            tabControlWindow = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            textBox2Total = new TextBox();
            textBox2TotalIncome = new TextBox();
            textBox2TotalExpense = new TextBox();
            dataGridView2 = new DataGridView();
            label8 = new Label();
            label6 = new Label();
            label7 = new Label();
            tabPage3 = new TabPage();
            textBox3Total = new TextBox();
            textBox3TotalIncome = new TextBox();
            textBox3TotalExpense = new TextBox();
            dataGridView3 = new DataGridView();
            label11 = new Label();
            label9 = new Label();
            label10 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridTransact).BeginInit();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            tabControlWindow.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).BeginInit();
            SuspendLayout();
            // 
            // ButtonAdd
            // 
            ButtonAdd.BackColor = Color.CornflowerBlue;
            ButtonAdd.FlatStyle = FlatStyle.Popup;
            ButtonAdd.ForeColor = Color.WhiteSmoke;
            ButtonAdd.Location = new Point(665, 185);
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
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridView1.BackgroundColor = SystemColors.Control;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(10, 7);
            dataGridView1.Margin = new Padding(4);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(860, 232);
            dataGridView1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(358, 2);
            label1.Name = "label1";
            label1.Size = new Size(226, 21);
            label1.TabIndex = 2;
            label1.Text = "Expense and Income Tracker";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(783, 9);
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
            ButtonDelete.Location = new Point(716, 322);
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
            groupBox1.Location = new Point(34, 415);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(769, 225);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Add Transactions";
            // 
            // dataGridTransact
            // 
            dataGridTransact.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridTransact.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridTransact.Location = new Point(6, 28);
            dataGridTransact.Name = "dataGridTransact";
            dataGridTransact.RowHeadersWidth = 56;
            dataGridTransact.Size = new Size(755, 155);
            dataGridTransact.TabIndex = 5;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnSearch);
            groupBox3.Controls.Add(textBoxSearch);
            groupBox3.Location = new Point(540, 349);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(376, 60);
            groupBox3.TabIndex = 7;
            groupBox3.TabStop = false;
            groupBox3.Text = "Search";
            // 
            // btnSearch
            // 
            btnSearch.BackColor = Color.CornflowerBlue;
            btnSearch.FlatStyle = FlatStyle.Popup;
            btnSearch.ForeColor = Color.WhiteSmoke;
            btnSearch.Location = new Point(280, 22);
            btnSearch.Margin = new Padding(4);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(96, 29);
            btnSearch.TabIndex = 12;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += btnSearch_Click;
            // 
            // textBoxSearch
            // 
            textBoxSearch.Location = new Point(18, 22);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.Size = new Size(358, 29);
            textBoxSearch.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnRestore);
            groupBox2.Controls.Add(btnBackup);
            groupBox2.Location = new Point(817, 415);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(99, 172);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Database";
            // 
            // btnRestore
            // 
            btnRestore.FlatStyle = FlatStyle.Popup;
            btnRestore.Location = new Point(7, 124);
            btnRestore.Margin = new Padding(4);
            btnRestore.Name = "btnRestore";
            btnRestore.Size = new Size(86, 32);
            btnRestore.TabIndex = 6;
            btnRestore.Text = "Restore";
            btnRestore.UseVisualStyleBackColor = true;
            btnRestore.Click += btnRestore_Click;
            // 
            // btnBackup
            // 
            btnBackup.FlatStyle = FlatStyle.Popup;
            btnBackup.Location = new Point(7, 29);
            btnBackup.Margin = new Padding(4);
            btnBackup.Name = "btnBackup";
            btnBackup.Size = new Size(86, 87);
            btnBackup.TabIndex = 5;
            btnBackup.Text = "Backup\r\nand\r\nExport";
            btnBackup.UseVisualStyleBackColor = true;
            btnBackup.Click += btnBackup_Click;
            // 
            // textBox1TotalExpense
            // 
            textBox1TotalExpense.BorderStyle = BorderStyle.None;
            textBox1TotalExpense.Location = new Point(460, 246);
            textBox1TotalExpense.Name = "textBox1TotalExpense";
            textBox1TotalExpense.ReadOnly = true;
            textBox1TotalExpense.Size = new Size(100, 22);
            textBox1TotalExpense.TabIndex = 8;
            textBox1TotalExpense.Text = "#######";
            // 
            // btnEdit
            // 
            btnEdit.FlatStyle = FlatStyle.Popup;
            btnEdit.Location = new Point(820, 322);
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
            btnFilter.Location = new Point(43, 325);
            btnFilter.Margin = new Padding(4);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(96, 29);
            btnFilter.TabIndex = 10;
            btnFilter.Text = "Category";
            btnFilter.UseVisualStyleBackColor = false;
            btnFilter.Click += btnFilter_Click;
            // 
            // comboBoxFilter
            // 
            comboBoxFilter.FormattingEnabled = true;
            comboBoxFilter.Location = new Point(146, 325);
            comboBoxFilter.Name = "comboBoxFilter";
            comboBoxFilter.Size = new Size(240, 29);
            comboBoxFilter.TabIndex = 11;
            // 
            // textBox1TotalIncome
            // 
            textBox1TotalIncome.BorderStyle = BorderStyle.None;
            textBox1TotalIncome.Location = new Point(254, 246);
            textBox1TotalIncome.Name = "textBox1TotalIncome";
            textBox1TotalIncome.ReadOnly = true;
            textBox1TotalIncome.Size = new Size(86, 22);
            textBox1TotalIncome.TabIndex = 12;
            textBox1TotalIncome.Text = "#######";
            // 
            // textBox1Total
            // 
            textBox1Total.BorderStyle = BorderStyle.None;
            textBox1Total.Location = new Point(58, 246);
            textBox1Total.Name = "textBox1Total";
            textBox1Total.ReadOnly = true;
            textBox1Total.Size = new Size(89, 22);
            textBox1Total.TabIndex = 13;
            textBox1Total.Text = "#######";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(10, 246);
            label3.Name = "label3";
            label3.Size = new Size(49, 21);
            label3.TabIndex = 14;
            label3.Text = "Total:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(146, 246);
            label4.Name = "label4";
            label4.Size = new Size(108, 21);
            label4.TabIndex = 15;
            label4.Text = "Total Income:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(341, 246);
            label5.Name = "label5";
            label5.Size = new Size(120, 21);
            label5.TabIndex = 16;
            label5.Text = "Total Expenses:";
            // 
            // tabControlWindow
            // 
            tabControlWindow.Controls.Add(tabPage1);
            tabControlWindow.Controls.Add(tabPage2);
            tabControlWindow.Controls.Add(tabPage3);
            tabControlWindow.Location = new Point(39, 2);
            tabControlWindow.Name = "tabControlWindow";
            tabControlWindow.SelectedIndex = 0;
            tabControlWindow.Size = new Size(886, 313);
            tabControlWindow.TabIndex = 17;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = SystemColors.Control;
            tabPage1.Controls.Add(textBox1TotalExpense);
            tabPage1.Controls.Add(textBox1TotalIncome);
            tabPage1.Controls.Add(textBox1Total);
            tabPage1.Controls.Add(dataGridView1);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(label5);
            tabPage1.Location = new Point(4, 30);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(878, 279);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Window 1";
            // 
            // tabPage2
            // 
            tabPage2.BackColor = SystemColors.Control;
            tabPage2.Controls.Add(textBox2Total);
            tabPage2.Controls.Add(textBox2TotalIncome);
            tabPage2.Controls.Add(textBox2TotalExpense);
            tabPage2.Controls.Add(dataGridView2);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(label7);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(878, 285);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Window 2";
            // 
            // textBox2Total
            // 
            textBox2Total.BorderStyle = BorderStyle.None;
            textBox2Total.Location = new Point(56, 246);
            textBox2Total.Name = "textBox2Total";
            textBox2Total.ReadOnly = true;
            textBox2Total.Size = new Size(89, 22);
            textBox2Total.TabIndex = 21;
            textBox2Total.Text = "#######";
            // 
            // textBox2TotalIncome
            // 
            textBox2TotalIncome.BorderStyle = BorderStyle.None;
            textBox2TotalIncome.Location = new Point(251, 246);
            textBox2TotalIncome.Name = "textBox2TotalIncome";
            textBox2TotalIncome.ReadOnly = true;
            textBox2TotalIncome.Size = new Size(86, 22);
            textBox2TotalIncome.TabIndex = 20;
            textBox2TotalIncome.Text = "#######";
            // 
            // textBox2TotalExpense
            // 
            textBox2TotalExpense.BorderStyle = BorderStyle.None;
            textBox2TotalExpense.Location = new Point(457, 246);
            textBox2TotalExpense.Name = "textBox2TotalExpense";
            textBox2TotalExpense.ReadOnly = true;
            textBox2TotalExpense.Size = new Size(100, 22);
            textBox2TotalExpense.TabIndex = 19;
            textBox2TotalExpense.Text = "#######";
            // 
            // dataGridView2
            // 
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridView2.BackgroundColor = SystemColors.Control;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(7, 7);
            dataGridView2.Margin = new Padding(4);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.ReadOnly = true;
            dataGridView2.Size = new Size(860, 232);
            dataGridView2.TabIndex = 18;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(338, 246);
            label8.Name = "label8";
            label8.Size = new Size(120, 21);
            label8.TabIndex = 24;
            label8.Text = "Total Expenses:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(7, 246);
            label6.Name = "label6";
            label6.Size = new Size(49, 21);
            label6.TabIndex = 22;
            label6.Text = "Total:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(143, 246);
            label7.Name = "label7";
            label7.Size = new Size(108, 21);
            label7.TabIndex = 23;
            label7.Text = "Total Income:";
            // 
            // tabPage3
            // 
            tabPage3.BackColor = SystemColors.Control;
            tabPage3.Controls.Add(textBox3Total);
            tabPage3.Controls.Add(textBox3TotalIncome);
            tabPage3.Controls.Add(textBox3TotalExpense);
            tabPage3.Controls.Add(dataGridView3);
            tabPage3.Controls.Add(label11);
            tabPage3.Controls.Add(label9);
            tabPage3.Controls.Add(label10);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(878, 285);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Window 3";
            // 
            // textBox3Total
            // 
            textBox3Total.BorderStyle = BorderStyle.None;
            textBox3Total.Location = new Point(60, 246);
            textBox3Total.Name = "textBox3Total";
            textBox3Total.ReadOnly = true;
            textBox3Total.Size = new Size(89, 22);
            textBox3Total.TabIndex = 21;
            textBox3Total.Text = "#######";
            // 
            // textBox3TotalIncome
            // 
            textBox3TotalIncome.BorderStyle = BorderStyle.None;
            textBox3TotalIncome.Location = new Point(255, 246);
            textBox3TotalIncome.Name = "textBox3TotalIncome";
            textBox3TotalIncome.ReadOnly = true;
            textBox3TotalIncome.Size = new Size(86, 22);
            textBox3TotalIncome.TabIndex = 20;
            textBox3TotalIncome.Text = "#######";
            // 
            // textBox3TotalExpense
            // 
            textBox3TotalExpense.BorderStyle = BorderStyle.None;
            textBox3TotalExpense.Location = new Point(461, 246);
            textBox3TotalExpense.Name = "textBox3TotalExpense";
            textBox3TotalExpense.ReadOnly = true;
            textBox3TotalExpense.Size = new Size(100, 22);
            textBox3TotalExpense.TabIndex = 19;
            textBox3TotalExpense.Text = "#######";
            // 
            // dataGridView3
            // 
            dataGridView3.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridView3.BackgroundColor = SystemColors.Control;
            dataGridView3.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView3.Location = new Point(11, 7);
            dataGridView3.Margin = new Padding(4);
            dataGridView3.Name = "dataGridView3";
            dataGridView3.ReadOnly = true;
            dataGridView3.Size = new Size(860, 232);
            dataGridView3.TabIndex = 18;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(342, 246);
            label11.Name = "label11";
            label11.Size = new Size(120, 21);
            label11.TabIndex = 24;
            label11.Text = "Total Expenses:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(11, 246);
            label9.Name = "label9";
            label9.Size = new Size(49, 21);
            label9.TabIndex = 22;
            label9.Text = "Total:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(147, 246);
            label10.Name = "label10";
            label10.Size = new Size(108, 21);
            label10.TabIndex = 23;
            label10.Text = "Total Income:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(942, 647);
            Controls.Add(groupBox2);
            Controls.Add(ButtonDelete);
            Controls.Add(label2);
            Controls.Add(btnEdit);
            Controls.Add(comboBoxFilter);
            Controls.Add(btnFilter);
            Controls.Add(groupBox3);
            Controls.Add(label1);
            Controls.Add(tabControlWindow);
            Controls.Add(groupBox1);
            Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Zaldy Financial Management System ";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridTransact).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            tabControlWindow.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).EndInit();
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
        private TextBox textBox1TotalExpense;
        private Button btnEdit;
        private TextBox textBoxSearch;
        private Button btnFilter;
        private ComboBox comboBoxFilter;
        private Button btnSearch;
        private TextBox textBox1TotalIncome;
        private TextBox textBox1Total;
        private Label label3;
        private Label label4;
        private Label label5;
        private TabControl tabControlWindow;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private DataGridView dataGridView2;
        private TextBox textBox2Total;
        private Label label8;
        private Label label6;
        private TextBox textBox2TotalIncome;
        private Label label7;
        private TextBox textBox2TotalExpense;
        private DataGridView dataGridView3;
        private TextBox textBox3Total;
        private Label label11;
        private Label label9;
        private TextBox textBox3TotalIncome;
        private Label label10;
        private TextBox textBox3TotalExpense;
    }
}
