using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Abstract methods force derived classes to provide their own implementation.
        public abstract bool DeleteRecord(int id, Form form, TabControl tabControl, ComboBox comboBoxFilter, string itemToDelete);
        public abstract void UpdateRecord(int id, string activity, decimal amount, DateTime transactionDate, string transactionType);
        public abstract DataTable SelectRecords(string filter = null);
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
    }
}
