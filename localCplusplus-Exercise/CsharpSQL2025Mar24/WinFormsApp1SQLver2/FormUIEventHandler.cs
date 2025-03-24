using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WinFormsApp1SQL.GridViewHelper;

namespace WinFormsApp1SQL
{
    // FormUI Interface
    public interface IFormUIEventHandler
    {
        // Declare the properties needed by the default implementation.
        bool m_IsAddRowInput { get; set; }
        bool m_IsAllowUserAddRow { get; set; }
        bool m_IsAddColNum { get; set; }
        bool m_IsAutoAdjustHeight { get; set; }
        int m_ActivityWidth { get; set; }
        int m_AmountWidth { get; set; }
        int m_DateWidth { get; set; }
        int m_TransactionTypeWidth { get; set; }

        //method signature here.
        void SetupTransactDataGridView(DataGridView grid);
        void SetupWindowTabDataGridView(Form form, TabControl tabControl, ComboBox comboBoxFilter);
    }

    // Abstract Base Class
    abstract class GridDataViewBaseEventHandler : IFormUIEventHandler
    {
        // Fields to store the control references
        protected DataGridView activeDGV;
        protected TextBox textBoxTotal;
        protected TextBox textBoxTotalIncome;
        protected TextBox textBoxTotalExpense;
        // Implement the properties.
        public bool m_IsAddRowInput { get; set; }
        public bool m_IsAllowUserAddRow { get; set; }
        public bool m_IsAddColNum { get; set; }
        public bool m_IsAutoAdjustHeight { get; set; }
        public int m_ActivityWidth { get; set; }
        public int m_AmountWidth { get; set; }
        public int m_DateWidth { get; set; }
        public int m_TransactionTypeWidth { get; set; }

        // Default method implementation for setting up the DataGridView.
        public virtual void SetupTransactDataGridView(DataGridView grid)
        {
            grid.Columns.Clear();

            if (m_IsAddColNum)
            {
                // 0. Add the row number column.
                DataGridViewTextBoxColumn rowNumberColumn = new DataGridViewTextBoxColumn
                {
                    Name = "RowNumber",
                    HeaderText = "No.",
                    ReadOnly = true,
                    Width = 50
                };
                grid.Columns.Add(rowNumberColumn);
            }

            // 1. Text Column for Activity Type
            DataGridViewTextBoxColumn activityColumn = new DataGridViewTextBoxColumn();
            activityColumn.Name = "ActivityType";
            activityColumn.HeaderText = "Activity Type";
            activityColumn.ValueType = typeof(string);
            grid.Columns.Add(activityColumn);
            activityColumn.Width = m_ActivityWidth;

            // 2. Numeric Column for Amount (using a TextBox column with numeric ValueType)
            DataGridViewTextBoxColumn amountColumn = new DataGridViewTextBoxColumn();
            amountColumn.Name = "Amount";
            amountColumn.HeaderText = "Amount";
            amountColumn.ValueType = typeof(decimal);
            grid.Columns.Add(amountColumn);
            amountColumn.Width = m_AmountWidth;

            // 3. DateTimePicker Column for Transaction Date
            // Use a custom column that hosts a DateTimePicker.
            CalendarColumn dateColumn = new CalendarColumn();
            dateColumn.Name = "TransactionDate";
            dateColumn.HeaderText = "Transaction Date";
            grid.Columns.Add(dateColumn);
            dateColumn.Width = m_DateWidth;

            // 4. ComboBox Column for Transaction Type
            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn();
            comboColumn.Name = "TransactionType";
            comboColumn.HeaderText = "Transaction Type";
            comboColumn.Items.Add("Income");
            comboColumn.Items.Add("Expense");
            comboColumn.Items.Add("Deposit");
            comboColumn.Items.Add("Withdrawal");
            grid.Columns.Add(comboColumn);
            comboColumn.Width = m_TransactionTypeWidth;

            // Adjust the height of Edit Fields
            if (m_IsAutoAdjustHeight) GridViewHeightHelper.AdjustDataGridViewHeight(grid);


            // Programmatically add one row for user input. RowUserInput
            if (m_IsAddRowInput)
            {

                for (int i = 0; i < 5; i++)
                    grid.Rows.Add();
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

                    string query;
                    if (!string.IsNullOrEmpty(activityTypeFilter))
                    {
                        if (useWildcards)
                        {
                            query = "SELECT " + transact_ID + ", ActivityType, Amount, TransactionDate, TransactionType " +
                                    "FROM FinancialTransactions " +
                                    "WHERE ActivityType LIKE @activityType " +
                                    "ORDER BY TransactionDate ASC";
                        }
                        else
                        {
                            query = "SELECT " + transact_ID + ", ActivityType, Amount, TransactionDate, TransactionType " +
                                    "FROM FinancialTransactions " +
                                    "WHERE ActivityType = @activityType " +
                                    "ORDER BY TransactionDate ASC";
                        }
                    }
                    else
                    {
                        query = "SELECT " + transact_ID + ", ActivityType, Amount, TransactionDate, TransactionType " +
                                "FROM FinancialTransactions " +
                                "ORDER BY TransactionDate ASC";
                    }


                    FbCommand cmd = new FbCommand(query, conn);
                    if (!string.IsNullOrEmpty(activityTypeFilter))
                    {
                        // Determine the parameter value based on the flag.
                        string paramValue = useWildcards ? "%" + activityTypeFilter + "%" : activityTypeFilter;
                        cmd.Parameters.AddWithValue("@activityType", paramValue);
                    }

                    FbDataAdapter adapter = new FbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Add a new column to store the row number.
                    dt.Columns.Add("No.", typeof(int));
                    // Fill in the row number for each row.
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["No."] = i + 1;
                    }
                    // Set the "RowNumber" column as the first column.
                    dt.Columns["No."].SetOrdinal(0);

                    // Bind the DataTable to the grid.
                    grid.DataSource = dt;

                    // Initialize variables for calculating totals.
                    decimal totalAmount = 0;
                    decimal totalIncome = 0;
                    decimal totalExpense = 0;

                    // Iterate over each row to calculate the totals from the "AMOUNT" column.
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal amount;
                        // Try to parse the value to a decimal.
                        if (decimal.TryParse(row["AMOUNT"].ToString(), out amount))
                        {
                            totalAmount += amount; // Sum of all amounts.

                            // Sum positive values as income.
                            if (amount >= 0)
                            {
                                totalIncome += amount;
                            }
                            // Sum negative values as expenses.
                            else
                            {
                                totalExpense += amount;
                            }
                        }
                    }

                    // Update the provided TextBoxes with the calculated totals.
                    // Format the numbers to display two decimal places.
                    if (textBoxTotal != null)
                        textBoxTotal.Text = totalAmount.ToString("N2");
                    if (textBoxTotalIncome != null)
                        textBoxTotalIncome.Text = totalIncome.ToString("N2");
                    if (textBoxTotalExpense != null)
                        textBoxTotalExpense.Text = totalExpense.ToString("N2");
                }

                // Scroll to and select the last row after loading the data
                ScrollToLastRow(grid);

                // Hide TransactionID column if it exists
                if (grid.Columns["TransactionID"] != null)
                {
                    grid.Columns["TransactionID"].Visible = false;
                }

                // Change the header text of the ActivityType column
                if (grid.Columns["ACTIVITYTYPE"] != null)
                {
                    grid.Columns["ACTIVITYTYPE"].HeaderText = "Activity Type";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "ACTIVITYTYPE", 300);
                }

                // Change the header text of the Amount column
                if (grid.Columns["AMOUNT"] != null)
                {
                    grid.Columns["AMOUNT"].HeaderText = "Amount";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "AMOUNT", 80);
                }

                // Change the header text of the Transaction Date column
                if (grid.Columns["TRANSACTIONDATE"] != null)
                {
                    grid.Columns["TRANSACTIONDATE"].HeaderText = "Transaction Date";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "TRANSACTIONDATE", 200);
                }

                // Change the header text of the Transaction Type column
                if (grid.Columns["TRANSACTIONTYPE"] != null)
                {
                    grid.Columns["TRANSACTIONTYPE"].HeaderText = "Transaction Type";
                    GridViewColumnHelper.SetFixedColumnWidth(grid, "TRANSACTIONTYPE", 170);
                }

                // Subscribe to RowPostPaint event for row numbering
                //dataGridView1.RowPostPaint += dataGridView1_RowPostPaint;
                GridViewColumnHelper.SetFixedColumnWidth(grid, "No.", 50);

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
    }
    // Class Member and Construtor here
    class GridDataViewTransactEventHandler : GridDataViewBaseEventHandler
    {
        // Constructor
        public GridDataViewTransactEventHandler()
        {
            m_IsAddRowInput = true;
            m_IsAllowUserAddRow = false;
            m_IsAddColNum = false;
            m_IsAutoAdjustHeight = false;
            m_ActivityWidth = 327;
            m_AmountWidth = 80;
            m_DateWidth = 150;
            m_TransactionTypeWidth = 140;
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
            m_ActivityWidth = 237;
            m_AmountWidth = 150;
            m_DateWidth = 180;
            m_TransactionTypeWidth = 140;
        }

    }
}
