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

        // Private field to store the record pass from the main form
        private FinancialTransactionRecord _record;

        // Declare a field for the higher-level service.
        private readonly FinancialTransactionService _transactionService;

        // Constructor
        public EditTransactionForm(FinancialTransactionRecord record)
        {
            InitializeComponent();

            // Use the factory to create the service instance.
            _transactionService = ServiceFactory.CreateFinancialTransactionService();

            formEditTransactUIEvent.SetupDataGridView(dataGridEditTransact);
            // Clear any existing rows (if any)
            dataGridEditTransact.Rows.Clear();

            // Add a row containing all fields in the expected order.
            // Ensure that the order here matches the order of columns defined in SetupDataGridView.
            dataGridEditTransact.Rows.Add(
                record.Activity,
                record.Amount,
                record.TransactionType,
                record.BudgetGroup,
                record.PaymentMethod,
                record.Payee,
                record.TransactionDate,
                record.Recurrence,
                record.Currency,
                record.Description);

            // Prevent the DataGridView from automatically adding a new row.
            dataGridEditTransact.AllowUserToAddRows = false;

            // Store the record in the private field from the main form
            _record = record;

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
                // Create a transaction record from UI fields.
                FinancialTransactionRecord recordEdit = formEditTransactUIEvent.GetTransactionRecord(dataGridEditTransact.CurrentRow, true);

                // Transaction ID and RowNumber is not included in the column, so we need to set it manually
                recordEdit.TransactionID = _record.TransactionID;
                recordEdit.RowNumber = _record.RowNumber;

                if (recordEdit.TransactionID == null)
                {
                    MessageBox.Show("Transaction ID is not valid:" + recordEdit.TransactionID);
                    return;
                }
                else
                {
                    // Check if the transaction ID is valid
                    MessageBox.Show("Processing the modification of the Transaction No." + recordEdit.RowNumber);
                }

                // Validate the input grid before proceeding
                if (!formEditTransactUIEvent.IsFillupComplete(dataGridEditTransact, dataGridEditTransact.CurrentRow, recordEdit)) return;

                // Loop through all rows in the dataGridEditTransact grid
                foreach (DataGridViewRow row in dataGridEditTransact.Rows)
                {
                    if (formEditTransactUIEvent.IsFillupValid(dataGridEditTransact, row, recordEdit, false))
                    {
                        // Update the Database
                        _transactionService.UpdateTransaction(recordEdit);
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
    }
}
