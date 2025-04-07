using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using static WinFormsApp1SQL.GridDataViewBaseEventHandler;
using static WinFormsApp1SQL.GridViewHelper;

namespace WinFormsApp1SQL
{
    //===============================
    // Interface
    //===============================
    public interface IFormUIEventHandler
    {
        // Declare the properties needed by the default implementation.
        bool m_IsAddRowInput { get; set; }
        bool m_IsAllowUserAddRow { get; set; }
        bool m_IsAddColNum { get; set; }
        bool m_IsAutoAdjustHeight { get; set; }
        int m_ActivityWidth { get; set; }
        int m_AmountWidth { get; set; }
        int m_TransactionTypeWidth { get; set; }

        // Method signature here.
        void SetupDataGridView(DataGridView grid);
        void SetupWindowTabDataGridView(Form form, TabControl tabControl, ComboBox comboBoxFilter,
                                                        ComboBox comboBoxBudgetGroup, ComboBox comboBoxPaymentMethod,
                                                        ComboBox comboBoxPayee, DateTimePicker dateTimePickerStart, DateTimePicker dateTimePickerEnd);
        bool SetUpEditFormDataGridView(Form form, TabControl tabControl, DataGridView grid, ComboBox comboBoxFilter,
                                                        ComboBox comboBoxBudgetGroup, ComboBox comboBoxPaymentMethod,
                                                        ComboBox comboBoxPayee, DateTimePicker dateTimePickerStart,
                                                        DateTimePicker dateTimePickerEnd);
        bool BackupOperation();
        bool RestoreOperation();
        bool ExportTabletoCSV(DataGridView grid);
    }
    public interface IDataGridEntry
    {
        bool IsFillupComplete(DataGridView grid, DataGridViewRow row, FinancialTransactionRecord record);
        bool IsFillupValid(DataGridView grid, DataGridViewRow row,
                                            FinancialTransactionRecord record = null,
                                            bool isNewEntry = true);
    }
    //===============================
    // Classes
    //===============================
    // Encapsulate transaction details.
    public class FinancialTransactionRecord
    {
        public int TransactionID { get; set; }  // Optional – For Future updates of data
        public int? RowNumber { get; set; }
        public string Activity { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string BudgetGroup { get; set; }
        public string PaymentMethod { get; set; }
        public string Payee { get; set; }
        public string Recurrence { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
    }


    // Abstract Base Class
    abstract class GridDataViewBaseEventHandler : IFormUIEventHandler, IDataGridEntry
    {
        // Declare a field for the higher-level service.
        private readonly FinancialTransactionService _transactionService_FormUI;

        // Fields to store the control references
        protected DataGridView activeDGV;
        protected TextBox textBoxTotal;
        protected TextBox textBoxTotalIncome;
        protected TextBox textBoxTotalExpense;

        // Private fields to hold transaction data.
        private int _transactionID = 0;
        private string _activity = string.Empty;
        private decimal _amount = 0;
        private DateTime _transactionDate = DateTime.MinValue;
        private string _transactionType = string.Empty;
        private string _budgetGroup = string.Empty;
        private string _paymentMethod = string.Empty;
        private string _payee = string.Empty;
        private string _recurrence = string.Empty;
        private string _currency = string.Empty;
        private string _description = string.Empty;

        // Database Credentials
        private string username_db = string.Empty;
        private string password_db = string.Empty;
        private string backupfile_db = string.Empty;
        private string restorefile_db = string.Empty;

        // Transaction Data
        DataTable m_transactionDataTable = null;
        string m_csv_filename = string.Empty;

        // Public read-only properties to expose the transaction data.
        public int TransactionID { get { return _transactionID; } }
        public string Activity { get { return _activity; } }
        public decimal Amount { get { return _amount; } }
        public DateTime TransactionDate { get { return _transactionDate; } }
        public string TransactionType { get { return _transactionType; } }
        public string BudgetGroup { get { return _budgetGroup; } }
        public string PaymentMethod { get { return _paymentMethod; } }
        public string Payee { get { return _payee; } }
        public string Recurrence { get { return _recurrence; } }
        public string Currency { get { return _currency; } }
        public string Description { get { return _description; } }

        // Public read-only properties to expose the log in data.
        public string Username_DB { get { return username_db; } }
        public string Password_DB { get { return password_db; } }
        public string BackupFile_DB { get { return backupfile_db; } }
        public string RestoreFile_DB { get { return restorefile_db; } }
        public DataTable TransactionDataTable { get { return m_transactionDataTable; } }
        public string CSVFilename { get { return m_csv_filename; } }

        // Implement the properties.
        public bool m_IsAddRowInput { get; set; }
        public bool m_IsAllowUserAddRow { get; set; }
        public bool m_IsAddColNum { get; set; }
        public bool m_IsAutoAdjustHeight { get; set; }
        public int m_ActivityWidth { get; set; }
        public int m_AmountWidth { get; set; }
        public int m_DateWidth { get; set; }
        public int m_TransactionTypeWidth { get; set; }
        public int m_BudgetGroupWidth { get; set; }
        public int m_PaymentMethodWidth { get; set; }
        public int m_PayeeWidth { get; set; }
        public int m_RecurrenceWidth { get; set; }
        public int m_CurrencyWidth { get; set; }
        public int m_DescriptionWidth { get; set; }

        // Constructor
        protected GridDataViewBaseEventHandler()
        {
            _transactionService_FormUI = ServiceFactory.CreateFinancialTransactionService();

            m_ActivityWidth = 260;
            m_AmountWidth = 130;  // Default value
            m_DateWidth = 190;
            m_TransactionTypeWidth = 190;
            m_BudgetGroupWidth = 100;
            m_PaymentMethodWidth = 120;
            m_PayeeWidth = 100;
            m_RecurrenceWidth = 80;
            m_CurrencyWidth = 70;
            m_DescriptionWidth = 50;

        }

        // 
        //===============================
        // Methods to implement the interface.
        //===============================
        // Default method implementation for setting up the DataGridView.
        protected FinancialTransactionRecord CreateTransactionRecordFromRow(DataGridViewRow row, bool isNewEntry = false)
        {
            var record = new FinancialTransactionRecord();

            if (!isNewEntry)
            {
                record.TransactionID = Convert.ToInt32(row.Cells["TransactionID"].Value);
                record.RowNumber = Convert.ToInt32(row.Cells["No."].Value);
            }

            record.Activity = row.Cells["ActivityType"].Value?.ToString();
            record.Amount = (row.Cells["Amount"].Value == null || row.Cells["Amount"].Value == DBNull.Value)
                        ? 0m
                        : Convert.ToDecimal(row.Cells["Amount"].Value);
            record.TransactionType = row.Cells["TransactionType"].Value?.ToString();
            record.BudgetGroup = row.Cells["BudgetGroup"].Value?.ToString();
            record.PaymentMethod = row.Cells["PaymentMethod"].Value?.ToString();
            record.Payee = row.Cells["Payee"].Value?.ToString();
            record.TransactionDate = row.Cells["TransactionDate"].Value != null ? Convert.ToDateTime(row.Cells["TransactionDate"].Value) : DateTime.MinValue;
            record.Recurrence = row.Cells["RecurrenceIndicator"].Value?.ToString();
            record.Currency = row.Cells["Currency"].Value?.ToString();
            record.Description = row.Cells["Description"].Value?.ToString();

            return record;
        }

        public virtual void SetupDataGridView(DataGridView grid)
        {
            // Column Count
            int rowCount = 10;

            grid.Columns.Clear();

            if (m_IsAddColNum)
            {
                // 0. Add the row number column.
                DataGridViewTextBoxColumn rowNumberColumn = new DataGridViewTextBoxColumn
                {
                    Name = "RowNumber",
                    HeaderText = "No.",
                    ReadOnly = true,
                    //Width = 50
                    FillWeight = 40
                };
                grid.Columns.Add(rowNumberColumn);
            }

            // 1. Text Column for Activity Type
            DataGridViewTextBoxColumn activityColumn = new DataGridViewTextBoxColumn();
            activityColumn.Name = "ActivityType";
            activityColumn.HeaderText = "Activity";
            activityColumn.ValueType = typeof(string);
            activityColumn.FillWeight = m_ActivityWidth;
            grid.Columns.Add(activityColumn);


            // 2. Numeric Column for Amount (using a TextBox column with numeric ValueType)
            DataGridViewTextBoxColumn amountColumn = new DataGridViewTextBoxColumn();
            amountColumn.Name = "Amount";
            amountColumn.HeaderText = "Amount";
            amountColumn.ValueType = typeof(decimal);
            amountColumn.DefaultCellStyle.Format = "#,##0.00"; // Formats as integer with thousand separators
            //amountColumn.Width = m_AmountWidth;
            amountColumn.FillWeight = m_AmountWidth;
            grid.Columns.Add(amountColumn);

            // 3. ComboBox Column for Transaction Type
            DataGridViewComboBoxColumn transactionTypeColumn = new DataGridViewComboBoxColumn
            {
                Name = "TransactionType",
                HeaderText = "Transaction",
                FillWeight = m_TransactionTypeWidth
            };
            transactionTypeColumn.Items.Add("Income");
            transactionTypeColumn.Items.Add("Expense");
            transactionTypeColumn.Items.Add("Deposit");
            transactionTypeColumn.Items.Add("Withdrawal");
            grid.Columns.Add(transactionTypeColumn);

            // 4. Text Column for Budget Group
            DataGridViewComboBoxColumn budgetGroupColumn = new DataGridViewComboBoxColumn
            {
                Name = "BudgetGroup",
                HeaderText = "Group",
                ValueType = typeof(string),
                FillWeight = m_BudgetGroupWidth,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox // Show as a combo box
            };

            // Populate the combo box items from the database.
            List<string> budgetGroups = _transactionService_FormUI.GetDistinctBudgetGroups("");
            foreach (string bg in budgetGroups)
            {
                budgetGroupColumn.Items.Add(bg);
            }

            grid.Columns.Add(budgetGroupColumn);

            // 5. ComboBox Column for Payment Method
            DataGridViewComboBoxColumn paymentMethodColumn = new DataGridViewComboBoxColumn
            {
                Name = "PaymentMethod",
                HeaderText = "Payment",
                ValueType = typeof(string),
                FillWeight = m_PaymentMethodWidth,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox // Display as combo box with editable style
            };

            // Populate the combo box items from the database.
            List<string> paymentMethods = _transactionService_FormUI.GetDistinctPaymentMethod("");
            foreach (string pm in paymentMethods)
            {
                paymentMethodColumn.Items.Add(pm);
            }
            grid.Columns.Add(paymentMethodColumn);

            // 6. ComboBox Column for Payee
            DataGridViewComboBoxColumn payeeColumn = new DataGridViewComboBoxColumn
            {
                Name = "Payee",
                HeaderText = "Payee",
                ValueType = typeof(string),
                FillWeight = m_PayeeWidth,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox // Display as combo box with editable style
            };

            // Populate the combo box items from the database.
            List<string> payees = _transactionService_FormUI.GetDistinctPayee("");
            foreach (string p in payees)
            {
                payeeColumn.Items.Add(p);
            }
            grid.Columns.Add(payeeColumn);

            // 7. DateTimePicker Column for Transaction Date
            // Use a custom column that hosts a DateTimePicker.
            CalendarColumn dateColumn = new CalendarColumn();
            dateColumn.Name = "TransactionDate";
            dateColumn.HeaderText = "Date";
            //dateColumn.Width = m_DateWidth;
            dateColumn.FillWeight = m_DateWidth;
            grid.Columns.Add(dateColumn);
            //grid.Columns["TransactionDate"].MaximumWidth = m_DateWidth;

            // 8. ComboBox Column for Recurrence
            DataGridViewComboBoxColumn recurrenceColumn = new DataGridViewComboBoxColumn
            {
                Name = "RecurrenceIndicator",
                HeaderText = "🔁",
                ValueType = typeof(string),
                FillWeight = m_RecurrenceWidth,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox  // Displays as a combo box
            };

            // Add default recurrence options. Adjust or extend the list as needed.
            recurrenceColumn.Items.Add("None");
            recurrenceColumn.Items.Add("Daily");
            recurrenceColumn.Items.Add("Weekly");
            recurrenceColumn.Items.Add("Biweekly");
            recurrenceColumn.Items.Add("Monthly");
            recurrenceColumn.Items.Add("Quarterly");
            recurrenceColumn.Items.Add("Semiannually");
            recurrenceColumn.Items.Add("Annually");

            grid.Columns.Add(recurrenceColumn);

            // 9. Text Column for Currency
            DataGridViewComboBoxColumn currencyColumn = new DataGridViewComboBoxColumn
            {
                Name = "Currency",
                HeaderText = "💰",
                ValueType = typeof(string),
                FillWeight = m_CurrencyWidth,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox // Display as combo box with editable style
            };

            // Populate the combo box items from the database.
            List<string> currencys = _transactionService_FormUI.GetDistinctCurrency("");
            foreach (string c in currencys)
            {
                currencyColumn.Items.Add(c);
            }
            grid.Columns.Add(currencyColumn);

            // 10. Text Column for Description
            DataGridViewTextBoxColumn descriptionColumn = new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "📝",
                ValueType = typeof(string),
                FillWeight = m_DescriptionWidth
            };
            grid.Columns.Add(descriptionColumn);

            // Adjust the height of Edit Fields
            if (m_IsAutoAdjustHeight) GridViewHeightHelper.AdjustDataGridViewHeight(grid);

            // Programmatically add one row for user input. RowUserInput
            if (m_IsAddRowInput)
            {
                for (int i = 0; i < rowCount; i++)
                    grid.Rows.Add();

                // Set the row number for each row
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    grid.Rows[i].Cells["RowNumber"].Value = i + 1;
                }
            }

            // Prevent the dataGridTransact from automatically adding a new row.
            if (!m_IsAllowUserAddRow) grid.AllowUserToAddRows = false;
        }
        public virtual void SetupWindowTabDataGridView(Form form, TabControl tabControl, ComboBox comboBoxFilter,
                                                        ComboBox comboBoxBudgetGroup, ComboBox comboBoxPaymentMethod,
                                                        ComboBox comboBoxPayee, DateTimePicker dateTimePickerStart,
                                                        DateTimePicker dateTimePickerEnd)
        {
            // Refresh only the control references.
            RefreshControlReferences(tabControl);

            if (activeDGV != null)
            {
                // Call the modified LoadData method to update both the DataGridView and the totals TextBoxes.
                LoadData(activeDGV, textBoxTotal, textBoxTotalIncome, textBoxTotalExpense);

                // Populate the comboBox with unique ActivityType values.
                PopulateActivityTypeComboBox(comboBoxFilter);
                PopulateBudgetGroupComboBox(comboBoxBudgetGroup);
                PopulatePaymentMethodComboBox(comboBoxPaymentMethod);
                PopulatePayeeComboBox(comboBoxPayee);
                InitializeDateTimePicker(dateTimePickerStart, true);  // Earliest TransactionDate
                InitializeDateTimePicker(dateTimePickerEnd, false);   // Latest TransactionDate
            }
            else
            {
                MessageBox.Show("No DataGridView found on the current tab that I can load the data.");
            }
        }
        public bool DeleteRowDataGridView(TabControl tabControl)
        {
            bool success = false;

            // Refresh only the control references.
            RefreshControlReferences(tabControl);

            if (activeDGV == null)
            {
                MessageBox.Show("No DataGridView found on the active tab.");
                return success;
            }

            // Use the active DataGridView.
            DataGridView currentDGV = activeDGV;

            // Ensure a row is selected.
            if (currentDGV.CurrentRow == null)
            {
                MessageBox.Show("Please select a transaction to delete.");
                return success;
            }

            // Retrieve values from the selected row.
            // Create a transaction record from UI fields.
            // Use the helper method to create the transaction record.
            FinancialTransactionRecord record = CreateTransactionRecordFromRow(currentDGV.CurrentRow);
            string itemToDelete = "No." + record.RowNumber + ") " + record.Activity + ", Amount: Php" + record.Amount +
                "\nDate:" + record.TransactionDate + " |" + record.TransactionType;

            // Ask for confirmation before deleting
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this transaction?\n\n" + itemToDelete,
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            // If user clicks "No", do nothing
            if (result == DialogResult.No)
                return success;
            else
            {
                success = true;
                _transactionID = record.TransactionID;
            }
            return success;

        }
        public bool SetUpEditFormDataGridView(Form form, TabControl tabControl, DataGridView grid, ComboBox comboBoxFilter,
                                                        ComboBox comboBoxBudgetGroup, ComboBox comboBoxPaymentMethod,
                                                        ComboBox comboBoxPayee, DateTimePicker dateTimePickerStart,
                                                        DateTimePicker dateTimePickerEnd)
        {
            bool success = false;
            // Refresh control references based on the currently active tab.
            RefreshControlReferences(tabControl);

            // Check if an active DataGridView was found.
            if (activeDGV == null)
            {
                MessageBox.Show("No DataGridView found on the active tab.");
                return success;
            }

            // Use the active DataGridView.
            DataGridView currentDGV = activeDGV;

            // Ensure a row is selected.
            if (currentDGV.CurrentRow == null)
            {
                MessageBox.Show("Please select a transaction to edit.");
                return success;
            }

            // Retrieve values from the selected row.
            // Create a transaction record from UI fields.
            // Use the helper method to create the transaction record.
            FinancialTransactionRecord record = CreateTransactionRecordFromRow(currentDGV.CurrentRow);

            // Create an instance of EditTransactionForm, passing all values.
            using EditTransactionForm editForm = new EditTransactionForm(record);

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Refresh the main data display on the active tab.
                SetupWindowTabDataGridView(form, tabControl, comboBoxFilter,
                                                               comboBoxBudgetGroup, comboBoxPaymentMethod,
                                                               comboBoxPayee, dateTimePickerStart,
                                                               dateTimePickerEnd);
                success = true;
            }
            return success;

        }
        public virtual bool BackupOperation()
        {
            bool success = false;
            try
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
                    username_db = user;
                    password_db = password;
                    backupfile_db = backupFile;
                }
                success = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Backup failed: " + ex.Message,
                                "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return success;
        }
        public virtual bool RestoreOperation()
        {
            bool success = false;
            try
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
                        username_db = user;
                        password_db = password;
                        backupfile_db = backupFile;
                        restorefile_db = restoredDatabaseFile;
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Restore failed: " + ex.Message,
                                "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return success;
        }
        public virtual bool ExportTabletoCSV(DataGridView grid)
        {
            bool success = false;
            try
            {
                // Open a SaveFileDialog to choose the CSV file destination.
                using (SaveFileDialog exportFileDialog = new SaveFileDialog())
                {
                    exportFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                    exportFileDialog.Title = "Export Financial Transactions to CSV";

                    if (exportFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // DataGridView is bound to a DataTable.
                        m_transactionDataTable = (DataTable)grid.DataSource;
                        m_csv_filename = exportFileDialog.FileName;

                        MessageBox.Show("Export successful.", "Export to CSV",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return success;

        }
        public virtual void RefreshControlReferences(TabControl tabControl)
        {
            // This method only refreshes the control references.

            // Get the currently active tab page.
            TabPage activeTab = tabControl.SelectedTab;

            // Reset the protected fields.
            activeDGV = null;
            textBoxTotal = null;
            textBoxTotalIncome = null;
            textBoxTotalExpense = null;

            // Loop through controls on the active tab and update the fields.
            foreach (Control ctrl in activeTab.Controls)
            {
                if (ctrl is DataGridView)
                {
                    activeDGV = (DataGridView)ctrl;
                }
                else if (ctrl is TextBox)
                {
                    // Check the Name property to assign the correct field.
                    switch (ctrl.Name)
                    {
                        case "textBox1Total":
                        case "textBox2Total":
                        case "textBox3Total":
                            textBoxTotal = (TextBox)ctrl;
                            break;
                        case "textBox1TotalIncome":
                        case "textBox2TotalIncome":
                        case "textBox3TotalIncome":
                            textBoxTotalIncome = (TextBox)ctrl;
                            break;
                        case "textBox1TotalExpense":
                        case "textBox2TotalExpense":
                        case "textBox3TotalExpense":
                            textBoxTotalExpense = (TextBox)ctrl;
                            break;
                    }
                }
            }
        }

        // Load data from the Firebird database into the DataGridView
        private void LoadData(DataGridView grid,
                                TextBox textBoxTotal,
                                TextBox textBoxTotalIncome,
                                TextBox textBoxTotalExpense,
                                string transact_ID = "TransactionID",
                                string activityTypeFilter = null,
                                string budgetGroupFilter = null,
                                string paymentMethodFilter = null,
                                string payeeFilter = null,
                                DateTime? startDate = null,
                                DateTime? endDate = null,
                                bool useWildcards = false)
        {
            try
            {
                using (FbConnection conn = new FbConnection(UIEventHandlers.m_FbConnectionString))
                {
                    conn.Open();

                    // Build the query string with all columns in the desired order.
                    // Start building the query.
                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append("SELECT " + transact_ID + ", TransactionDate, ActivityType, TransactionType, BudgetGroup, Amount, PaymentMethod, Payee, Currency, RecurrenceIndicator, Description ");
                    queryBuilder.Append("FROM FinancialTransactions WHERE 1=1");

                    //string query;
                    if (!string.IsNullOrEmpty(activityTypeFilter))
                    {
                        if (useWildcards)
                            queryBuilder.Append(" AND ActivityType LIKE @activityType");
                        else
                            queryBuilder.Append(" AND ActivityType = @activityType");
                    }

                    // Append condition for TransactionDate range.
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        queryBuilder.Append(" AND TransactionDate BETWEEN @startDate AND @endDate");
                    }

                    // Append condition for BudgetGroup.
                    if (!string.IsNullOrEmpty(budgetGroupFilter))
                    {
                        queryBuilder.Append(" AND BudgetGroup = @budgetGroup");
                    }

                    // Append condition for PaymentMethod.
                    if (!string.IsNullOrEmpty(paymentMethodFilter))
                    {
                        queryBuilder.Append(" AND PaymentMethod = @paymentMethod");
                    }

                    // Append condition for Payee.
                    if (!string.IsNullOrEmpty(payeeFilter))
                    {
                        queryBuilder.Append(" AND Payee = @payee");
                    }

                    queryBuilder.Append(" ORDER BY TransactionDate ASC");

                    string query = queryBuilder.ToString();
                    FbCommand cmd = new FbCommand(query, conn);

                    // Set parameters.
                    if (!string.IsNullOrEmpty(activityTypeFilter))
                    {
                        string paramValue = useWildcards ? "%" + activityTypeFilter + "%" : activityTypeFilter;
                        cmd.Parameters.AddWithValue("@activityType", paramValue);
                    }
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate.Value);
                        cmd.Parameters.AddWithValue("@endDate", endDate.Value);
                    }
                    if (!string.IsNullOrEmpty(budgetGroupFilter))
                    {
                        cmd.Parameters.AddWithValue("@budgetGroup", budgetGroupFilter);
                    }
                    if (!string.IsNullOrEmpty(paymentMethodFilter))
                    {
                        cmd.Parameters.AddWithValue("@paymentMethod", paymentMethodFilter);
                    }
                    if (!string.IsNullOrEmpty(payeeFilter))
                    {
                        cmd.Parameters.AddWithValue("@payee", payeeFilter);
                    }

                    FbDataAdapter adapter = new FbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Add a new column for row numbering.
                    dt.Columns.Add("No.", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["No."] = i + 1;
                    }
                    // Place the row number column as the first column.
                    dt.Columns["No."].SetOrdinal(0);

                    // Subscribe to the DataBindingComplete event BEFORE setting the DataSource.
                    grid.DataBindingComplete += (s, e) =>
                    {
                        if (grid.Columns["NO."] != null)
                            grid.Columns["NO."].DisplayIndex = 0;
                        if (grid.Columns["ACTIVITYTYPE"] != null)
                            grid.Columns["ACTIVITYTYPE"].DisplayIndex = 1;
                        if (grid.Columns["AMOUNT"] != null)
                            grid.Columns["AMOUNT"].DisplayIndex = 2;
                        if (grid.Columns["TRANSACTIONTYPE"] != null)
                            grid.Columns["TRANSACTIONTYPE"].DisplayIndex = 3;
                        if (grid.Columns["BUDGETGROUP"] != null)
                            grid.Columns["BUDGETGROUP"].DisplayIndex = 4;
                        if (grid.Columns["PAYMENTMETHOD"] != null)
                            grid.Columns["PAYMENTMETHOD"].DisplayIndex = 5;
                        if (grid.Columns["PAYEE"] != null)
                            grid.Columns["PAYEE"].DisplayIndex = 6;
                        if (grid.Columns["TRANSACTIONDATE"] != null)
                            grid.Columns["TRANSACTIONDATE"].DisplayIndex = 7;
                        if (grid.Columns["RECURRENCEINDICATOR"] != null)
                            grid.Columns["RECURRENCEINDICATOR"].DisplayIndex = 8;
                        if (grid.Columns["CURRENCY"] != null)
                            grid.Columns["CURRENCY"].DisplayIndex = 9;
                        if (grid.Columns["DESCRIPTION"] != null)
                            grid.Columns["DESCRIPTION"].DisplayIndex = 10;
                    };
                    // Bind the DataTable to the grid.
                    grid.DataSource = dt;

                    // Calculate totals from the AMOUNT column.
                    decimal totalAmount = 0;
                    decimal totalIncome = 0;
                    decimal totalExpense = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (decimal.TryParse(row["AMOUNT"].ToString(), out decimal amount))
                        {
                            totalAmount += amount;
                            if (amount >= 0)
                                totalIncome += amount;
                            else
                                totalExpense += amount;
                        }
                    }

                    if (textBoxTotal != null)
                        textBoxTotal.Text = "₱" + totalAmount.ToString("N2");
                    if (textBoxTotalIncome != null)
                        textBoxTotalIncome.Text = "₱" + totalIncome.ToString("N2");
                    if (textBoxTotalExpense != null)
                        textBoxTotalExpense.Text = "₱" + totalExpense.ToString("N2");
                }

                // Scroll to and select the last row after loading the data.
                ScrollToLastRow(grid);

                // Hide the TransactionID column if it exists.
                if (grid.Columns["TRANSACTIONID"] != null)
                {
                    grid.Columns["TRANSACTIONID"].Visible = false;
                }

                // Set header text and formatting for the DataGridView columns.

                if (grid.Columns["No."] != null)
                {
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "No.", 50);
                }
                if (grid.Columns["ACTIVITYTYPE"] != null)
                {
                    grid.Columns["ACTIVITYTYPE"].HeaderText = "Activity Type";
                    // Optionally, set a fixed width here.
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "ACTIVITYTYPE", 200);
                }
                if (grid.Columns["AMOUNT"] != null)
                {
                    grid.Columns["AMOUNT"].HeaderText = "Amount";
                    grid.Columns["AMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    grid.Columns["AMOUNT"].DefaultCellStyle.Format = "₱#,##0.00";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "AMOUNT", 130);
                }
                if (grid.Columns["TransactionType"] != null)
                {
                    grid.Columns["TransactionType"].HeaderText = "Transaction";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "TransactionType", 130);
                }
                if (grid.Columns["BudgetGroup"] != null)
                {
                    grid.Columns["BudgetGroup"].HeaderText = "Group";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "BudgetGroup", 90);
                }

                if (grid.Columns["PaymentMethod"] != null)
                {
                    grid.Columns["PaymentMethod"].HeaderText = "Payment Method";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "PaymentMethod", 170);
                }
                if (grid.Columns["Payee"] != null)
                {
                    grid.Columns["Payee"].HeaderText = "Payee";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "Payee", 150);
                }
                if (grid.Columns["Currency"] != null)
                {
                    grid.Columns["Currency"].HeaderText = "Unit";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "Currency", 50);
                }
                if (grid.Columns["TransactionDate"] != null)
                {
                    grid.Columns["TransactionDate"].HeaderText = "Date";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "TransactionDate", 115);
                }
                if (grid.Columns["RecurrenceIndicator"] != null)
                {
                    grid.Columns["RecurrenceIndicator"].HeaderText = "Cycle";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "RecurrenceIndicator", 115);
                }
                if (grid.Columns["Description"] != null)
                {
                    grid.Columns["Description"].HeaderText = "Description";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "Description", 200);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void ScrollToLastRow(DataGridView grid) // dataGridView1
        {
            if (grid.Rows.Count > 0)
            {
                int lastRowIndex = grid.Rows.Count - 1;
                grid.ClearSelection();
                grid.Rows[lastRowIndex].Selected = true;
                grid.FirstDisplayedScrollingRowIndex = lastRowIndex;
                // Set the current cell to a visible cell (e.g., the "No." column).
                grid.CurrentCell = grid.Rows[lastRowIndex].Cells["No."];

            }
        }

        public void ApplyFilter(TabControl tabControl, string activityType, string budgetGroup,
                        string paymentMethod, string payee, DateTime startDate, DateTime endDate)
        {
            // Refresh only the control references.
            RefreshControlReferences(tabControl);

            if (activeDGV != null && textBoxTotal != null &&
                textBoxTotalIncome != null && textBoxTotalExpense != null)
            {
                if (activityType == "All")
                    activityType = string.Empty;
                if (budgetGroup == "All")
                    budgetGroup = string.Empty;
                if (paymentMethod == "All")
                    paymentMethod = string.Empty;
                if (payee == "All")
                    payee = string.Empty;


                LoadData(activeDGV, textBoxTotal, textBoxTotalIncome, textBoxTotalExpense,
                                activityTypeFilter: activityType,
                                budgetGroupFilter: budgetGroup,
                                paymentMethodFilter: paymentMethod,
                                payeeFilter: payee,
                                startDate: startDate,
                                endDate: endDate);
            }
            else
            {
                MessageBox.Show("Required controls are not available.");
            }
        }
        public void GetRowContents()
        {
            // To do list, may be not needed anymore : c/o zaldy
        }

        public void ApplySearch(TabControl tabControl, string searchTerm, string transactionID)
        {
            // Refresh only the control references.
            RefreshControlReferences(tabControl);

            if (activeDGV != null && textBoxTotal != null &&
                textBoxTotalIncome != null && textBoxTotalExpense != null)
            {
                // Call LoadData with the search term and set useWildcards to true.
                LoadData(activeDGV, textBoxTotal, textBoxTotalIncome, textBoxTotalExpense,
                         transactionID, searchTerm, useWildcards: true);
            }
            else
            {
                MessageBox.Show("No DataGridView found on the current tab that I can load the data.");
            }
        }
        public void ShowAllColumn(TabControl tabControl)
        {
            // Refresh only the control references.
            RefreshControlReferences(tabControl);
            // Make sure the required controls are available.
            if (activeDGV != null && textBoxTotal != null &&
                textBoxTotalIncome != null && textBoxTotalExpense != null)
            {
                // Iterate over all columns in the DataGridView (assuming its name is dataGridView1)
                foreach (DataGridViewColumn column in activeDGV.Columns)
                {
                    column.Visible = true;
                }

                // Hide the TransactionID column if it exists.
                if (activeDGV.Columns["TRANSACTIONID"] != null)
                {
                    activeDGV.Columns["TRANSACTIONID"].Visible = false;
                }
            }
            else
            {
                MessageBox.Show("No DataGridView found on the current tab that I can show all the column.");
            }

        }

        private void PopulateActivityTypeComboBox(ComboBox comboBoxFilter)
        {
            try
            {
                using (FbConnection conn = new FbConnection(UIEventHandlers.m_FbConnectionString))
                {
                    conn.Open();
                    string query = "SELECT DISTINCT ActivityType FROM FinancialTransactions ORDER BY ActivityType ASC";
                    FbCommand cmd = new FbCommand(query, conn);
                    FbDataAdapter adapter = new FbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    comboBoxFilter.Items.Clear();
                    comboBoxFilter.Items.Add("All"); // Option to show all records.
                    foreach (DataRow row in dt.Rows)
                    {
                        comboBoxFilter.Items.Add(row["ActivityType"].ToString());
                    }
                    comboBoxFilter.SelectedIndex = 0; // Default selection is "All".
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error populating filter options: " + ex.Message);
            }
        }
        private void PopulateBudgetGroupComboBox(ComboBox comboBoxBudgetGroup)
        {
            try
            {
                using (FbConnection conn = new FbConnection(UIEventHandlers.m_FbConnectionString))
                {
                    conn.Open();
                    string query = "SELECT DISTINCT BudgetGroup FROM FinancialTransactions ORDER BY BudgetGroup ASC";
                    FbCommand cmd = new FbCommand(query, conn);
                    FbDataAdapter adapter = new FbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    comboBoxBudgetGroup.Items.Clear();
                    comboBoxBudgetGroup.Items.Add("All"); // Option to show all records.
                    foreach (DataRow row in dt.Rows)
                    {
                        comboBoxBudgetGroup.Items.Add(row["BudgetGroup"].ToString());
                    }
                    comboBoxBudgetGroup.SelectedIndex = 0; // Default selection is "All".
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error populating BudgetGroup options: " + ex.Message);
            }
        }
        private void PopulatePaymentMethodComboBox(ComboBox comboBoxPaymentMethod)
        {
            try
            {
                using (FbConnection conn = new FbConnection(UIEventHandlers.m_FbConnectionString))
                {
                    conn.Open();
                    string query = "SELECT DISTINCT PaymentMethod FROM FinancialTransactions ORDER BY PaymentMethod ASC";
                    FbCommand cmd = new FbCommand(query, conn);
                    FbDataAdapter adapter = new FbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    comboBoxPaymentMethod.Items.Clear();
                    comboBoxPaymentMethod.Items.Add("All"); // Option to show all records.
                    foreach (DataRow row in dt.Rows)
                    {
                        comboBoxPaymentMethod.Items.Add(row["PaymentMethod"].ToString());
                    }
                    comboBoxPaymentMethod.SelectedIndex = 0; // Default selection is "All".
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error populating PaymentMethod options: " + ex.Message);
            }
        }
        private void PopulatePayeeComboBox(ComboBox comboBoxPayee)
        {
            try
            {
                using (FbConnection conn = new FbConnection(UIEventHandlers.m_FbConnectionString))
                {
                    conn.Open();
                    string query = "SELECT DISTINCT Payee FROM FinancialTransactions ORDER BY Payee ASC";
                    FbCommand cmd = new FbCommand(query, conn);
                    FbDataAdapter adapter = new FbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    comboBoxPayee.Items.Clear();
                    comboBoxPayee.Items.Add("All"); // Option to show all records.
                    foreach (DataRow row in dt.Rows)
                    {
                        comboBoxPayee.Items.Add(row["Payee"].ToString());
                    }
                    comboBoxPayee.SelectedIndex = 0; // Default selection is "All".
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error populating Payee options: " + ex.Message);
            }
        }
        private void InitializeDateTimePickerStart(DateTimePicker dateTimePickerStart)
        {
            try
            {
                using (FbConnection conn = new FbConnection(UIEventHandlers.m_FbConnectionString))
                {
                    conn.Open();
                    // Query to get the earliest TransactionDate.
                    string query = "SELECT MIN(TransactionDate) FROM FinancialTransactions";
                    FbCommand cmd = new FbCommand(query, conn);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        // Convert result to DateTime and set it as the start date.
                        dateTimePickerStart.Value = Convert.ToDateTime(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing start date: " + ex.Message);
            }
        }
        private void InitializeDateTimePicker(DateTimePicker dtPicker, bool getEarliest)
        {
            try
            {
                using (FbConnection conn = new FbConnection(UIEventHandlers.m_FbConnectionString))
                {
                    conn.Open();
                    // Use MIN for earliest, MAX for latest.
                    string aggregateFunction = getEarliest ? "MIN" : "MAX";
                    string query = $"SELECT {aggregateFunction}(TransactionDate) FROM FinancialTransactions";
                    FbCommand cmd = new FbCommand(query, conn);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        dtPicker.Value = Convert.ToDateTime(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing date picker: " + ex.Message);
            }
        }
        public class TransactionData(FinancialTransactionRecord record)
        {
            // Constructor to initialize the record.
            private string m_message;
            public string GetMessage()
            {
                return m_message;
            }
            // Example of a validation method
            public bool IsValid()
            {
                if (string.IsNullOrWhiteSpace(record.Activity) ||
                    record.Amount == 0m ||
                    record.TransactionDate == DateTime.MinValue ||
                    string.IsNullOrWhiteSpace(record.TransactionType) ||
                    string.IsNullOrWhiteSpace(record.BudgetGroup) ||
                    string.IsNullOrWhiteSpace(record.PaymentMethod) ||
                    string.IsNullOrWhiteSpace(record.Payee) ||
                    string.IsNullOrWhiteSpace(record.Recurrence) ||
                    string.IsNullOrWhiteSpace(record.Currency) ||
                    string.IsNullOrWhiteSpace(record.Description))
                {
                    m_message = "\nPlease fill up all the required fields.";
                    return false;
                }

                // Additional checks based on transaction type.
                if ((record.TransactionType.Equals("Withdrawal", StringComparison.OrdinalIgnoreCase) ||
                     record.TransactionType.Equals("Expense", StringComparison.OrdinalIgnoreCase)) &&
                     record.Amount >= 0)
                {
                    m_message = "\nAmount should be less than 0";
                    return false;
                }
                else if ((record.TransactionType.Equals("Income", StringComparison.OrdinalIgnoreCase) ||
                          record.TransactionType.Equals("Deposit", StringComparison.OrdinalIgnoreCase)) &&
                          record.Amount <= 0)
                {
                    m_message = "\nAmount should be greater than 0";
                    return false;
                }

                return true;
            }
        }

        public virtual bool IsFillupComplete(DataGridView grid, DataGridViewRow row, FinancialTransactionRecord record)
        {
            bool success = false;

            // Loop through all rows in the dataGridView
            // Check if one of the column in the row has filled
            if (!string.IsNullOrWhiteSpace(record.Activity) ||
                record.Amount != 0m || // Adjust the condition as needed (e.g., _amount <= 0m) but i have negative value
                !string.IsNullOrWhiteSpace(record.TransactionType) ||
                !string.IsNullOrWhiteSpace(record.BudgetGroup) ||
                !string.IsNullOrWhiteSpace(record.PaymentMethod) ||
                !string.IsNullOrWhiteSpace(record.Payee) ||
                !string.IsNullOrWhiteSpace(record.Recurrence) ||
                !string.IsNullOrWhiteSpace(record.Currency) ||
                !string.IsNullOrWhiteSpace(record.Description))
            {
                // Check if the transaction data is valid.
                TransactionData data = new TransactionData(record);
                if (!data.IsValid())
                {
                    MessageBox.Show("Activity: " + record.Activity + data.GetMessage());
                    return success;
                }
            }
            return success = true;
        }
        private bool ValidateRecord(FinancialTransactionRecord record)
        {
            return !string.IsNullOrWhiteSpace(record.Activity) &&
                   record.Amount != 0m &&  // Adjust condition if needed
                   record.TransactionDate != DateTime.MinValue &&
                   !string.IsNullOrWhiteSpace(record.TransactionType) &&
                   !string.IsNullOrWhiteSpace(record.BudgetGroup) &&
                   !string.IsNullOrWhiteSpace(record.PaymentMethod) &&
                   !string.IsNullOrWhiteSpace(record.Payee) &&
                   !string.IsNullOrWhiteSpace(record.Recurrence) &&
                   !string.IsNullOrWhiteSpace(record.Currency) &&
                   !string.IsNullOrWhiteSpace(record.Description);
        }
        public virtual bool IsFillupValid(DataGridView grid, DataGridViewRow row,
                                            FinancialTransactionRecord record = null,
                                            bool isNewEntry = true)
        {
            bool success = false;
            FinancialTransactionRecord _record;

            // Skip the new row placeholder
            if (row.IsNewRow)
                return success;

            // Retrieve values from each column
            if (isNewEntry)
            {
                _record = CreateTransactionRecordFromRow(row, isNewEntry);
            }
            else
            {
                _record = record;
            }

            if (ValidateRecord(_record))
            {
                success = true;
            }

            return success;
        }
    }

    // Class Member and Constructor here
    class GridDataViewTransactEventHandler : GridDataViewBaseEventHandler
    {
        // Constructor
        public GridDataViewTransactEventHandler()
        {
            m_IsAddRowInput = true;
            m_IsAllowUserAddRow = false;
            m_IsAddColNum = true;
            m_IsAutoAdjustHeight = false;
            m_AmountWidth = 105;
            m_DateWidth = 120;
            m_TransactionTypeWidth = 105;
        }

        //public void SetupDataGridView(DataGridView grid)
        //{
        //    // Custom implementation here.
        //}
        // Public wrapper method for the protected method.
        public FinancialTransactionRecord GetTransactionRecord(DataGridViewRow row, bool isNewEntry = false)
        {
            return CreateTransactionRecordFromRow(row, isNewEntry);
        }
    }

    // Child class inherits from Interface IFormUIEventHandler.
    class GridDataViewEditFormEventHandler : GridDataViewBaseEventHandler
    {
        // Constructor
        public GridDataViewEditFormEventHandler()
        {
            // Set new parameters for the child class.
            m_IsAddRowInput = false;
            m_IsAllowUserAddRow = true;
            m_IsAddColNum = false; // remains false
            m_IsAutoAdjustHeight = true;
            m_ActivityWidth = 260;
            m_AmountWidth = 100;
            m_DateWidth = 100;
            m_TransactionTypeWidth = 100;
            m_BudgetGroupWidth = 90;
            m_PaymentMethodWidth = 120;
            m_PayeeWidth = 100;
            m_RecurrenceWidth = 80;
            m_CurrencyWidth = 55;
            m_DescriptionWidth = 50;
        }

        // Methods
        public FinancialTransactionRecord GetTransactionRecord(DataGridViewRow row, bool isNewEntry = false)
        {
            return CreateTransactionRecordFromRow(row, isNewEntry);
        }

    }

    public static class GridViewHeightHelper
    {
        public static void AdjustDataGridViewHeight(DataGridView dgv)
        {
            // Start with the height of the column headers (if they are visible)
            int totalHeight = dgv.ColumnHeadersVisible ? dgv.ColumnHeadersHeight : 0;

            // Add the height of each row in the grid
            foreach (DataGridViewRow row in dgv.Rows)
            {
                totalHeight += row.Height;
            }

            // Optionally, add any additional padding or border thickness here if needed
            int padding = 8;
            // Set the grid's height to the calculated total
            dgv.Height = totalHeight + padding;
        }
    }

    public static class GridViewHelper
    {

        // Custom Calendar Column that hosts a DateTimePicker in editing mode.
        public class CalendarColumn : DataGridViewColumn
        {
            public CalendarColumn() : base(new CalendarCell())
            {
                this.ValueType = typeof(DateTime);
            }

            public override DataGridViewCell CellTemplate
            {
                get { return base.CellTemplate; }
                set
                {
                    // Ensure that the cell used for the template is a CalendarCell.
                    if (value != null && !value.GetType().IsAssignableFrom(typeof(CalendarCell)))
                        throw new InvalidCastException("Must be a CalendarCell");
                    base.CellTemplate = value;
                }
            }
        }

        // Custom Calendar Cell that uses a DateTimePicker for editing.
        public class CalendarCell : DataGridViewTextBoxCell
        {
            public CalendarCell() : base()
            {
                this.Style.Format = "d"; // Use short date format.
            }

            public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
            {
                base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
                CalendarEditingControl ctl = DataGridView.EditingControl as CalendarEditingControl;
                if (this.Value == null || this.Value == DBNull.Value)
                {
                    ctl.Value = DateTime.Now;
                }
                else
                {
                    // Attempt to parse the value if it's not already a DateTime.
                    if (this.Value is DateTime dateValue)
                    {
                        ctl.Value = dateValue;
                    }
                    else if (DateTime.TryParse(this.Value.ToString(), out DateTime parsedDate))
                    {
                        ctl.Value = parsedDate;
                    }
                    else
                    {
                        ctl.Value = DateTime.Now; // Fallback default value.
                    }
                }
            }

            public override Type EditType
            {
                get { return typeof(CalendarEditingControl); }
            }

            public override Type ValueType
            {
                get { return typeof(DateTime); }
            }

            public override object DefaultNewRowValue
            {
                get { return DateTime.Now; }
            }
        }

        // The editing control hosted by the CalendarCell.
        public class CalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
        {
            DataGridView dataGridView;
            private bool valueChanged = false;
            int rowIndex;

            public CalendarEditingControl()
            {
                this.Format = DateTimePickerFormat.Short;
            }

            public object EditingControlFormattedValue
            {
                get { return this.Value.ToShortDateString(); }
                set
                {
                    if (value is string)
                    {
                        try
                        {
                            this.Value = DateTime.Parse((string)value);
                        }
                        catch
                        {
                            this.Value = DateTime.Now;
                        }
                    }
                }
            }

            public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
            {
                return EditingControlFormattedValue;
            }

            public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
            {
                this.Font = dataGridViewCellStyle.Font;
                this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
                this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
            }

            public int EditingControlRowIndex
            {
                get { return rowIndex; }
                set { rowIndex = value; }
            }

            public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
            {
                // Let the DateTimePicker handle the keys listed.
                switch (key & Keys.KeyCode)
                {
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Right:
                    case Keys.Home:
                    case Keys.End:
                    case Keys.PageDown:
                    case Keys.PageUp:
                        return true;
                    default:
                        return !dataGridViewWantsInputKey;
                }
            }

            public void PrepareEditingControlForEdit(bool selectAll)
            {
                // No preparation needed.
            }

            public bool RepositionEditingControlOnValueChange
            {
                get { return false; }
            }

            public DataGridView EditingControlDataGridView
            {
                get { return dataGridView; }
                set { dataGridView = value; }
            }

            public bool EditingControlValueChanged
            {
                get { return valueChanged; }
                set { valueChanged = value; }
            }

            public Cursor EditingPanelCursor
            {
                get { return base.Cursor; }
            }

            protected override void OnValueChanged(EventArgs eventargs)
            {
                valueChanged = true;
                this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
                base.OnValueChanged(eventargs);
            }
        }

    }

    public static class GridViewColumnHelper
    {
        public static void SetFixedColumnWidth(DataGridView grid, string columnName, int width)
        {
            if (grid.Columns.Contains(columnName))
            {
                grid.Columns[columnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns[columnName].Width = width;
            }
        }
    }

    public static class IniFile
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetPrivateProfileString(
            string section,
            string key,
            string defaultValue,
            StringBuilder retVal,
            int size,
            string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern long WritePrivateProfileString(
            string section,
            string key,
            string value,
            string filePath);

        public static string Read(string section, string key, string filePath, string defaultValue = "")
        {
            StringBuilder retVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, retVal, 255, filePath);
            return retVal.ToString();
        }
        public static void Write(string section, string key, string value, string filePath)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }
    }


}
