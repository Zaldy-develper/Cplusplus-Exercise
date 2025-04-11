using static Ipv4SocketCommunication.ValidationFlow;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Data;
using System.Net.Sockets;
using System.Text;
using Ipv4SocketCommunication.Interfaces; // Contains IUpdateNotifier and IClientParameters

namespace Ipv4SocketCommunication
{
    public partial class Form1 : Form, IUpdateNotifier
    {
        private string logStartMessage = "Welcome to my Chatroom:\n";
        public Form1()
        {
            InitializeComponent();

            // Subscribe to the Load event.
            this.Load += Form1_Load;
            //config = new ConfigParams(ipAddMaskText, portText, savePathText, statusText);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Load the configuration data into the controls.
            LoadConfigData();

            // Update status
            StatusUpdater.TextBoxUpdater("", statusText);

            // Set the initial focus on the IP address text box.
            this.ActiveControl = ipAddMaskText;
        }

        #region IUpdateNotifier Implementation

        // Update the status text on the status TextBox.
        public void UpdateStatus(string message)
        {
            if (statusText.InvokeRequired)
            {
                statusText.Invoke(new Action(() =>
                {
                    statusText.AppendText(message + Environment.NewLine);
                    statusText.SelectionStart = statusText.Text.Length;
                    statusText.ScrollToCaret();
                }));
            }
            else
            {
                statusText.AppendText(message + Environment.NewLine);
                statusText.SelectionStart = statusText.Text.Length;
                statusText.ScrollToCaret();
            }
        }

        // LogMessage on the Message History RichTextBox.
        public void LogMessage(string message)
        {
            bool IsClearLogExecute = false;

            if (message == logStartMessage)
            {
                IsClearLogExecute = true;
            }
            if (messageHistoryRichText.InvokeRequired)
            {
                messageHistoryRichText.Invoke(new Action(() =>
                {
                    ClearLogMessageContents(IsClearLogExecute);
                    messageHistoryRichText.AppendText(message + Environment.NewLine);
                    messageHistoryRichText.SelectionStart = messageHistoryRichText.Text.Length;
                    messageHistoryRichText.ScrollToCaret();
                }));
            }
            else
            {
                ClearLogMessageContents(IsClearLogExecute);
                messageHistoryRichText.AppendText(message + Environment.NewLine);
                messageHistoryRichText.SelectionStart = messageHistoryRichText.Text.Length;
                messageHistoryRichText.ScrollToCaret();
            }
        }
        public void ClearLogMessageContents(bool ClearMe)
        {
            if (ClearMe)
            {
                messageHistoryRichText.Clear();
            }
        }
        // Exposes the file path text value.
        public string SaveFilePath => savePathText.Text;
        // Exposes the server message string (from the serverText TextBox).
        public string ServerMessage => serverText.Text;

        #endregion

        #region Button Handlers
        private void btnStart_Click(object sender, EventArgs e)
        {
            // Set Focus
            statusText.Focus();
            ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, btnSend, false);

            if (isValidEntry())
            {
                // Saving Set-up Configuration Ini File
                SaveConfigData();

                // Initialize the SocketHandler instance since it's not already set.
                if (SocketHandler.Instance == null)
                {
                    int port = int.Parse(portText.Text);  // Ensure proper parsing and error checking.
                    string ipAddress = ipAddMaskText.Text;
                    new SocketHandler(port, ipAddress);  // The constructor sets Instance = this.
                }

                // Set the initial text for the message history.
                RichTextBox_FontColorUpdater.SetRichTextBox_FontColor(messageHistoryRichText);
                LogMessage("Welcome to my Chatroom:\n");

                // Enable Now the Stop and Send buttons.
                ButtonHandler.StopButtonActive(btnStop, true);
                ButtonHandler.SendButtonActive(btnSend, true);

                // Start the server on a new background thread.
                Thread serverThread = new Thread(StartServerThread)
                {
                    IsBackground = true
                };
                serverThread.Start();
            }

        }

        // This thread method calls the SocketHandler using the notifier (this).
        private void StartServerThread()
        {
            SocketHandler.Instance.StartServer(this);
        }

        private void btnSavePath_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "All Files (*.*)|*.*",
                // You can use Application.StartupPath if desired:
                // InitialDirectory = Application.StartupPath,
                InitialDirectory = @"C:\Users\RTE\Desktop",
                FileName = "newconnectionstatus-CsharpVersion.txt",
                ValidateNames = true,
                CheckPathExists = true
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                // Update your text boxes with the selected file name.
                TextBox_FontColorUpdater.SetTextBox_FontColor(savePathText);
                string message = "\nConnection Save File Path Location: ";
                StatusUpdater.TextBoxUpdater(message, statusText);

                System.Threading.Thread.Sleep(300);
                StatusUpdater.TextBoxUpdater(ToString(), statusText);
                message = saveDialog.FileName + "\n";
                StatusUpdater.TextBoxUpdater(message, statusText);
                savePathText.Text = saveDialog.FileName;
            }
        }
        private async void btnStop_Click(object sender, EventArgs e)
        {
            // Set Focus
            statusText.Focus();
            UpdateStatus("All connections have been stopped. Suspend.");

            // Stop the server by calling the instance.
            if (SocketHandler.Instance != null)
            {
                SocketHandler.Instance.StopServer();
            }

            // Await a 3 second delay without freezing the UI.
            await Task.Delay(3000);

            ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, btnSend, true);

            // Option: close the form if that is desired:
            // this.Close();
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            
            // Ensure the SocketHandler instance exists and that there is at least one client.
            if (SocketHandler.Instance == null || SocketHandler.Instance.GetClientSockets.Count == 0)
            {
                UpdateStatus("No clients are currently connected.");
                return;
            }
            if (string.IsNullOrWhiteSpace(serverText.Text) || serverText.Text == DefaultItems.StatusDefaultText)
            {
                UpdateStatus("Please enter a message to send.");
                return;
            }

            int sel_clientNumber = 0;
            Socket selectedSocket = null;
            var clientSockets = SocketHandler.Instance.GetClientSockets;

            // If exactly one client is connected, pick that one.
            if (clientSockets.Count == 1)
            {
                selectedSocket = clientSockets[0];
            }
            else
            {
                // More than one client: let the user choose using a modal dialog.
                using (ClientSelectionForm selectionForm = new ClientSelectionForm())
                {
                    int clientNumber = 1;
                    // Populate the ListBox with connected client info.
                    foreach (Socket sock in clientSockets)
                    {
                        selectionForm.clientListBox.Items.Add("Client No." + clientNumber.ToString() +
                            " on Socket:"+ sock.Handle.ToInt32() + "|" + sock.RemoteEndPoint.ToString());
                        clientNumber++;
                    }

                    // Select the first item by default.
                    if (selectionForm.clientListBox.Items.Count > 0)
                        selectionForm.clientListBox.SelectedIndex = 0;

                    // Show the selection form as a modal dialog.
                    if (selectionForm.ShowDialog() == DialogResult.OK)
                    {
                        int selectedIndex = selectionForm.clientListBox.SelectedIndex;
                        if (selectedIndex >= 0)
                        {
                            selectedSocket = clientSockets[selectedIndex];
                            sel_clientNumber = selectedIndex + 1; // Adjust for 0-based index.
                        }
                        else
                        {
                            StatusUpdater.TextBoxUpdater("No client selected.", statusText);
                            return;
                        }
                    }
                    else
                    {
                        // The user cancelled the selection dialog.
                        return;
                    }
                }
            }
            // Create an adapter for client-specific parameters.
            ClientParameters clientParams = new ClientParameters(selectedSocket, this, sel_clientNumber);
            if (!SocketHandler.Instance.MessageSendToClient(clientParams))
            {
                UpdateStatus("Failed to send message to the selected client.");
            }
            else
            {
                // Prepare and write the message to a file.
                DefaultItems.InitializeMessageToClient(serverText);
                string messageToSend = DefaultItems.MessageToSend;
                byte[] messageBytes = Encoding.ASCII.GetBytes(messageToSend);
                if (!FileWriter.WriteBufferToFile(savePathText, messageBytes, messageBytes.Length))
                {
                    UpdateStatus("Failed to write message to file.");
                }
                // Clear or reset the serverText TextBox.
                TextBox_FontColorUpdater.SetTextBox_FontColor(serverText, System.Drawing.Color.Gray);
                serverText.Text = DefaultItems.StatusDefaultText;
            }
        }
        #endregion

    }
}
