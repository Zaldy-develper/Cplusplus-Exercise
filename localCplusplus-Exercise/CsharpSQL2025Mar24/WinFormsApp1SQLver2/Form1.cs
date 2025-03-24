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
        // Create instance for the dataGrid of Transact
        private GridDataViewTransactEventHandler formTransactUIEvent = new GridDataViewTransactEventHandler();

        // Create instance for the Database Operation
        private FinancialTransactionsOperations dbOps = new FinancialTransactionsOperations();

        private string m_transactionID = "TransactionID";

        // Constructor
        public Form1()
        {
            InitializeComponent();
            formTransactUIEvent.SetupTransactDataGridView(dataGridTransact);
            formTransactUIEvent.SetupWindowTabDataGridView(this, tabControlWindow, comboBoxFilter);
            this.Shown += Form1_Shown; // Attach Shown event handler
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
                                MessageBox.Show("Amount should be less than 0");
                                return;
                            }
                            // For "Income" or "Deposit", the amount should be positive.
                            else if ((transactionType.Equals("Income", StringComparison.OrdinalIgnoreCase) ||
                                      transactionType.Equals("Deposit", StringComparison.OrdinalIgnoreCase)) &&
                                      amount <= 0)
                            {
                                // Skip this row if the condition is not satisfied.
                                MessageBox.Show("Amount should be greater than 0");
                                return;
                            }
                        }
                    }
                }

                // Loop through all rows in the dataGridTransact grid
                foreach (DataGridViewRow row in dataGridTransact.Rows)
                //for (int i = dataGridTransact.Rows.Count - 1; i >= 0; i--)
                {
                    //DataGridViewRow row = dataGridTransact.Rows[i];

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
                    dbOps.AddRecord(activity, amount, transactionDate, transactionType);
                }

                // Clear the input grid after insertion
                dataGridTransact.Rows.Clear();
                formTransactUIEvent.SetupTransactDataGridView(dataGridTransact);

                // Refresh the main data display (if applicable)
                formTransactUIEvent.SetupWindowTabDataGridView(this, tabControlWindow, comboBoxFilter);

                // Populate the comboBox with unique ActivityType values.
                //PopulateActivityTypeComboBox();
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
                string itemToDelete = "Item: " + activityType + ", Amount: Php" + amount + "\nDate:" + transactionDate + " |" + transactionType;
                // Ask for confirmation before deleting
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this transaction?\n\n" + itemToDelete,
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // If user clicks "No", do nothing
                if (result == DialogResult.No)
                    return;

                // Call DeleteRecord and store the result.
                bool deletionSuccessful = dbOps.DeleteRecord(transactionID, this, tabControlWindow, comboBoxFilter, itemToDelete);

                // If deletion was finalized, refresh the DataGridView.
                if (deletionSuccessful)
                {
                    formTransactUIEvent.SetupWindowTabDataGridView(this, tabControlWindow, comboBoxFilter);
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
                    // Refresh the main data display (if applicable)
                    formTransactUIEvent.SetupWindowTabDataGridView(this, tabControlWindow, comboBoxFilter);
                }
            }
        }

        

        // Handles the Filter/Category button click event.
        // Loads data using the selected ActivityType from comboBoxFilter.
        private void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedActivity = comboBoxFilter.SelectedItem.ToString();
            formTransactUIEvent.ApplyFilter(tabControlWindow, selectedActivity);
        }
        // Handles the Filter/Category button click event.
        // Loads data using the selected ActivityType from textBoxSearch.
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = textBoxSearch.Text.Trim();
            formTransactUIEvent.ApplySearch(tabControlWindow, searchTerm, m_transactionID);
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
                string user = UIEventHandlers.m_firebirdUser;       // Replace with your Firebird username
                string password = UIEventHandlers.m_firebirdPassword;     // Replace with your Firebird password
                string database = UIEventHandlers.m_databasePath;  // Replace with the path to your database

                // Build the gbak command for backup.
                // The -B switch indicates a backup operation.
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = UIEventHandlers.m_backupUtilityPath, // Ensure gbak.exe is accessible (in PATH or provide full path)
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

        //public static void GetFBConnectionString()
        //{
        //    m_connectionString = UIEventHandlers.m_FbConnectionString;
        //}
    }

    /// <summary>
    /// Populates comboBoxFilter with unique ActivityType values from the database.
    /// An "All" option is added to allow displaying all records.
    /// </summary>
    

}
