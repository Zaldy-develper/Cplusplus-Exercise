using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualBasic;
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
        // Create instance for the dataGridView :dataGridViewTransact
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
        }
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
                        dbOps.AddRecord(activity, amount, transactionDate, transactionType);
                    }
                }

                // Clear the input grid after insertion
                dataGridTransact.Rows.Clear();
                formTransactUIEvent.SetupTransactDataGridView(dataGridTransact);

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
            dbOps.BackupFirebirdDatabase(dataGridView1);
        }
        private void btnRestore_Click(object sender, EventArgs e)
        {
            dbOps.RestoreFirebirdDatabase();
        }
        
    }

}
