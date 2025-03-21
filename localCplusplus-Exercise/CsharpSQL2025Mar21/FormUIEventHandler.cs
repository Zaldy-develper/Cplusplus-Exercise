using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WinFormsApp1SQL.GridViewHelper;

namespace WinFormsApp1SQL
{
    class FormUIEventHandler
    {
        //private DataGridView m_grid { get; set; } 
        protected bool m_isAddRowInput { get; set; }
        protected bool m_isAllowUserAddRow { get; set; }
        protected bool m_isAddColNum { get; set; }
        protected bool m_isAutoAdjustHeight { get; set; }
        protected int m_activityWidth { get; set; }
        protected int m_amountWidth { get; set; }
        protected int m_dateWidth { get; set; }
        private int m_transactionTypeWidth { get; set; }

        // Contructor
        public FormUIEventHandler()
        {
            m_isAddRowInput = true;
            m_isAllowUserAddRow = false;
            m_isAddColNum = false;
            m_isAutoAdjustHeight = false;
            m_activityWidth = 327;
            m_amountWidth = 80;
            m_dateWidth = 150;
            m_transactionTypeWidth = 140;

            //m_isAddRowInput = false;
            //m_isAllowUserAddRow = true;
            //m_isAddColNum = false;
            //m_isAutoAdjustHeight = true;
            //m_activityWidth = 337;
            //m_amountWidth = 80;
            //m_dateWidth = 200;
            //m_transactionTypeWidth = 170;
        }

        // Virtual Method
        public virtual void SetupDataGridView(DataGridView grid)
        {
            grid.Columns.Clear();

            if (m_isAddColNum)
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
            activityColumn.Width = m_activityWidth;

            // 2. Numeric Column for Amount (using a TextBox column with numeric ValueType)
            DataGridViewTextBoxColumn amountColumn = new DataGridViewTextBoxColumn();
            amountColumn.Name = "Amount";
            amountColumn.HeaderText = "Amount";
            amountColumn.ValueType = typeof(decimal);
            grid.Columns.Add(amountColumn);
            amountColumn.Width = m_amountWidth;

            // 3. DateTimePicker Column for Transaction Date
            // Use a custom column that hosts a DateTimePicker.
            CalendarColumn dateColumn = new CalendarColumn();
            dateColumn.Name = "TransactionDate";
            dateColumn.HeaderText = "Transaction Date";
            grid.Columns.Add(dateColumn);
            dateColumn.Width = m_dateWidth;

            // 4. ComboBox Column for Transaction Type
            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn();
            comboColumn.Name = "TransactionType";
            comboColumn.HeaderText = "Transaction Type";
            comboColumn.Items.Add("Income");
            comboColumn.Items.Add("Expense");
            comboColumn.Items.Add("Deposit");
            comboColumn.Items.Add("Withdrawal");
            grid.Columns.Add(comboColumn);
            comboColumn.Width = m_transactionTypeWidth;

            // Adjust the height of Edit Fields
            if (m_isAutoAdjustHeight) GridViewHeightHelper.AdjustDataGridViewHeight(grid);


            // Programmatically add one row for user input. RowUserInput
            if (m_isAddRowInput)
            {

                for (int i = 0; i < 5; i++)
                    grid.Rows.Add();
            }

            // Prevent the dataGridTransact from automatically adding a new row.
            if (!m_isAllowUserAddRow) grid.AllowUserToAddRows = false;
        }
    }

    // Child class inherits from FormUIEventHandler.
    class CustomEditFormUIEventHandler : FormUIEventHandler
    {
        public CustomEditFormUIEventHandler() : base()
        {
            // Set new parameters for the child class.
            m_isAddRowInput = false;
            m_isAllowUserAddRow = true;
            m_isAddColNum = false; // remains false
            m_isAutoAdjustHeight = true;
            m_activityWidth = 237;
            m_amountWidth = 150;
            m_dateWidth = 180;
            // m_dateWidth and m_transactionTypeWidth remain unchanged.
        }

        // Optionally, SetupDataGridView can be if need additional customization.
        // public override void SetupDataGridView(DataGridView grid)
        // {
        //     base.SetupDataGridView(grid);
        //     // Additional customization here.
        // }
    }
}
