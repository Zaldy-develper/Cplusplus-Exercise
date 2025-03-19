using FirebirdSql.Data.FirebirdClient;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;
using System.Windows.Forms;
using static WinFormsApp1SQL.Form1;

namespace WinFormsApp1SQL
{

    public partial class Form1 : Form
    {
        // Connection string to Firebird database
        private string connectionString;
        private string m_transactionID = "TransactionID";
        public Form1(string fbConnectionString)
        {
            InitializeComponent();

            // Update the connection string with Firebird DB details.
            // C:\Users\RTE\source\repos\zaldy\sql\Project1SQL
            // connectionString = "User=SYSDBA;Password=masterkey;Database=C:\\Path\\To\\FinancialDB.fdb;DataSource=localhost;Port=3050;Dialect=3;";

            // hardcoded
            //connectionString = "User=SYSDBA;Password=masterkey;Database=C:\\Users\\RTE\\source\\repos\\zaldy\\sql\\WinFormsApp1SQL\\FINANCIALDB.FDB;DataSource=localhost;Port=3050;Dialect=3;";

            // path where the program runs
            //string dbPath = Path.Combine(Application.StartupPath, "FINANCIALDB.FDB");
            //connectionString = $"User=SYSDBA;Password=masterkey;Database={dbPath};DataSource=localhost;Port=3050;Dialect=3;";
            connectionString = fbConnectionString;


            //SetupDataGridView();
            // Use the helper to set up the dataGridTransact DataGridView
            GridViewHelper.SetupDataGrid(dataGridTransact, true, false, false, false);
            // public static void SetupDataGrid(DataGridView grid, bool isAddRowInput = false, 
            //bool isAllowUserAddRow = true, bool isAddColNum = false, bool isAutoAdjustHeight = true)


            LoadData();

            // Prevent the DataGridView from automatically adding a new row.
            dataGridView1.AllowUserToAddRows = false;


            this.Shown += Form1_Shown; // Attach Shown event handler

            // Populate the comboBox with unique ActivityType values.
            PopulateActivityTypeComboBox();

        }

        // Load data from the Firebird database into the DataGridView
        private void LoadData(string transact_ID = "TransactionID", string activityTypeFilter = null, bool useWildcards = false)
        {
            try
            {
                using (FbConnection conn = new FbConnection(connectionString))
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

                    // Bind the DataTable to dataGridView1.
                    dataGridView1.DataSource = dt;

                }

                // Scroll to and select the last row after loading the data
                ScrollToLastRow();

                // Hide TransactionID column if it exists
                if (dataGridView1.Columns["TransactionID"] != null)
                {
                    dataGridView1.Columns["TransactionID"].Visible = false;
                }

                // Change the header text of the ActivityType column
                if (dataGridView1.Columns["ACTIVITYTYPE"] != null)
                {
                    dataGridView1.Columns["ACTIVITYTYPE"].HeaderText = "Activity Type";
                    GridViewColumnHelper.SetFixedColumnWidth(dataGridView1, "ACTIVITYTYPE", 300);
                }

                // Change the header text of the Amount column
                if (dataGridView1.Columns["AMOUNT"] != null)
                {
                    dataGridView1.Columns["AMOUNT"].HeaderText = "Amount";
                    GridViewColumnHelper.SetFixedColumnWidth(dataGridView1, "AMOUNT", 80);
                }

                // Change the header text of the Transaction Date column
                if (dataGridView1.Columns["TRANSACTIONDATE"] != null)
                {
                    dataGridView1.Columns["TRANSACTIONDATE"].HeaderText = "Transaction Date";
                    GridViewColumnHelper.SetFixedColumnWidth(dataGridView1, "TRANSACTIONDATE", 200);
                }


                // Change the header text of the Transaction Type column
                if (dataGridView1.Columns["TRANSACTIONTYPE"] != null)
                {
                    dataGridView1.Columns["TRANSACTIONTYPE"].HeaderText = "Transaction Type";
                    GridViewColumnHelper.SetFixedColumnWidth(dataGridView1, "TRANSACTIONTYPE", 170);
                }

                // Subscribe to RowPostPaint event for row numbering
                //dataGridView1.RowPostPaint += dataGridView1_RowPostPaint;
                GridViewColumnHelper.SetFixedColumnWidth(dataGridView1, "No.", 50);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Calculate the row number string
            string rowNumber = (e.RowIndex + 1).ToString();

            // Determine the bounds for drawing the row number in the header cell
            Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, dataGridView1.RowHeadersWidth, e.RowBounds.Height);

            // Draw the row number using TextRenderer
            TextRenderer.DrawText(e.Graphics, rowNumber, dataGridView1.Font, headerBounds, dataGridView1.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //ScrollToLastRow();
        }

        private void ScrollToLastRow()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                int lastRowIndex = dataGridView1.Rows.Count - 1;
                dataGridView1.ClearSelection();
                dataGridView1.Rows[lastRowIndex].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = lastRowIndex;
                // Set the current cell to a visible cell (e.g., the "No." column).
                dataGridView1.CurrentCell = dataGridView1.Rows[lastRowIndex].Cells["No."];

            }
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Loop through all rows in the dataGridTransact grid
                // Check if one of the column in the row has filled
                foreach (DataGridViewRow row in dataGridTransact.Rows)
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
                            MessageBox.Show("Please complete the remaining field. ");
                            return;
                        }
                    }
                }

                // Loop through all rows in the dataGridTransact grid
                foreach (DataGridViewRow row in dataGridTransact.Rows)
                {
                    // Skip the new row placeholder
                    if (row.IsNewRow)
                        continue;

                    // Retrieve values from each column
                    string activity = row.Cells["ActivityType"].Value?.ToString();
                    string amountText = row.Cells["Amount"].Value?.ToString();
                    string dateText = row.Cells["TransactionDate"].Value?.ToString();
                    string transactionType = row.Cells["TransactionType"].Value?.ToString();

                    // Validate required fields (you can add further validation if needed)
                    // Skip Checking of this row if one of column is empty
                    if (string.IsNullOrWhiteSpace(activity) ||
                        string.IsNullOrWhiteSpace(amountText) ||
                        string.IsNullOrWhiteSpace(dateText) ||
                        string.IsNullOrWhiteSpace(transactionType))
                    {
                        // Optionally, skip this row or notify the user
                        continue;
                    }

                    // Parse Amount and TransactionDate
                    if (!Decimal.TryParse(amountText, out decimal amount))
                    {
                        MessageBox.Show("Invalid numeric value for Amount in row " + row.Index);
                        continue;
                    }

                    if (!DateTime.TryParse(dateText, out DateTime transactionDate))
                    {
                        MessageBox.Show("Invalid date value in row " + row.Index);
                        continue;
                    }

                    // Insert each transaction into the database
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

                // Clear the input grid after insertion
                dataGridTransact.Rows.Clear();
                GridViewHelper.SetupDataGrid(dataGridTransact, true, false, false, false);

                // Refresh the main data display (if applicable)
                LoadData();

                // Populate the comboBox with unique ActivityType values.
                PopulateActivityTypeComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding transaction: " + ex.Message);
            }
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a row is selected
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Please select a transaction to delete.");
                    return;
                }

                // Retrieve values from the selected row.
                int transactionID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["TransactionID"].Value);
                string activityType = dataGridView1.CurrentRow.Cells["ActivityType"].Value.ToString();
                decimal amount = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["Amount"].Value);
                DateTime transactionDate = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["TransactionDate"].Value);
                string transactionType = dataGridView1.CurrentRow.Cells["TransactionType"].Value.ToString();
                string itemToDelete = "\n\nItem: " + activityType + ", Amount: Php" + amount + "\nDate:" + transactionDate + " |" + transactionType;
                // Ask for confirmation before deleting
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this transaction?" + itemToDelete,
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // If user clicks "No", do nothing
                if (result == DialogResult.No)
                    return;

                // Open connection and execute DELETE statement
                using (FbConnection conn = new FbConnection(connectionString))
                {
                    conn.Open();
                    using (FbTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string query = "DELETE FROM FinancialTransactions WHERE TransactionID = @TransactionID";
                            using (FbCommand cmd = new FbCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@TransactionID", transactionID);
                                cmd.ExecuteNonQuery();
                            }

                            // Ask again before committing the delete
                            DialogResult finalConfirm = MessageBox.Show(
                                "Do you want to finalize the deletion?" + itemToDelete,
                                "Final Confirmation",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            );

                            if (finalConfirm == DialogResult.Yes)
                            {
                                transaction.Commit(); // Finalize the deletion
                                // Refresh the grid to reflect changes
                                LoadData();
                                MessageBox.Show("Transaction deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                transaction.Rollback(); // Undo the deletion
                                MessageBox.Show("Deletion was canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                        }

                        catch (Exception ex)
                        {
                            transaction.Rollback(); // Undo changes in case of an error
                            MessageBox.Show("Error deleting transaction: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting transaction: " + ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Ensure a row is selected.
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a transaction to edit.");
                return;
            }

            // Retrieve values from the selected row.
            int transactionID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["TransactionID"].Value);
            string activityType = dataGridView1.CurrentRow.Cells["ActivityType"].Value.ToString();
            decimal amount = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["Amount"].Value);
            DateTime transactionDate = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["TransactionDate"].Value);
            string transactionType = dataGridView1.CurrentRow.Cells["TransactionType"].Value.ToString();

            // Create an instance of EditTransactionForm and show it as a dialog
            using EditTransactionForm editForm = new EditTransactionForm(transactionID, activityType, amount, transactionDate, transactionType);
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the grid to reflect the updated record.
                    LoadData();
                }
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Perform a hit test to see where the user clicked.
            DataGridView.HitTestInfo hit = dataGridView1.HitTest(e.X, e.Y);
            if (hit.Type == DataGridViewHitTestType.None)
            {
                // Clear the selection and remove the current cell.
                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;
            }
        }

        private void PopulateActivityTypeComboBox()
        {
            try
            {
                using (FbConnection conn = new FbConnection(ConnectionString.FbConnectionString))
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

        /// Handles the Filter button click event.
        /// Loads data using the selected ActivityType from comboBoxFilter.
        private void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedActivity = comboBoxFilter.SelectedItem.ToString();
            if (selectedActivity == "All")
            {
                LoadData();
            }
            else
            {
                LoadData(activityTypeFilter: selectedActivity);
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
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

                // Get connection parameters.
                // You can retrieve these from your connection string or configuration.
                string user = ConnectionString.m_firebirdUser;       // Replace with your Firebird username
                string password = ConnectionString.m_firebirdPassword;     // Replace with your Firebird password
                string database = ConnectionString.m_databasePath;  // Replace with the path to your database

                // Build the gbak command for backup.
                // The -B switch indicates a backup operation.
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = ConnectionString.m_backupUtilityPath, // Ensure gbak.exe is accessible (in PATH or provide full path)
                    Arguments = $"-B -USER {user} -PASSWORD {password} \"{database}\" \"{backupFile}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                try
                {
                    Process proc = Process.Start(psi);
                    proc.WaitForExit();

                    MessageBox.Show("Backup completed successfully.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        ExportDataGridViewToCSV(dataGridView1, exportFileDialog.FileName);
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = textBoxSearch.Text.Trim();
            LoadData(m_transactionID, searchTerm, true);
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

            // Set the grid's height to the calculated total
            dgv.Height = totalHeight;
        }
    }

    public static class GridViewHelper
    {
        public static void SetupDataGrid(DataGridView grid, bool isAddRowInput = false,
            bool isAllowUserAddRow = true, bool isAddColNum = false, bool isAutoAdjustHeight = true,
            int activityWidth = 337, int amountWidth = 80)
        {
            grid.Columns.Clear();

            if (isAddColNum)
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
            activityColumn.Width = activityWidth;

            // 2. Numeric Column for Amount (using a TextBox column with numeric ValueType)
            DataGridViewTextBoxColumn amountColumn = new DataGridViewTextBoxColumn();
            amountColumn.Name = "Amount";
            amountColumn.HeaderText = "Amount";
            amountColumn.ValueType = typeof(decimal);
            grid.Columns.Add(amountColumn);
            amountColumn.Width = amountWidth;

            // 3. DateTimePicker Column for Transaction Date
            // Use a custom column that hosts a DateTimePicker.
            CalendarColumn dateColumn = new CalendarColumn();
            dateColumn.Name = "TransactionDate";
            dateColumn.HeaderText = "Transaction Date";
            grid.Columns.Add(dateColumn);
            dateColumn.Width = 200;

            // 4. ComboBox Column for Transaction Type
            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn();
            comboColumn.Name = "TransactionType";
            comboColumn.HeaderText = "Transaction Type";
            comboColumn.Items.Add("Income");
            comboColumn.Items.Add("Expense");
            comboColumn.Items.Add("Deposit");
            comboColumn.Items.Add("Withdrawal");
            grid.Columns.Add(comboColumn);
            comboColumn.Width = 170;

            // Adjust the height of Edit Fields
            if (isAutoAdjustHeight) GridViewHeightHelper.AdjustDataGridViewHeight(grid);


            // Programmatically add one row for user input. RowUserInput
            if (isAddRowInput)
            {

                for (int i = 0; i < 5; i++)
                    grid.Rows.Add();
            }

            // Prevent the dataGridTransact from automatically adding a new row.
            if (!isAllowUserAddRow) grid.AllowUserToAddRows = false;
        }

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
        /// <summary>
        /// Sets a fixed width for a specified column by disabling auto sizing.
        /// </summary>
        /// <param name="grid">The DataGridView containing the column.</param>
        /// <param name="columnName">The name of the column to adjust.</param>
        /// <param name="width">The fixed width to set.</param>
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

        /// <summary>
        /// Reads a value from an INI file.
        /// </summary>
        public static string Read(string section, string key, string filePath, string defaultValue = "")
        {
            StringBuilder retVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, retVal, 255, filePath);
            return retVal.ToString();
        }

        /// <summary>
        /// Writes a value to an INI file.
        /// </summary>
        public static void Write(string section, string key, string value, string filePath)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }
    }

    /// <summary>
    /// Populates comboBoxFilter with unique ActivityType values from the database.
    /// An "All" option is added to allow displaying all records.
    /// </summary>
    

    }
