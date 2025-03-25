using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace WinFormsApp1SQL
{
    public abstract class DatabaseOperationsBase

    {
        // Common connection string or other shared fields.
        // Cannot be altered by derived classes.
        protected string connectionString = UIEventHandlers.m_FbConnectionString;

        // Virtual method for adding a record.
        public virtual void AddRecord(string activity, decimal amount, DateTime transactionDate, string transactionType)
        {
            using (FbConnection conn = new FbConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO FinancialTransactions (ActivityType, Amount, TransactionDate, TransactionType) " +
                               "VALUES (@ActivityType, @Amount, @TransactionDate, @TransactionType)";
                using (FbCommand cmd = new FbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ActivityType", activity);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@TransactionDate", transactionDate);
                    cmd.Parameters.AddWithValue("@TransactionType", transactionType);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Abstract methods to force derived classes to provide their own implementation.
        public abstract bool DeleteRecord(int id, Form form, TabControl tabControl, ComboBox comboBoxFilter, string itemToDelete);
        public abstract void UpdateRecord(int id, string activity, decimal amount, DateTime transactionDate, string transactionType);
        public abstract DataTable SelectRecords(string filter = null);
        public abstract void BackupFirebirdDatabase(DataGridView grid);
        public abstract void RestoreFirebirdDatabase();
    }
    public class FinancialTransactionsOperations : DatabaseOperationsBase
    {
        bool success = false;

        public override bool DeleteRecord(int id, Form form, TabControl tabControl, ComboBox comboBoxFilter, string itemToDelete)
        {
            using (FbConnection conn = new FbConnection(connectionString))
            {
                conn.Open();
                using (FbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Build and execute the DELETE query within the transaction.
                        string query = "DELETE FROM FinancialTransactions WHERE TransactionID = @TransactionID";
                        using (FbCommand cmd = new FbCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@TransactionID", id);
                            cmd.ExecuteNonQuery();
                        }

                        // Ask for final confirmation before finalizing the deletion.
                        string _itemToDelete = "\n\nRecord: " + itemToDelete;
                        DialogResult finalConfirm = MessageBox.Show(
                            "Do you want to finalize the deletion of " + _itemToDelete + "?",
                            "Final Confirmation",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (finalConfirm == DialogResult.Yes)
                        {
                            transaction.Commit(); // Finalize the deletion.
                            success = true;
                        }
                        else
                        {
                            transaction.Rollback(); // Undo the deletion.
                            success = false;
                            MessageBox.Show("Deletion was canceled.", "Canceled",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        try { transaction.Rollback(); } catch { }
                        MessageBox.Show("Error deleting record: " + ex.Message, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        success = false;
                    }
                }
            }
            return success;
        }

        public override void UpdateRecord(int id, string activity, decimal amount, DateTime transactionDate, string transactionType)
        {
            using (FbConnection conn = new FbConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE FinancialTransactions SET ActivityType = @ActivityType, Amount = @Amount, " +
                               "TransactionDate = @TransactionDate, TransactionType = @TransactionType " +
                               "WHERE TransactionID = @ID";
                using (FbCommand cmd = new FbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ActivityType", activity);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@TransactionDate", transactionDate);
                    cmd.Parameters.AddWithValue("@TransactionType", transactionType);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override DataTable SelectRecords(string filter = null)
        {
            DataTable dt = new DataTable();
            using (FbConnection conn = new FbConnection(connectionString))
            {
                conn.Open();
                string query;
                if (string.IsNullOrEmpty(filter))
                {
                    query = "SELECT TransactionID, ActivityType, Amount, TransactionDate, TransactionType FROM FinancialTransactions";
                }
                else
                {
                    query = "SELECT TransactionID, ActivityType, Amount, TransactionDate, TransactionType FROM FinancialTransactions " +
                            "WHERE ActivityType LIKE @filter";
                }
                using (FbCommand cmd = new FbCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        cmd.Parameters.AddWithValue("@filter", "%" + filter + "%");
                    }
                    FbDataAdapter adapter = new FbDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
        public override void BackupFirebirdDatabase(DataGridView grid)
        {
            // Let the user choose where to save the backup file.
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Backup Files (*.fbk)|*.fbk|All Files (*.*)|*.*",
                Title = "Select Backup File Destination"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string backupFile = saveFileDialog.FileName;

                // Prompt the user for Firebird DB credentials.
                string user = Interaction.InputBox("Please enter your Firebird username:",
                                                     "Database Credentials", "SYSDBA");
                string password = Interaction.InputBox("Please enter your Firebird password:",
                                                         "Database Credentials", "masterkey");

                // Get connection parameters.
                // You can retrieve these from your connection string or configuration.
                //string user = UIEventHandlers.m_firebirdUser;       // Replace with your Firebird username
                //string password = UIEventHandlers.m_firebirdPassword; // Replace with your Firebird password
                string database = UIEventHandlers.m_databasePath;     // Replace with the path to your database

                // Build the gbak command for backup.
                // The -B switch indicates a backup operation.
                //string user = "SYSDBA";
                //string password = "masterkey";
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = UIEventHandlers.m_backupUtilityPath, // Ensure gbak.exe is accessible (in PATH or provide full path)
                    Arguments = $"-B -USER {user} -PASSWORD {password} \"{database}\" \"{backupFile}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                try
                {
                    using (Process proc = Process.Start(psi))
                    {
                        proc.WaitForExit();

                        // Check the exit code to determine if the backup succeeded.
                        if (proc.ExitCode == 0)
                        {
                            MessageBox.Show("Backup completed successfully.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Backup failed with exit code {proc.ExitCode}.", "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Backup failed: " + ex.Message, "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Export
            // Open a SaveFileDialog to choose the CSV file destination.
            using (SaveFileDialog exportFileDialog = new SaveFileDialog())
            {
                exportFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                exportFileDialog.Title = "Export Financial Transactions to CSV";

                if (exportFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportDataGridViewToCSV(grid, exportFileDialog.FileName);
                        MessageBox.Show("Export successful.", "Export to CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Export failed: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void ExportDataGridViewToCSV(DataGridView grid, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            // Get the column headers
            var headers = grid.Columns.Cast<DataGridViewColumn>();
            sb.AppendLine(string.Join(",", headers.Select(column => $"\"{column.HeaderText}\"")));

            // Loop through the rows
            foreach (DataGridViewRow row in grid.Rows)
            {
                // Skip the new row placeholder if present
                if (row.IsNewRow) continue;

                var cells = row.Cells.Cast<DataGridViewCell>();
                // Escape any quotes in cell values by doubling them
                string[] cellValues = cells.Select(cell =>
                {
                    string cellValue = cell.Value?.ToString() ?? string.Empty;
                    cellValue = cellValue.Replace("\"", "\"\"");
                    return $"\"{cellValue}\"";
                }).ToArray();
                sb.AppendLine(string.Join(",", cellValues));
            }

            // Write the CSV data to the specified file
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
        public override void RestoreFirebirdDatabase()
        {
            // Ask the user for the backup file to restore from.
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Backup Files (*.fbk)|*.fbk|All Files (*.*)|*.*",
                Title = "Select Backup File to Restore"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string backupFile = openFileDialog.FileName;

                // Ask the user for the destination of the restored database.
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Firebird Database Files (*.fdb)|*.fdb|All Files (*.*)|*.*",
                    Title = "Select Destination for Restored Database"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string restoredDatabaseFile = saveFileDialog.FileName;

                    // Prompt the user for Firebird DB credentials.
                    string user = Interaction.InputBox("Please enter your Firebird username:",
                                                         "Database Credentials", "SYSDBA");
                    string password = Interaction.InputBox("Please enter your Firebird password:",
                                                             "Database Credentials", "masterkey");

                    // Build the gbak command for restore.
                    // -c indicates the create/restore operation.
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = UIEventHandlers.m_backupUtilityPath, // Ensure gbak.exe path is correct
                        Arguments = $"-c -USER {user} -PASSWORD {password} \"{backupFile}\" \"{restoredDatabaseFile}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    try
                    {
                        using (Process proc = Process.Start(psi))
                        {
                            proc.WaitForExit();

                            // Check the exit code for success.
                            if (proc.ExitCode == 0)
                            {
                                MessageBox.Show("Database restored successfully.",
                                                "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show($"Restore failed with exit code {proc.ExitCode}.",
                                                "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Restore failed: " + ex.Message,
                                        "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
