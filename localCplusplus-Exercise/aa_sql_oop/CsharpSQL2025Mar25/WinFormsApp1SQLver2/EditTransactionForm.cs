using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1SQL
{
    
    public partial class EditTransactionForm : Form
    {
        // Create instance for the dataGrid of Edit Transact
        GridDataViewEditFormEventHandler formEditTransactUIEvent = new GridDataViewEditFormEventHandler();
        
        // Private field to store the TransactionID because it is not included in the column
        private int _transactionID;

        // Create instance for the Database Operation
        private FinancialTransactionsOperations dbOps_edit = new FinancialTransactionsOperations();

        // Constructor for the EditTransactionForm
        public EditTransactionForm(int transactionID, string activityType, decimal amount, DateTime transactionDate, string transactionType)
        {
            InitializeComponent();
            formEditTransactUIEvent.SetupTransactDataGridView(dataGridEditTransact);

            // Store the TransactionID in a private field for later use
            // (because it is not include in the column of the transaction edit)
            _transactionID = transactionID;

            // Clear any existing rows (if any)
            dataGridEditTransact.Rows.Clear();

            // Add Rows: ActivityType, Amount, TransactionDate, TransactionType
            dataGridEditTransact.Rows.Add(activityType, amount, transactionDate, transactionType);

            // Prevent the DataGridView from automatically adding a new row.
            dataGridEditTransact.AllowUserToAddRows = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate the input grid before proceeding
                if (!formEditTransactUIEvent.IsFillupComplete(dataGridEditTransact)) return;

                // Loop through all rows in the dataGridEditTransact grid
                foreach (DataGridViewRow row in dataGridEditTransact.Rows)
                {
                    if (formEditTransactUIEvent.IsFillupValid(dataGridEditTransact, row))
                    {
                        string activity = formEditTransactUIEvent.Activity;
                        decimal amount = formEditTransactUIEvent.Amount;
                        DateTime transactionDate = formEditTransactUIEvent.TransactionDate;
                        string transactionType = formEditTransactUIEvent.TransactionType;

                        // Update the Database
                        dbOps_edit.UpdateRecord(_transactionID, activity, amount, transactionDate, transactionType);
                    }
                }
               
                // Clear the input grid after insertion
                dataGridEditTransact.Rows.Clear();

                // Set the DialogResult to OK when login is successful
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding transaction: " + ex.Message);
            }
        }
    }
}
