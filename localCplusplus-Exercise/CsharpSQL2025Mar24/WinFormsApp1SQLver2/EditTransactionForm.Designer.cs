namespace WinFormsApp1SQL
{
    partial class EditTransactionForm
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
            dataGridEditTransact = new DataGridView();
            btnSave = new Button();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridEditTransact).BeginInit();
            SuspendLayout();
            // 
            // dataGridEditTransact
            // 
            dataGridEditTransact.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridEditTransact.BackgroundColor = SystemColors.Control;
            dataGridEditTransact.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridEditTransact.Cursor = Cursors.IBeam;
            dataGridEditTransact.Location = new Point(35, 13);
            dataGridEditTransact.Margin = new Padding(4);
            dataGridEditTransact.Name = "dataGridEditTransact";
            dataGridEditTransact.Size = new Size(845, 50);
            dataGridEditTransact.TabIndex = 0;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(35, 70);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(192, 44);
            btnSave.TabIndex = 1;
            btnSave.Text = "SAVE";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // button1
            // 
            button1.Location = new Point(688, 70);
            button1.Name = "button1";
            button1.Size = new Size(192, 44);
            button1.TabIndex = 2;
            button1.Text = "CLOSE";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // EditTransactionForm
            // 
            AcceptButton = btnSave;
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 121);
            Controls.Add(button1);
            Controls.Add(btnSave);
            Controls.Add(dataGridEditTransact);
            Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditTransactionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EditTransactionForm";
            Enter += btnSave_Click;
            ((System.ComponentModel.ISupportInitialize)dataGridEditTransact).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridEditTransact;
        private Button btnSave;
        private Button button1;
    }
}