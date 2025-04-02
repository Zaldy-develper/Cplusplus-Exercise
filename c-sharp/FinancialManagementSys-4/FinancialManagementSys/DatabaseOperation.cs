using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinFormsApp1SQL
{
    // ---------------------------
    // Core Database Operations
    // ---------------------------
    public interface IDatabaseOperations
    {
        void AddRecord(string activity, decimal amount, DateTime transactionDate, string transactionType,
                      string budgetGroup, string paymentMethod, string payee, string recurrence,
                      string currency, string description);
        bool DeleteRecord(int id);
        void UpdateRecord(int id, string activity, decimal amount, DateTime transactionDate, string transactionType,
                            string budgetGroup, string paymentMethod, string payee, string recurrence, string currency, string description);
        DataTable SelectRecords(string filter = null);

        // Methods for getting distinct values from the database.
        List<string> GetDistinctBudgetGroups(string budgetgroup);
        List<string> GetDistinctPaymentMethod(string paymentmethod);
        List<string> GetDistinctPayee(string payee);
        List<string> GetDistinctCurrency(string currency);
    }

    public class FirebirdDatabaseOperations : IDatabaseOperations
    {
        private readonly string _connectionString;

        public FirebirdDatabaseOperations(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddRecord(string activity, decimal amount, DateTime transactionDate, string transactionType,
                      string budgetGroup, string paymentMethod, string payee, string recurrence,
                      string currency, string description)
        {
            using (FbConnection conn = new FbConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO FinancialTransactions (ActivityType, Amount, TransactionDate, TransactionType, " +
                       "BudgetGroup, PaymentMethod, Payee, Currency, RecurrenceIndicator, Description) " +
                       "VALUES (@ActivityType, @Amount, @TransactionDate, @TransactionType, " +
                       "@BudgetGroup, @PaymentMethod, @Payee, @Currency, @RecurrenceIndicator, @Description)";
                using (FbCommand cmd = new FbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ActivityType", activity);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@TransactionDate", transactionDate);
                    cmd.Parameters.AddWithValue("@TransactionType", transactionType);
                    cmd.Parameters.AddWithValue("@BudgetGroup", budgetGroup);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@Payee", payee);
                    cmd.Parameters.AddWithValue("@Currency", currency);
                    cmd.Parameters.AddWithValue("@RecurrenceIndicator", recurrence);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool DeleteRecord(int id)
        {
            bool success = false;
            using (FbConnection conn = new FbConnection(_connectionString))
            {
                conn.Open();
                using (FbTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = "DELETE FROM FinancialTransactions WHERE TransactionID = @TransactionID";
                        using (FbCommand cmd = new FbCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@TransactionID", id);
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        success = true;
                    }
                    catch (Exception)
                    {
                        try { transaction.Rollback(); } catch { }
                        success = false;
                    }
                }
            }
            return success;
        }

        public void UpdateRecord(int id, string activity, decimal amount, DateTime transactionDate, string transactionType,
                            string budgetGroup, string paymentMethod, string payee, string recurrence, string currency, string description)
        {
            using (FbConnection conn = new FbConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE FinancialTransactions SET " +
                               "ActivityType = @ActivityType, " +
                               "Amount = @Amount, " +
                               "TransactionDate = @TransactionDate, " +
                               "TransactionType = @TransactionType, " +
                               "BudgetGroup = @BudgetGroup, " +
                               "PaymentMethod = @PaymentMethod, " +
                               "Payee = @Payee, " +
                               "RecurrenceIndicator = @RecurrenceIndicator, " +
                               "Currency = @Currency, " +
                               "Description = @Description " +
                               "WHERE TransactionID = @ID";
                using (FbCommand cmd = new FbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ActivityType", activity);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@TransactionDate", transactionDate);
                    cmd.Parameters.AddWithValue("@TransactionType", transactionType);
                    cmd.Parameters.AddWithValue("@BudgetGroup", budgetGroup);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@Payee", payee);
                    cmd.Parameters.AddWithValue("@RecurrenceIndicator", recurrence);
                    cmd.Parameters.AddWithValue("@Currency", currency);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable SelectRecords(string filter = null)
        {
            DataTable dt = new DataTable();
            using (FbConnection conn = new FbConnection(_connectionString))
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

        public List<string> GetDistinctBudgetGroups(string budgetgroup)
        {
            List<string> budgetGroups = new List<string>();

            // Ensure budgetgroup is not null.
            if (budgetgroup == null)
                budgetgroup = string.Empty;

            using (FbConnection conn = new FbConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT DISTINCT BudgetGroup FROM FinancialTransactions WHERE BudgetGroup LIKE @BudgetGroup";
                using (FbCommand cmd = new FbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BudgetGroup", "%" + budgetgroup + "%");
                    using (FbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            budgetGroups.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return budgetGroups;
        }
        public List<string> GetDistinctPaymentMethod(string paymentmethod)
        {
            List<string> paymentMethods = new List<string>();

            // Ensure budgetgroup is not null.
            if (paymentmethod == null)
                paymentmethod = string.Empty;

            using (FbConnection conn = new FbConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT DISTINCT PaymentMethod FROM FinancialTransactions WHERE PaymentMethod LIKE @PaymentMethod";
                using (FbCommand cmd = new FbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PaymentMethod", "%" + paymentmethod + "%");
                    using (FbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            paymentMethods.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return paymentMethods;
        }
        public List<string> GetDistinctPayee(string payee)
        {
            List<string> payeeMethods = new List<string>();

            // Ensure budgetgroup is not null.
            if (payee == null)
                payee = string.Empty;

            using (FbConnection conn = new FbConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT DISTINCT Payee FROM FinancialTransactions WHERE Payee LIKE @Payee";
                using (FbCommand cmd = new FbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Payee", "%" + payee + "%");
                    using (FbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payeeMethods.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return payeeMethods;
        }
        public List<string> GetDistinctCurrency(string currency)
        {
            List<string> currencyMethods = new List<string>();

            // Ensure budgetgroup is not null.
            if (currency == null)
                currency = string.Empty;

            using (FbConnection conn = new FbConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT DISTINCT Currency FROM FinancialTransactions WHERE Currency LIKE @Currency";
                using (FbCommand cmd = new FbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Currency", "%" + currency + "%");
                    using (FbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            currencyMethods.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return currencyMethods;
        }
    }

    // ---------------------------
    // Backup and Restore Operations
    // ---------------------------
    public interface IBackupRestoreOperations
    {
        void BackupDatabase(string user, string password, string backupFilePath);
        void RestoreDatabase(string backupFilePath, string user, string password, string restoredDatabaseFile);
    }

    public class FirebirdBackupRestoreOperations : IBackupRestoreOperations
    {
        private readonly string _backupUtilityPath;
        private readonly string _database;

        public FirebirdBackupRestoreOperations(string backupUtilityPath, string database)
        {
            _backupUtilityPath = backupUtilityPath;
            _database = database;
        }

        public void BackupDatabase(string user, string password, string backupFilePath)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = _backupUtilityPath,
                Arguments = $"-B -USER {user} -PASSWORD {password} \"{_database}\" \"{backupFilePath}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using (Process proc = Process.Start(psi))
            {
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    throw new Exception($"Backup failed with exit code {proc.ExitCode}");
                }
            }
        }

        public void RestoreDatabase(string backupFilePath, string user, string password, string restoredDatabaseFile)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = _backupUtilityPath,
                Arguments = $"-c -USER {user} -PASSWORD {password} \"{backupFilePath}\" \"{restoredDatabaseFile}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using (Process proc = Process.Start(psi))
            {
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    throw new Exception($"Restore failed with exit code {proc.ExitCode}");
                }
            }
        }
    }

    // ---------------------------
    // CSV Export Service
    // ---------------------------
    public interface IExportService
    {
        void ExportDataTableToCSV(DataTable dataTable, string filePath);
    }

    public class CsvExportService : IExportService
    {
        public void ExportDataTableToCSV(DataTable dataTable, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            // Write column headers.
            IEnumerable<string> columnNames = dataTable.Columns
                                                       .Cast<DataColumn>()
                                                       .Select(column => $"\"{column.ColumnName}\"");
            sb.AppendLine(string.Join(",", columnNames));

            // Write rows.
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray
                                                .Select(field => $"\"{field.ToString().Replace("\"", "\"\"")}\"");
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }

    // ---------------------------
    // Financial Transaction Service
    // ---------------------------
    // This higher-level service class demonstrates dependency injection with interfaces,
    // similar to public Processor(ILogger logger).
    public class FinancialTransactionService
    {
        private readonly IDatabaseOperations _databaseOperations;
        private readonly IBackupRestoreOperations _backupRestoreOperations;
        private readonly IExportService _exportService;

        public FinancialTransactionService(
            IDatabaseOperations databaseOperations,
            IBackupRestoreOperations backupRestoreOperations,
            IExportService exportService)
        {
            _databaseOperations = databaseOperations;
            _backupRestoreOperations = backupRestoreOperations;
            _exportService = exportService;
        }

        // Add here all the methods that will be used in the  FinancialTransactionService class which has contract
        // with the IDatabaseOperations, IBackupRestoreOperations and IExportService interfaces.

        public void AddTransaction(string activity, decimal amount, DateTime transactionDate, string transactionType,
                           string budgetGroup, string paymentMethod, string payee, string recurrence,
                           string currency, string description)
        {
            _databaseOperations.AddRecord(activity, amount, transactionDate, transactionType,
                                  budgetGroup, paymentMethod, payee, recurrence, currency, description);
        }

        public bool DeleteTransaction(int id)
        {
            return _databaseOperations.DeleteRecord(id);
        }

        public void UpdateTransaction(int id, string activity, decimal amount, DateTime transactionDate, string transactionType,
                            string budgetGroup, string paymentMethod, string payee, string recurrence, string currency, string description)
        {
            _databaseOperations.UpdateRecord(id, activity, amount, transactionDate, transactionType,
                            budgetGroup, paymentMethod, payee, recurrence, currency, description);
        }

        public DataTable GetTransactions(string filter = null)
        {
            return _databaseOperations.SelectRecords(filter);
        }

        public List<string> GetDistinctBudgetGroups(string budgetgroup)
        {
            return _databaseOperations.GetDistinctBudgetGroups(budgetgroup);
        }
        public List<string> GetDistinctPaymentMethod(string paymentmethod)
        {
            return _databaseOperations.GetDistinctPaymentMethod(paymentmethod);
        }
        public List<string> GetDistinctPayee(string payee)
        {
            return _databaseOperations.GetDistinctPayee(payee);
        }
        public List<string> GetDistinctCurrency(string currency)
        {
            return _databaseOperations.GetDistinctCurrency(currency);
        }
        public void Backup(string user, string password, string database)
        {
            _backupRestoreOperations.BackupDatabase(user, password, database);
        }

        public void Restore(string backupFilePath, string user, string password, string restoredDatabaseFile)
        {
            _backupRestoreOperations.RestoreDatabase(backupFilePath, user, password, restoredDatabaseFile);
        }

        public void ExportTransactions(DataTable transactions, string filePath)
        {
            _exportService.ExportDataTableToCSV(transactions, filePath);
        }
    }

    // This class will be used repeatedly on other Forms
    public static class ServiceFactory
    {
        public static FinancialTransactionService CreateFinancialTransactionService()
        {
            // Create instances of the concrete implementations for the interfaces.
            IDatabaseOperations dbOps = new FirebirdDatabaseOperations(UIEventHandlers.m_FbConnectionString);
            IBackupRestoreOperations backupRestoreOps = new FirebirdBackupRestoreOperations(UIEventHandlers.m_backupUtilityPath, UIEventHandlers.m_databasePath);
            IExportService exportService = new CsvExportService();

            // Inject these instances into the FinancialTransactionService.
            return new FinancialTransactionService(dbOps, backupRestoreOps, exportService);
        }
    }

}
