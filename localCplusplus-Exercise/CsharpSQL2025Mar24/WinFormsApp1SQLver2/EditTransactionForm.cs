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

            // Column order: ActivityType, Amount, TransactionDate, TransactionType
            dataGridEditTransact.Rows.Add(activityType, amount, transactionDate, transactionType);

            //// Programmatically add one row for user input.
            //dataGridEditTransact.Rows.Add();

            // Prevent the DataGridView from automatically adding a new row.
            dataGridEditTransact.AllowUserToAddRows = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Loop through all rows in the dataGridTransact grid
                foreach (DataGridViewRow row in dataGridEditTransact.Rows)
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
                    using (FbConnection conn = new FbConnection(UIEventHandlers.m_FbConnectionString))
                    {
                        conn.Open();

                        string query = @"UPDATE FinancialTransactions 
                                 SET ActivityType = @ActivityType, 
                                     Amount = @Amount, 
                                     TransactionDate = @TransactionDate, 
                                     TransactionType = @TransactionType 
                                 WHERE TransactionID = @TransactionID";
                        using (FbCommand cmd = new FbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@TransactionID", _transactionID);
                            cmd.Parameters.AddWithValue("@ActivityType", activity);
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.Parameters.AddWithValue("@TransactionDate", transactionDate);
                            cmd.Parameters.AddWithValue("@TransactionType", transactionType);

                            cmd.ExecuteNonQuery();
                        }
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
