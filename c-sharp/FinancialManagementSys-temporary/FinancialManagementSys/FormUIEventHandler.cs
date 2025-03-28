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
        //int m_DateWidth { get; set; }
        int m_TransactionTypeWidth { get; set; }

        // Method signature here.
        void SetupDataGridView(DataGridView grid);
        void SetupWindowTabDataGridView(Form form, TabControl tabControl, ComboBox comboBoxFilter);
        bool BackupOperation();
        bool RestoreOperation();
        bool ExportTabletoCSV(DataGridView grid);
    }
    public interface IDataGridEntry
    {
        bool IsFillupComplete(DataGridView grid);
        bool IsFillupValid(DataGridView grid, DataGridViewRow row);
    }
    //===============================
    // Classes
    //===============================
    // Abstract Base Class
    abstract class GridDataViewBaseEventHandler : IFormUIEventHandler, IDataGridEntry
    {
        // Fields to store the control references
        protected DataGridView activeDGV;
        protected TextBox textBoxTotal;
        protected TextBox textBoxTotalIncome;
        protected TextBox textBoxTotalExpense;

        // Private fields to hold transaction data.
        private string _activity = string.Empty;
        private string _amountText = string.Empty;
        private decimal amount = 0;
        private string _transactionDate = string.Empty;
        private DateTime transactionDate = DateTime.MinValue;
        private string _transactionType = string.Empty;

        // Database Credentials
        private string username_db = string.Empty;
        private string password_db = string.Empty;
        private string backupfile_db = string.Empty;
        private string restorefile_db = string.Empty;

        // Transaction Data
        DataTable m_transactionDataTable = null;
        string m_csv_filename = string.Empty;

        // Public read-only properties to expose the transaction data.
        public string Activity { get { return _activity; } }
        public decimal Amount { get { return amount; } }
        public DateTime TransactionDate { get { return transactionDate; } }
        public string TransactionType { get { return _transactionType; } }
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

        // Constructor
        protected GridDataViewBaseEventHandler()
        {
            m_AmountWidth = 130;  // Default value
            m_DateWidth = 190;
            m_TransactionTypeWidth = 190;
        }

        // Methods to implement the interface.

        // Default method implementation for setting up the DataGridView.
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
                    FillWeight = 15
                };
                grid.Columns.Add(rowNumberColumn);
            }

            // 1. Text Column for Activity Type
            DataGridViewTextBoxColumn activityColumn = new DataGridViewTextBoxColumn();
            activityColumn.Name = "ActivityType";
            activityColumn.HeaderText = "Activity Type";
            activityColumn.ValueType = typeof(string);
            grid.Columns.Add(activityColumn);
            //activityColumn.Width = m_ActivityWidth;

            // 2. Numeric Column for Amount (using a TextBox column with numeric ValueType)
            DataGridViewTextBoxColumn amountColumn = new DataGridViewTextBoxColumn();
            amountColumn.Name = "Amount";
            amountColumn.HeaderText = "Amount";
            amountColumn.ValueType = typeof(decimal);
            amountColumn.DefaultCellStyle.Format = "#,##0.00"; // Formats as integer with thousand separators
            //amountColumn.Width = m_AmountWidth;
            amountColumn.FillWeight = m_AmountWidth;
            grid.Columns.Add(amountColumn);


            // 3. DateTimePicker Column for Transaction Date
            // Use a custom column that hosts a DateTimePicker.
            CalendarColumn dateColumn = new CalendarColumn();
            dateColumn.Name = "TransactionDate";
            dateColumn.HeaderText = "Transaction Date";
            //dateColumn.Width = m_DateWidth;
            dateColumn.FillWeight = m_DateWidth;
            grid.Columns.Add(dateColumn);
            //grid.Columns["TransactionDate"].MaximumWidth = m_DateWidth;

            // 4. ComboBox Column for Transaction Type
            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn();
            comboColumn.Name = "TransactionType";
            comboColumn.HeaderText = "Transaction Type";
            comboColumn.Items.Add("Income");
            comboColumn.Items.Add("Expense");
            comboColumn.Items.Add("Deposit");
            comboColumn.Items.Add("Withdrawal");
            //comboColumn.Width = m_TransactionTypeWidth;
            comboColumn.FillWeight = m_TransactionTypeWidth;
            grid.Columns.Add(comboColumn);


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
        public virtual void SetupWindowTabDataGridView(Form form, TabControl tabControl, ComboBox comboBoxFilter)
        {
            // Refresh only the control references.
            RefreshControlReferences(tabControl);

            if (activeDGV != null)
            {
                // Call the modified LoadData method to update both the DataGridView and the totals TextBoxes.
                LoadData(activeDGV, textBoxTotal, textBoxTotalIncome, textBoxTotalExpense);

                // Populate the comboBox with unique ActivityType values.
                PopulateActivityTypeComboBox(comboBoxFilter);
            }
            else
            {
                MessageBox.Show("No DataGridView found on the current tab that I can load the data.");
            }
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
                                bool useWildcards = false)
        {
            try
            {
                using (FbConnection conn = new FbConnection(UIEventHandlers.m_FbConnectionString))
                {
                    conn.Open();

                    // Build the query string with all columns in the desired order.
                    string query;
                    if (!string.IsNullOrEmpty(activityTypeFilter))
                    {
                        if (useWildcards)
                        {
                            query = "SELECT " + transact_ID + ", TransactionDate, ActivityType, TransactionType, BudgetGroup, Amount, PaymentMethod, Payee, Currency, RecurrenceIndicator, Description " +
                                    "FROM FinancialTransactions " +
                                    "WHERE ActivityType LIKE @activityType " +
                                    "ORDER BY TransactionDate ASC";
                        }
                        else
                        {
                            query = "SELECT " + transact_ID + ", TransactionDate, ActivityType, TransactionType, BudgetGroup, Amount, PaymentMethod, Payee, Currency, RecurrenceIndicator, Description " +
                                    "FROM FinancialTransactions " +
                                    "WHERE ActivityType = @activityType " +
                                    "ORDER BY TransactionDate ASC";
                        }
                    }
                    else
                    {
                        query = "SELECT " + transact_ID + ", TransactionDate, ActivityType, TransactionType, BudgetGroup, Amount, PaymentMethod, Payee, Currency, RecurrenceIndicator, Description " +
                                "FROM FinancialTransactions " +
                                "ORDER BY TransactionDate ASC";
                    }

                    FbCommand cmd = new FbCommand(query, conn);
                    if (!string.IsNullOrEmpty(activityTypeFilter))
                    {
                        // Apply the appropriate parameter value.
                        string paramValue = useWildcards ? "%" + activityTypeFilter + "%" : activityTypeFilter;
                        cmd.Parameters.AddWithValue("@activityType", paramValue);
                    }

                    FbDataAdapter adapter = new FbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Convert all column names to uppercase.
                    //foreach (DataColumn col in dt.Columns)
                    //{
                    //    col.ColumnName = col.ColumnName.ToUpper();
                    //}

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
                        if (grid.Columns["CURRENCY"] != null)
                            grid.Columns["CURRENCY"].DisplayIndex = 7;
                        if (grid.Columns["TRANSACTIONDATE"] != null)
                            grid.Columns["TRANSACTIONDATE"].DisplayIndex = 8;
                        if (grid.Columns["RECURRENCEINDICATOR"] != null)
                            grid.Columns["RECURRENCEINDICATOR"].DisplayIndex = 9;
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
                    //grid.Columns["No."].DisplayIndex = 0;
                }
                if (grid.Columns["ACTIVITYTYPE"] != null)
                {
                    grid.Columns["ACTIVITYTYPE"].HeaderText = "Activity Type";
                    // Optionally, set a fixed width here.
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "ACTIVITYTYPE", 200);
                    //grid.Columns["ACTIVITYTYPE"].DisplayIndex = 1;
                }
                if (grid.Columns["AMOUNT"] != null)
                {
                    grid.Columns["AMOUNT"].HeaderText = "Amount";
                    grid.Columns["AMOUNT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    grid.Columns["AMOUNT"].DefaultCellStyle.Format = "₱#,##0.00";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "AMOUNT", 130);
                    //grid.Columns["AMOUNT"].DisplayIndex = 2;
                }
                if (grid.Columns["TransactionType"] != null)
                {
                    grid.Columns["TransactionType"].HeaderText = "Transaction Type";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "TransactionType", 190);
                    //grid.Columns["TransactionType"].DisplayIndex = 3;
                }
                if (grid.Columns["BudgetGroup"] != null)
                {
                    grid.Columns["BudgetGroup"].HeaderText = "Budget Group";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "BudgetGroup", 150);
                    //grid.Columns["BudgetGroup"].DisplayIndex = 4;
                }

                if (grid.Columns["PaymentMethod"] != null)
                {
                    grid.Columns["PaymentMethod"].HeaderText = "Payment Method";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "PaymentMethod", 170);
                    //grid.Columns["PaymentMethod"].DisplayIndex = 5;
                }
                if (grid.Columns["Payee"] != null)
                {
                    grid.Columns["Payee"].HeaderText = "Payee";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "Payee", 150);
                    //grid.Columns["Payee"].DisplayIndex = 6;
                }
                if (grid.Columns["Currency"] != null)
                {
                    grid.Columns["Currency"].HeaderText = "Currency";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "Currency", 100);
                    //grid.Columns["Currency"].DisplayIndex = 7;
                }
                if (grid.Columns["TransactionDate"] != null)
                {
                    grid.Columns["TransactionDate"].HeaderText = "Transaction Date";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "TransactionDate", 190);
                    //grid.Columns["TransactionDate"].DisplayIndex = 8;
                }
                if (grid.Columns["RecurrenceIndicator"] != null)
                {
                    grid.Columns["RecurrenceIndicator"].HeaderText = "Recurrence";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "RecurrenceIndicator", 100);
                    //grid.Columns["RecurrenceIndicator"].DisplayIndex = 9;
                }
                if (grid.Columns["Description"] != null)
                {
                    grid.Columns["Description"].HeaderText = "Description";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "Description", 200);
                    //grid.Columns["Description"].DisplayIndex = 10;
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

        public void ApplyFilter(TabControl tabControl, string activityType)
        {
            // Refresh only the control references.
            RefreshControlReferences(tabControl);

            if (activeDGV != null && textBoxTotal != null &&
                textBoxTotalIncome != null && textBoxTotalExpense != null)
            {
                if (activityType == "All")
                {
                    LoadData(activeDGV, textBoxTotal, textBoxTotalIncome, textBoxTotalExpense);
                }
                else
                {
                    LoadData(activeDGV, textBoxTotal, textBoxTotalIncome, textBoxTotalExpense, activityTypeFilter: activityType);
                }
            }
            else
            {
                MessageBox.Show("Required controls are not available.");
            }
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
                         transactionID, searchTerm, true);
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

        public virtual bool IsFillupComplete(DataGridView grid)
        {
            bool success = false;

            // Loop through all rows in the dataGridView
            // Check if one of the column in the row has filled
            foreach (DataGridViewRow row in grid.Rows)
            {
                // Retrieve values from each column
                string activity = row.Cells["ActivityType"].Value?.ToString();
                string amountText = row.Cells["Amount"].Value?.ToString();
                string dateText = row.Cells["TransactionDate"].Value?.ToString();
                string transactionType = row.Cells["TransactionType"].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(activity) ||
                    !string.IsNullOrWhiteSpace(amountText) ||
                    !string.IsNullOrWhiteSpace(dateText) ||
                    !string.IsNullOrWhiteSpace(transactionType))
                {
                    if (string.IsNullOrWhiteSpace(activity) ||
                        string.IsNullOrWhiteSpace(amountText) ||
                        string.IsNullOrWhiteSpace(dateText) ||
                        string.IsNullOrWhiteSpace(transactionType))
                    {
                        MessageBox.Show("Activity: " + activity + "\nPlease complete the remaining field. ");
                        row.Cells["TransactionDate"].Value = string.Empty;  // Clear the dateText
                        return success;
                    }
                    else
                    {
                        decimal amount = Convert.ToDecimal(row.Cells["Amount"].Value);

                        // Check TransactionType conditions:
                        // For "Withdrawal" or "Expense", the amount should be negative.
                        if ((transactionType.Equals("Withdrawal", StringComparison.OrdinalIgnoreCase) ||
                             transactionType.Equals("Expense", StringComparison.OrdinalIgnoreCase)) &&
                             amount >= 0)
                        {
                            // Skip this row if the condition is not satisfied.
                            MessageBox.Show("Activity: " + activity + "\nAmount should be less than 0");
                            return success;
                        }
                        // For "Income" or "Deposit", the amount should be positive.
                        else if ((transactionType.Equals("Income", StringComparison.OrdinalIgnoreCase) ||
                                  transactionType.Equals("Deposit", StringComparison.OrdinalIgnoreCase)) &&
                                  amount <= 0)
                        {
                            // Skip this row if the condition is not satisfied.
                            MessageBox.Show("Activity: " + activity + "\nAmount should be greater than 0");
                            return success;
                        }
                    }
                }
            }
            return success = true;
        }

        public virtual bool IsFillupValid(DataGridView grid, DataGridViewRow row)
        {
            bool success = false;

            // Skip the new row placeholder
            if (row.IsNewRow)
                return success;

            // Retrieve values from each column
            _activity = row.Cells["ActivityType"].Value?.ToString();
            _amountText = row.Cells["Amount"].Value?.ToString();
            _transactionDate = row.Cells["TransactionDate"].Value?.ToString();
            _transactionType = row.Cells["TransactionType"].Value?.ToString();

            // Validate required fields (you can add further validation if needed)
            // Skip Checking of this row if one of column is empty
            if (string.IsNullOrWhiteSpace(_activity) ||
                string.IsNullOrWhiteSpace(_amountText) ||
                string.IsNullOrWhiteSpace(_transactionDate) ||
                string.IsNullOrWhiteSpace(_transactionType))
            {
                // Skip this row and do not notify the user
                return success;
            }

            // Parse Amount and TransactionDate
            // Check by discarding the output. In C# 7.0 and later, you can use the discard operator (_) 
            if (!Decimal.TryParse(_amountText, out amount))
            {
                MessageBox.Show("Invalid numeric value for Amount in row " + row.Index);
                return success;
            }

            if (!DateTime.TryParse(_transactionDate, out transactionDate))
            {
                MessageBox.Show("Invalid date value in row " + row.Index);
                return success;
            }
            success = true;
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
            m_AmountWidth = 45;
            m_DateWidth = 65;
            m_TransactionTypeWidth = 65;
        }

        //public void SetupDataGridView(DataGridView grid)
        //{
        //    // Custom implementation here.
        //}
    }

    // Child class inherits from Interface IFormUIEventHandler.
    class GridDataViewEditFormEventHandler : GridDataViewBaseEventHandler
    {
        public GridDataViewEditFormEventHandler()
        {
            // Set new parameters for the child class.
            m_IsAddRowInput = false;
            m_IsAllowUserAddRow = true;
            m_IsAddColNum = false; // remains false
            m_IsAutoAdjustHeight = true;
            m_AmountWidth = 30;
            m_DateWidth = 50;
            m_TransactionTypeWidth = 50;
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
                    ctl.Value = (DateTime)this.Value;
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
