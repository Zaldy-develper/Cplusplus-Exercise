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

        // declare a field for the higher-level service.
        private readonly FinancialTransactionService _transactionService;

        // Constructor
        public EditTransactionForm(int transactionID, string activityType, decimal amount, DateTime transactionDate,
                                    string transactionType, string budgetGroup, string paymentMethod, string payee,
                                    string recurrence, string currency, string description)
        {
            InitializeComponent();

            // Store the TransactionID in a private field for later use
            // (because it is not include in the column of the transaction edit)
            _transactionID = transactionID;

            // Use the factory to create the service instance.
            _transactionService = ServiceFactory.CreateFinancialTransactionService();

            formEditTransactUIEvent.SetupDataGridView(dataGridEditTransact);
            // Clear any existing rows (if any)
            dataGridEditTransact.Rows.Clear();

            // Add a row containing all fields in the expected order.
            // Ensure that the order here matches the order of columns defined in SetupDataGridView.
            dataGridEditTransact.Rows.Add(activityType, amount, transactionType, budgetGroup, paymentMethod,
                                        payee, transactionDate, recurrence, currency, description);

            // Prevent the DataGridView from automatically adding a new row.
            dataGridEditTransact.AllowUserToAddRows = false;

            // Subscribe to the EditingControlShowing event

            dataGridEditTransact.EditingControlShowing += dataGridEditTransact_EditingControlShowing;
            dataGridEditTransact.CellValidating += dataGridEditTransact_CellValidating;
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

                        // Retrieve extra fields.
                        string budgetGroup = formEditTransactUIEvent.BudgetGroup; 
                        string paymentMethod = formEditTransactUIEvent.PaymentMethod;
                        string payee = formEditTransactUIEvent.Payee;
                        string recurrence = formEditTransactUIEvent.Recurrence;
                        string currency = formEditTransactUIEvent.Currency;
                        string description = formEditTransactUIEvent.Description;

                        // Update the Database
                        _transactionService.UpdateTransaction(_transactionID, activity, amount,
                                                                transactionDate, transactionType,
                                                                budgetGroup, paymentMethod, payee,
                                                                recurrence, currency, description);
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
        private void dataGridEditTransact_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
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
        private void dataGridEditTransact_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Check if the cell belongs to one of the editable ComboBox columns.
            string columnName = dataGridEditTransact.Columns[e.ColumnIndex].Name;
            if (columnName == "BudgetGroup" || columnName == "PaymentMethod" || columnName == "Payee"
                 || columnName == "Currency")
            {
                DataGridViewComboBoxCell cell = dataGridEditTransact.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
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
        private void dataGridEditTransact_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Show a custom error message to the user
            MessageBox.Show("Please ask your admin about the error details.");
            // Prevent the default error dialog from appearing
            e.ThrowException = false;
        }
    }
}
