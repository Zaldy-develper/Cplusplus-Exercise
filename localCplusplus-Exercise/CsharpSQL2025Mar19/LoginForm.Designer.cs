namespace WinFormsApp1SQL
{
    partial class LoginForm
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            textBoxUser = new TextBox();
            textBoxPassword = new TextBox();
            textBoxIPAdd = new TextBox();
            textBoxPath = new TextBox();
            btnLogin = new Button();
            textBoxBackupUtility = new TextBox();
            label5 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 22);
            label1.Name = "label1";
            label1.Size = new Size(83, 21);
            label1.TabIndex = 0;
            label1.Text = "Username";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(285, 22);
            label2.Name = "label2";
            label2.Size = new Size(79, 21);
            label2.TabIndex = 1;
            label2.Text = "Password";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(23, 78);
            label3.Name = "label3";
            label3.Size = new Size(163, 21);
            label3.TabIndex = 2;
            label3.Text = "Database IP Address:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(23, 134);
            label4.Name = "label4";
            label4.Size = new Size(46, 21);
            label4.TabIndex = 3;
            label4.Text = "Path:";
            // 
            // textBoxUser
            // 
            textBoxUser.Cursor = Cursors.IBeam;
            textBoxUser.Location = new Point(23, 46);
            textBoxUser.Name = "textBoxUser";
            textBoxUser.PlaceholderText = "Firebird Username";
            textBoxUser.Size = new Size(244, 29);
            textBoxUser.TabIndex = 4;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Cursor = Cursors.IBeam;
            textBoxPassword.Location = new Point(285, 46);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.PasswordChar = '*';
            textBoxPassword.PlaceholderText = "Type your password here";
            textBoxPassword.Size = new Size(244, 29);
            textBoxPassword.TabIndex = 5;
            // 
            // textBoxIPAdd
            // 
            textBoxIPAdd.Cursor = Cursors.IBeam;
            textBoxIPAdd.Location = new Point(23, 102);
            textBoxIPAdd.Name = "textBoxIPAdd";
            textBoxIPAdd.PlaceholderText = "localhost (if Login in Server)";
            textBoxIPAdd.Size = new Size(244, 29);
            textBoxIPAdd.TabIndex = 6;
            // 
            // textBoxPath
            // 
            textBoxPath.Cursor = Cursors.IBeam;
            textBoxPath.Location = new Point(23, 158);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.PlaceholderText = "Path location of FINANCIALDB.FRB";
            textBoxPath.Size = new Size(506, 29);
            textBoxPath.TabIndex = 7;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.SteelBlue;
            btnLogin.ForeColor = Color.WhiteSmoke;
            btnLogin.Location = new Point(23, 193);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(506, 37);
            btnLogin.TabIndex = 8;
            btnLogin.Text = "Log-in";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // textBoxBackupUtility
            // 
            textBoxBackupUtility.Cursor = Cursors.IBeam;
            textBoxBackupUtility.Location = new Point(285, 102);
            textBoxBackupUtility.Name = "textBoxBackupUtility";
            textBoxBackupUtility.PlaceholderText = "C:\\Program Files\\Firebird\\Firebird_4_0\\gbak.exe";
            textBoxBackupUtility.Size = new Size(244, 29);
            textBoxBackupUtility.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(285, 78);
            label5.Name = "label5";
            label5.Size = new Size(162, 21);
            label5.TabIndex = 10;
            label5.Text = "Back Utility Location:";
            // 
            // LoginForm
            // 
            AcceptButton = btnLogin;
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(552, 243);
            Controls.Add(label5);
            Controls.Add(textBoxBackupUtility);
            Controls.Add(btnLogin);
            Controls.Add(textBoxPath);
            Controls.Add(textBoxIPAdd);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxUser);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Zaldy Login Form";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox textBoxUser;
        private TextBox textBoxPassword;
        private TextBox textBoxIPAdd;
        private TextBox textBoxPath;
        private Button btnLogin;
        private TextBox textBoxBackupUtility;
        private Label label5;
    }
}