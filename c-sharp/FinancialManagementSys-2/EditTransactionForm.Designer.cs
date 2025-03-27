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
            btnClose = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridEditTransact).BeginInit();
            SuspendLayout();
            // 
            // dataGridEditTransact
            // 
            dataGridEditTransact.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridEditTransact.BackgroundColor = SystemColors.Control;
            dataGridEditTransact.BorderStyle = BorderStyle.Fixed3D;
            dataGridEditTransact.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridEditTransact.Cursor = Cursors.IBeam;
            dataGridEditTransact.Location = new Point(13, 13);
            dataGridEditTransact.Margin = new Padding(4);
            dataGridEditTransact.Name = "dataGridEditTransact";
            dataGridEditTransact.Size = new Size(1110, 55);
            dataGridEditTransact.TabIndex = 0;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(12, 79);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(192, 44);
            btnSave.TabIndex = 1;
            btnSave.Text = "SAVE";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(931, 79);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(192, 44);
            btnClose.TabIndex = 2;
            btnClose.Text = "CLOSE";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // EditTransactionForm
            // 
            AcceptButton = btnSave;
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1136, 131);
            Controls.Add(btnClose);
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
        private Button btnClose;
    }
}