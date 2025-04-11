using static Ipv4SocketCommunication.ValidationFlow;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Data;
using System.Net.Sockets;
using System.Text;

namespace Ipv4SocketCommunication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Subscribe to the Load event.
            this.Load += Form1_Load;
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
                string messageHistoryLog = "Welcome to my Chatroom: \n";
                MessageLogger.MessageHistoryLog(messageHistoryRichText, messageHistoryLog, true);

                // Enable Now the Stop and Send buttons.
                ButtonHandler.StopButtonActive(btnStop, true);
                ButtonHandler.SendButtonActive(btnSend, true);

                // Start the server on a new background thread.
                Thread serverThread = new Thread(new ThreadStart(StartServerThread));
                serverThread.IsBackground = true;
                serverThread.Start();
            }

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
                //statusText.Text = saveDialog.FileName;
                TextBox_FontColorUpdater.SetTextBox_FontColor(savePathText);
                string message = "\nConnection Save File Path Location: ";
                StatusUpdater.TextBoxUpdater(message, statusText);

                System.Threading.Thread.Sleep(300);
                StatusUpdater.TextBoxUpdater(ToString(), statusText);
                //statusText.AppendText(saveDialog.FileName + Environment.NewLine);
                message = saveDialog.FileName + "\n";
                StatusUpdater.TextBoxUpdater(message, statusText);
                savePathText.Text = saveDialog.FileName;
            }
        }
        private async void btnStop_Click(object sender, EventArgs e)
        {
            // Set Focus
            statusText.Focus();

            StatusUpdater.TextBoxUpdater(Environment.NewLine + "All connection has been stopped. \nSuspend." + Environment.NewLine, statusText);

            // Stop the server by calling the static StopServer method.
            SocketHandler.StopServer(); // This calls the instance Shutdown() method.

            // Await a 3 second delay without freezing the UI.
            await Task.Delay(3000);

            ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, btnSend, true);

            // Option: close the form if that is desired:
            // this.Close();
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            int sel_clientNumber = 0;
            // Ensure the SocketHandler instance exists and that there is at least one client.
            if (SocketHandler.Instance == null || SocketHandler.Instance.GetClientSockets.Count == 0)
            {
                StatusUpdater.TextBoxUpdater("No clients are currently connected.", statusText);
                return;
            }

            // Check if the server message textbox is empty.
            if (string.IsNullOrWhiteSpace(serverText.Text) || serverText.Text == DefaultItems.StatusDefaultText)
            {
                StatusUpdater.TextBoxUpdater("Please enter a message to send.", statusText);
                return;
            }

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
            ClientThreadParams clientThreadParams = new ClientThreadParams(selectedSocket, statusText,
                            savePathText, sel_clientNumber, messageHistoryRichText, serverText);
            // Attempt to send the message to the selected client.
            if (!SocketHandler.Instance.MessageSendToClient(clientThreadParams))
            {
                StatusUpdater.TextBoxUpdater("Failed to send message to the selected client.", statusText);
            }
            else
            {
                // Write the server message to file using the overload that takes the file path TextBox and message TextBox.
                // (Convert the message to a byte array and pass its length.)
                DefaultItems.InitializeMessageToClient(serverText);
                string messageToSend = DefaultItems.MessageToSend;
                
                byte[] messageBytes = Encoding.ASCII.GetBytes(messageToSend);
                if (!FileWriter.WriteBufferToFile(savePathText, messageBytes, messageBytes.Length))
                {
                    StatusUpdater.TextBoxUpdater("Failed to write message to file.", statusText);
                }
                // Clear the message textbox after sending.
                TextBox_FontColorUpdater.SetTextBox_FontColor(serverText, Color.Gray);
                serverText.Text = DefaultItems.StatusDefaultText;
            }
        }
    }
}
