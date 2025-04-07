using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Services;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
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
        // Methods
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if rows has been fill up
                foreach (DataGridViewRow row in dataGridTransact.Rows)
                {
                    // Create a transaction record from UI fields.
                    FinancialTransactionRecord record = formTransactUIEvent.GetTransactionRecord(row, true);

                    if (!formTransactUIEvent.IsFillupComplete(dataGridTransact, row, record))
                    {
                        return;
                    }
                }

                // Loop through all rows in the dataGridTransact grid
                foreach (DataGridViewRow row in dataGridTransact.Rows)
                {
                    if (formTransactUIEvent.IsFillupValid(dataGridTransact, row))
                    {
                        // Create a transaction record from UI fields.
                        FinancialTransactionRecord record = formTransactUIEvent.GetTransactionRecord(row, true);

                        // Use the encapsulated record.
                        _transactionService.AddTransaction(record);
                    }
                }

                // Clear the input grid after insertion
                dataGridTransact.Rows.Clear();
                formTransactUIEvent.SetupDataGridView(dataGridTransact);

                // Refresh the main data display (if applicable)
                formTransactUIEvent.SetupWindowTabDataGridView(this, tabControlWindow, comboBoxFilter,
                                                        comboBoxBudgetGroup, comboBoxPaymentMethod,
                                                        comboBoxPayee, dateTimePickerStart, dateTimePickerEnd);
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
                if (formTransactUIEvent.DeleteRowDataGridView(tabControlWindow) &&
                    _transactionService.DeleteTransaction(formTransactUIEvent.TransactionID))
                {
                    formTransactUIEvent.SetupWindowTabDataGridView(this, tabControlWindow, comboBoxFilter,
                                                    comboBoxBudgetGroup, comboBoxPaymentMethod,
                                                    comboBoxPayee, dateTimePickerStart,
                                                    dateTimePickerEnd);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting transaction: " + ex.Message);
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Update the dataGridView on tabControlWindow and dataGridTransact with the latest data
            // from the database.
            // Create a transaction record from UI fields.
            if (formTransactUIEvent.SetUpEditFormDataGridView(this, tabControlWindow, dataGridTransact, comboBoxFilter,
                                                          comboBoxBudgetGroup, comboBoxPaymentMethod, comboBoxPayee,
                                                          dateTimePickerStart, dateTimePickerEnd))
            {
                formTransactUIEvent.SetupDataGridView(dataGridTransact);
            }
        }
        private void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedActivity = comboBoxFilter.SelectedItem.ToString();
            string selectedBudgetGroup = comboBoxBudgetGroup.SelectedItem != null ? comboBoxBudgetGroup.SelectedItem.ToString() : string.Empty;
            string selectedPaymentMethod = comboBoxPaymentMethod.SelectedItem != null ? comboBoxPaymentMethod.SelectedItem.ToString() : string.Empty;
            string selectedPayee = comboBoxPayee.SelectedItem != null ? comboBoxPayee.SelectedItem.ToString() : string.Empty;
            DateTime startDate = dateTimePickerStart.Value;
            DateTime endDate = dateTimePickerEnd.Value;

            formTransactUIEvent.ApplyFilter(tabControlWindow, selectedActivity, selectedBudgetGroup,
                                    selectedPaymentMethod,
                                    selectedPayee,
                                    startDate,
                                    endDate);
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = textBoxSearch.Text.Trim();
            formTransactUIEvent.ApplySearch(tabControlWindow, searchTerm, m_transactionID);
        }
        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (formTransactUIEvent.BackupOperation())
            {
                string user = formTransactUIEvent.Username_DB;
                string password = formTransactUIEvent.Password_DB;
                string backupFile = formTransactUIEvent.BackupFile_DB;
                _transactionService.Backup(user, password, backupFile);
            }

            // Export
            // Use the FinancialTransactionService to export the DataTable.
            if (formTransactUIEvent.ExportTabletoCSV(dataGridView1))
            {
                DataTable transactions = formTransactUIEvent.TransactionDataTable;
                string csvFilename = formTransactUIEvent.CSVFilename;
                _transactionService.ExportTransactions(transactions, csvFilename);
            }
        }
        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (formTransactUIEvent.RestoreOperation())
            {
                string user = formTransactUIEvent.Username_DB;
                string password = formTransactUIEvent.Password_DB;
                string backupFile = formTransactUIEvent.BackupFile_DB;
                string restoredDatabaseFile = formTransactUIEvent.RestoreFile_DB;
                _transactionService.Restore(backupFile, user, password, restoredDatabaseFile);
            }

        }
        private void dataGridTransact_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Log the error details. In a production system, replace this with a proper logging framework.
            System.Diagnostics.Debug.WriteLine("DataGridView DataError: " + e.Exception.ToString());
            // Show a custom error message to the user
            // Display a custom error message including the error code (HResult)
            MessageBox.Show($"Please contact Admin for this error. Error Code: {e.Exception.HResult}"
                            + Environment.NewLine + "Check also if you have filled up the Amount Field."
                            ,
                            "Data Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

            // Prevent the default error dialog from appearing
            e.ThrowException = false;
        }
        private void btnShowAll_Click(object sender, EventArgs e)
        {
            formTransactUIEvent.ShowAllColumn(tabControlWindow);
            // Disable the Show All button.
            btnShowAll.Enabled = false;
        }
        private void btnShowAll2_Click(object sender, EventArgs e)
        {
            formTransactUIEvent.ShowAllColumn(tabControlWindow);
            // Disable the Show All button.
            btnShowAll2.Enabled = false;
        }
        private void btnShowAll3_Click(object sender, EventArgs e)
        {
            formTransactUIEvent.ShowAllColumn(tabControlWindow);
            // Disable the Show All button.
            btnShowAll3.Enabled = false;
        }
        private void HideDataGridViewColumn(object sender, int columnIndex)
        {
            // Check that the column index is valid and sender is a DataGridView.
            if (columnIndex >= 0 && sender is DataGridView grid)
            {
                grid.Columns[columnIndex].Visible = false;
            }
        }
        private void dataGridView1_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Confirm a valid column index was clicked.
            HideDataGridViewColumn(sender, e.ColumnIndex);

            // Enable the Show All button.
            btnShowAll.Enabled = true;
        }
        private void dataGridView2_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Confirm a valid column index was clicked.
            HideDataGridViewColumn(sender, e.ColumnIndex);

            // Enable the Show All button.
            btnShowAll2.Enabled = true;
        }

        private void dataGridView3_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Confirm a valid column index was clicked.
            HideDataGridViewColumn(sender, e.ColumnIndex);

            // Enable the Show All button.
            btnShowAll3.Enabled = true;
        }
        private void dataGridTransact_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv.CurrentCell.OwningColumn.Name == "BudgetGroup" ||
                dgv.CurrentCell.OwningColumn.Name == "PaymentMethod" ||
                dgv.CurrentCell.OwningColumn.Name == "Payee" ||
                dgv.CurrentCell.OwningColumn.Name == "Currency")
            {
                ComboBox combo = e.Control as ComboBox;
                if (combo != null)
                {
                    // Allow user to type new values.
                    combo.DropDownStyle = ComboBoxStyle.DropDown;
                }
            }
        }
        private void dataGridTransact_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Check if the cell belongs to one of the editable ComboBox columns.
            string columnName = dataGridTransact.Columns[e.ColumnIndex].Name;
            if (columnName == "BudgetGroup" || columnName == "PaymentMethod" || columnName == "Payee"
                 || columnName == "Currency")
            {
                DataGridViewComboBoxCell cell = dataGridTransact.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                if (cell != null)
                {
                    string newValue = e.FormattedValue.ToString();

                    // If the entered value is not in the Items collection, add it.
                    if (!cell.Items.Contains(newValue))
                    {
                        cell.Items.Add(newValue);
                    }
                }
            }
        }

        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }

}
