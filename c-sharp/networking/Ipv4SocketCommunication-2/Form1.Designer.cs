namespace Ipv4SocketCommunication
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
            saveFileDialog1 = new SaveFileDialog();
            ipAddMaskText = new MaskedTextBox();
            portText = new TextBox();
            label1 = new Label();
            label2 = new Label();
            savePathText = new TextBox();
            label3 = new Label();
            btnSavePath = new Button();
            groupBox1 = new GroupBox();
            statusText = new TextBox();
            groupBox2 = new GroupBox();
            btnStart = new Button();
            btnStop = new Button();
            serverText = new TextBox();
            button4 = new Button();
            groupBox3 = new GroupBox();
            messageHistoryRichText = new RichTextBox();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // ipAddMaskText
            // 
            ipAddMaskText.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ipAddMaskText.Location = new Point(135, 34);
            ipAddMaskText.Mask = "000.000.0.000";
            ipAddMaskText.Name = "ipAddMaskText";
            ipAddMaskText.Size = new Size(126, 32);
            ipAddMaskText.TabIndex = 0;
            ipAddMaskText.TextAlign = HorizontalAlignment.Center;
            // 
            // portText
            // 
            portText.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            portText.Location = new Point(388, 34);
            portText.Name = "portText";
            portText.Size = new Size(55, 32);
            portText.TabIndex = 1;
            portText.Text = "1024";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(27, 37);
            label1.Name = "label1";
            label1.Size = new Size(107, 25);
            label1.TabIndex = 2;
            label1.Text = "IP Address:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(260, 37);
            label2.Name = "label2";
            label2.Size = new Size(130, 25);
            label2.TabIndex = 3;
            label2.Text = "Port Number:";
            // 
            // savePathText
            // 
            savePathText.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            savePathText.ForeColor = Color.DimGray;
            savePathText.Location = new Point(135, 72);
            savePathText.Name = "savePathText";
            savePathText.Size = new Size(308, 32);
            savePathText.TabIndex = 4;
            savePathText.Text = "Location...";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(27, 75);
            label3.Name = "label3";
            label3.Size = new Size(100, 25);
            label3.TabIndex = 5;
            label3.Text = "Save Path:";
            // 
            // btnSavePath
            // 
            btnSavePath.FlatStyle = FlatStyle.Popup;
            btnSavePath.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSavePath.Location = new Point(406, 72);
            btnSavePath.Name = "btnSavePath";
            btnSavePath.Size = new Size(37, 32);
            btnSavePath.TabIndex = 6;
            btnSavePath.Text = "...";
            btnSavePath.UseVisualStyleBackColor = true;
            btnSavePath.Click += btnSavePath_Click;
            // 
            // groupBox1
            // 
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(440, 112);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Server Configuration";
            // 
            // statusText
            // 
            statusText.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            statusText.ForeColor = Color.DimGray;
            statusText.Location = new Point(135, 146);
            statusText.Multiline = true;
            statusText.Name = "statusText";
            statusText.ScrollBars = ScrollBars.Both;
            statusText.Size = new Size(310, 109);
            statusText.TabIndex = 8;
            statusText.Text = "Suspend.";
            // 
            // groupBox2
            // 
            groupBox2.Location = new Point(125, 130);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(327, 131);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Status";
            // 
            // btnStart
            // 
            btnStart.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStart.Location = new Point(12, 138);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(107, 35);
            btnStart.TabIndex = 10;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStop.Location = new Point(12, 226);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(107, 35);
            btnStop.TabIndex = 11;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // serverText
            // 
            serverText.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            serverText.ForeColor = Color.DimGray;
            serverText.Location = new Point(12, 278);
            serverText.Name = "serverText";
            serverText.ScrollBars = ScrollBars.Horizontal;
            serverText.Size = new Size(440, 32);
            serverText.TabIndex = 12;
            serverText.Text = "Message...";
            // 
            // button4
            // 
            button4.FlatStyle = FlatStyle.Popup;
            button4.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button4.Location = new Point(377, 278);
            button4.Name = "button4";
            button4.Size = new Size(75, 32);
            button4.TabIndex = 13;
            button4.Text = "Send";
            button4.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(messageHistoryRichText);
            groupBox3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox3.Location = new Point(476, 9);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(312, 302);
            groupBox3.TabIndex = 14;
            groupBox3.TabStop = false;
            groupBox3.Text = "Message History";
            // 
            // messageHistoryRichText
            // 
            messageHistoryRichText.ForeColor = Color.DimGray;
            messageHistoryRichText.Location = new Point(9, 25);
            messageHistoryRichText.Name = "messageHistoryRichText";
            messageHistoryRichText.ScrollBars = RichTextBoxScrollBars.Vertical;
            messageHistoryRichText.Size = new Size(297, 271);
            messageHistoryRichText.TabIndex = 0;
            messageHistoryRichText.Text = "--Empty--";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 323);
            Controls.Add(groupBox3);
            Controls.Add(button4);
            Controls.Add(serverText);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(statusText);
            Controls.Add(btnSavePath);
            Controls.Add(label3);
            Controls.Add(savePathText);
            Controls.Add(label1);
            Controls.Add(portText);
            Controls.Add(ipAddMaskText);
            Controls.Add(groupBox2);
            Controls.Add(label2);
            Controls.Add(groupBox1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ZALDY IPv4 Socket Communication [Server Edition]";
            Load += Form1_Load;
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SaveFileDialog saveFileDialog1;
        private MaskedTextBox ipAddMaskText;
        private TextBox portText;
        private Label label1;
        private Label label2;
        private TextBox savePathText;
        private Label label3;
        private Button btnSavePath;
        private GroupBox groupBox1;
        private TextBox statusText;
        private GroupBox groupBox2;
        private Button btnStart;
        private Button btnStop;
        private TextBox serverText;
        private Button button4;
        private GroupBox groupBox3;
        private RichTextBox messageHistoryRichText;
    }
}
