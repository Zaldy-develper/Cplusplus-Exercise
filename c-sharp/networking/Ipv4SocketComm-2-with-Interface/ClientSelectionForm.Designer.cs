namespace Ipv4SocketCommunication
{
    partial class ClientSelectionForm
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
            clientListBox = new ListBox();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // clientListBox
            // 
            clientListBox.FormattingEnabled = true;
            clientListBox.ItemHeight = 25;
            clientListBox.Location = new Point(14, 20);
            clientListBox.Margin = new Padding(5);
            clientListBox.Name = "clientListBox";
            clientListBox.Size = new Size(497, 179);
            clientListBox.TabIndex = 0;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(14, 220);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(163, 44);
            btnOK.TabIndex = 1;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(348, 220);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(163, 44);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "CANCEL";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // ClientSelectionForm
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(525, 286);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(clientListBox);
            Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(5);
            Name = "ClientSelectionForm";
            Text = "ClientSelectionForm";
            ResumeLayout(false);
        }

        #endregion
        private Button btnOK;
        private Button btnCancel;
        public ListBox clientListBox;
    }
}