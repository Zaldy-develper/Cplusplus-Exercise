using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1SQL
{
    // UIEventHandlers class Member and Construtor here
    partial class UIEventHandlers
    {
        // Path to the INI file located in the same folder as the executable.
        private static string m_iniFilePath { get; set; }
        // Connection string to Firebird database
        public static string m_firebirdUser { get; private set; }
        public static string m_firebirdPassword { get; private set; }
        public static string m_serverIp { get; private set; }
        public static string m_databasePath { get; private set; }
        public static string m_backupUtilityPath { get; private set; }
        public static string m_FbConnectionString { get; private set; }

        public UIEventHandlers()
        // Constructor logic here
        {
            m_iniFilePath = Path.Combine(Application.StartupPath, "settings.ini");
        }
        public static void SetFbConnectionString(string connectionString)
        {
            m_FbConnectionString = connectionString;
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
    // UIEventHandlers class Method here
    partial class UIEventHandlers
    {
        // Loads saved settings from the INI file (if it exists) and populates the input controls.
        public static void LoadIniSettings(TextBox textBoxUser,
                                            TextBox textBoxIPAdd,
                                            TextBox textBoxPath,
                                            TextBox textBoxBackupUtility)
        {
            if (File.Exists(m_iniFilePath))
            {
                textBoxUser.Text = IniFile.Read("Credentials", "User", m_iniFilePath);
                textBoxIPAdd.Text = IniFile.Read("Credentials", "ServerIp", m_iniFilePath);
                textBoxPath.Text = IniFile.Read("Credentials", "DatabasePath", m_iniFilePath);
                textBoxBackupUtility.Text = IniFile.Read("Credentials", "BackupUtilityPath", m_iniFilePath);
                // Note: The password is intentionally not saved.
            }
        }
        public static void Login(Form form, TextBox textBoxUser,
                                            TextBox textBoxIPAdd,
                                            TextBox textBoxPath,
                                            TextBox textBoxBackupUtility,
                                            TextBox textBoxpassword)
        {
            // Retrieve values from input controls.
            string firebirdUser = textBoxUser.Text;
            string firebirdPassword = textBoxpassword.Text;
            string serverIp = textBoxIPAdd.Text; // e.g., "localhost"
            string databasePath = textBoxPath.Text; // e.g., "C:\\Databases\\FINANCIAL.FDB"
            string backupUtilityPath = textBoxBackupUtility.Text;

            // Optional: Validate credentials (For the meantime use a simple check).
            bool isConnectionAccepted = true;
            do
            {
                // Build the connection string dynamically from user input.
                string fbConnectionString = $"User={firebirdUser};Password={firebirdPassword};Database={databasePath};DataSource={serverIp};Port=3050;Dialect=3;";

                // Attempt to open a connection to the Firebird database.
                try
                {
                    using (FbConnection connection = new FbConnection(fbConnectionString))
                    {
                        connection.Open();

                        // Set the property to pass the connection string to Program.cs
                        SetFbConnectionString(fbConnectionString);
                        SetFirebirdUser(firebirdUser);
                        SetFirebirdPassword(firebirdPassword);
                        SetServerIp(serverIp);
                        SetDatabasePath(databasePath);
                        SetBackupUtilityPath(backupUtilityPath);

                        // Save credentials (excluding the password) to the INI file.
                        IniFile.Write("Credentials", "User", firebirdUser, m_iniFilePath);
                        IniFile.Write("Credentials", "ServerIp", serverIp, m_iniFilePath);
                        IniFile.Write("Credentials", "DatabasePath", databasePath, m_iniFilePath);
                        IniFile.Write("Credentials", "BackupUtilityPath", backupUtilityPath, m_iniFilePath);

                        // Set the DialogResult to OK when login is successful
                        form.DialogResult = DialogResult.OK;
                        form.Close();
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
}
