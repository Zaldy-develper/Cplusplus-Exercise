using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1SQL
{
    public partial class LoginForm : Form
    {
        // Path to the INI file located in the same folder as the executable.
        private readonly string iniFilePath = Path.Combine(Application.StartupPath, "settings.ini");

        public LoginForm()
        {
            InitializeComponent();
            LoadIniSettings();
        }

        /// <summary>
        /// Loads saved settings from the INI file (if it exists) and populates the input controls.
        /// </summary>
        private void LoadIniSettings()
        {
            if (File.Exists(iniFilePath))
            {
                textBoxUser.Text = IniFile.Read("Credentials", "User", iniFilePath);
                textBoxIPAdd.Text = IniFile.Read("Credentials", "ServerIp", iniFilePath);
                textBoxPath.Text = IniFile.Read("Credentials", "DatabasePath", iniFilePath);
                textBoxBackupUtility.Text = IniFile.Read("Credentials", "BackupUtilityPath", iniFilePath);
                // Note: The password is intentionally not saved.
            }
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Retrieve values from input controls.
            string firebirdUser = textBoxUser.Text;
            string firebirdPassword = textBoxPassword.Text;
            string serverIp = textBoxIPAdd.Text; // e.g., "localhost"
            string databasePath = textBoxPath.Text; // e.g., "C:\\Databases\\FINANCIAL.FDB"
            string backupUtilityPath = textBoxBackupUtility.Text;

            // Optional: Validate credentials (for demonstration, we use a simple check).
            bool isConnectionAccepted = true;
            do {
                // Build the connection string dynamically from user input.
                string fbConnectionString = $"User={firebirdUser};Password={firebirdPassword};Database={databasePath};DataSource={serverIp};Port=3050;Dialect=3;";

                // Attempt to open a connection to the Firebird database.
                try
                {
                    using (FbConnection connection = new FbConnection(fbConnectionString))
                    {
                        connection.Open();

                        // Set the property to pass the connection string to Program.cs
                        ConnectionString.SetFbConnectionString(fbConnectionString);
                        ConnectionString.SetFirebirdUser(firebirdUser);
                        ConnectionString.SetFirebirdPassword(firebirdPassword); 
                        ConnectionString.SetServerIp(serverIp);
                        ConnectionString.SetDatabasePath(databasePath);
                        ConnectionString.SetBackupUtilityPath(backupUtilityPath);

                        // Save credentials (excluding the password) to the INI file.
                        IniFile.Write("Credentials", "User", firebirdUser, iniFilePath);
                        IniFile.Write("Credentials", "ServerIp", serverIp, iniFilePath);
                        IniFile.Write("Credentials", "DatabasePath", databasePath, iniFilePath);
                        IniFile.Write("Credentials", "BackupUtilityPath", backupUtilityPath, iniFilePath);

                        // Set the DialogResult to OK when login is successful
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to connect to the database: " + ex.Message,
                                    "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isConnectionAccepted = false;
                }
            } while (isConnectionAccepted == true);
           
        }

    }

    public static class ConnectionString
    {
        // Connection string to Firebird database
        public static string m_firebirdUser { get; private set; }
        public static string m_firebirdPassword { get; private set; }
        public static string m_serverIp { get; private set; }
        public static string m_databasePath { get; private set; }

        public static string m_backupUtilityPath { get; private set; }

        public static string FbConnectionString { get; private set; }
        public static void SetFbConnectionString(string connectionString)
        {
            FbConnectionString = connectionString;
        }

        public static void SetFirebirdUser(string firebirdUser)
        {
            m_firebirdUser = firebirdUser;
        }

        public static void SetFirebirdPassword(string firebirdPassword)
        {
            m_firebirdPassword = firebirdPassword;
        }

        public static void SetServerIp(string serverIp) 
        {
            m_serverIp = serverIp;
        }

        public static void SetDatabasePath(string databasePath)
        { 
            m_databasePath = databasePath; 
        }
      
        public static void SetBackupUtilityPath(string backupUtilityPath)
        {
            m_backupUtilityPath = backupUtilityPath;
        }
    }
}
