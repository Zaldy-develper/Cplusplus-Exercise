using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1SQL
{
    partial class Form1 : Form
    {
        // ---------------------------
        // Properties
        // ---------------------------

        // Create instance for the dataGridView :dataGridViewTransact
        private GridDataViewTransactEventHandler formTransactUIEvent = new GridDataViewTransactEventHandler();
        // Declare a field for the higher-level service.
        private readonly FinancialTransactionService _transactionService;
        private string m_transactionID = "TransactionID";

        // Constructor
        public Form1()
        {
            InitializeComponent();
            // Use the factory to create the service instance.
            _transactionService = ServiceFactory.CreateFinancialTransactionService();
            formTransactUIEvent.SetupDataGridView(dataGridTransact);
            formTransactUIEvent.SetupWindowTabDataGridView(this, tabControlWindow, comboBoxFilter);
        }

    }
}
