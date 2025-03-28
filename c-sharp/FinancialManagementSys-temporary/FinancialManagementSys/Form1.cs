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
                if (!formTransactUIEvent.IsFillupComplete(dataGridTransact)) return;

                // Loop through all rows in the dataGridTransact grid
                foreach (DataGridViewRow row in dataGridTransact.Rows)
                {
                    if (formTransactUIEvent.IsFillupValid(dataGridTransact, row))
                    {
                        string activity = formTransactUIEvent.Activity;
                        decimal amount = formTransactUIEvent.Amount;
                        DateTime transactionDate = formTransactUIEvent.TransactionDate;
                        string transactionType = formTransactUIEvent.TransactionType;

                        // Insert each transaction into the database
                        _transactionService.AddTransaction(activity, amount, transactionDate, transactionType);
                    }
                }

                // Clear the input grid after insertion
                dataGridTransact.Rows.Clear();
                formTransactUIEvent.SetupDataGridView(dataGridTransact);

                // Refresh the main data display (if applicable)
                formTransactUIEvent.SetupWindowTabDataGridView(this, tabControlWindow, comboBoxFilter);
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
                bool deletionSuccessful = _transactionService.DeleteTransaction(transactionID);

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
        private void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedActivity = comboBoxFilter.SelectedItem.ToString();
            formTransactUIEvent.ApplyFilter(tabControlWindow, selectedActivity);
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
            // Show a custom error message to the user
            MessageBox.Show("Please enter a valid Amount or pick a valid Date.");
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
       
        
    }

}
