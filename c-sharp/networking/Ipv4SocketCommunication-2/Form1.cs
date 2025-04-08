using static Ipv4SocketCommunication.ValidationFlow;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Data;

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
            LoadConfigData(ipAddMaskText, portText, savePathText, statusText);

            // Optionally, update status (if needed—here, we're passing an empty string).
            StatusUpdater.ReceiveStatus("", statusText);

            // Set the initial focus on the IP address text box.
            this.ActiveControl = ipAddMaskText;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, false);
            if (isValidEntry())
            {
                // Saving Set-up Configuration Ini File
                SaveConfigData(ipAddMaskText, portText, savePathText, statusText);

                // Initialize the SocketHandler instance since it's not already set.
                if (SocketHandler.Instance == null)
                {
                    int port = int.Parse(portText.Text);  // Ensure proper parsing and error checking.
                    string ipAddress = ipAddMaskText.Text;
                    new SocketHandler(port, ipAddress);  // The constructor sets Instance = this.
                }

                // Set the initial text for the message history.
                RichTextBox_FontColorUpdater.SetRichTextBox_FontColor(messageHistoryRichText);
                string messageHistoryLog = "Message History: \n";
                MessageLogger.MessageHistoryLog(messageHistoryRichText, messageHistoryLog, true);

                // Start the server on a new background thread.
                Thread serverThread = new Thread(new ThreadStart(StartServerThread));
                serverThread.IsBackground = true;
                serverThread.Start();
            }

        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
        /*==========================================================================*
         *      Custom Functions
         *						@                                   				*
         *==========================================================================*/
        private void StartServerThread()
        {
            // Assuming SocketHandler.Instance was correctly initialized earlier,
            // you call its StartServer method. Since StartServer blocks while waiting for connections,
            // running it on a separate thread prevents the UI from freezing.
            SocketHandler.Instance.StartServer(statusText, savePathText, messageHistoryRichText);
        }

        //Validation Flow of the Input Fields 
        private bool isValidEntry()
        {
            bool success = false; // Initialize success to true
            // Setting the status text to indicate the start of the process.
            StatusUpdater.ReceiveStatus("Performing Configuration Set-up." + Environment.NewLine, statusText);

            // Get the parameters for the CustomInput.
            var parameters = new CustomInputParameters
            {
                Ip = ipAddMaskText.Text,
                Port = portText.Text,
                FilePath = savePathText.Text,
                IpMaskedTextBox = ipAddMaskText
            };

            CustomInput input = new CustomInput(parameters); // Assign my CustomInput
            string errorMessage = string.Empty;

            // Build the validation flow using the configuration.
            List<ValidationFlow.ValidationBlock> blocks = ValidationFlowConfig.SetupValidationBlocks();

            // For demonstration: show that each block is initialized.
            for (int i = 0; i < blocks.Count; i++)
            {
                StatusUpdater.ReceiveStatus("Administator checking only:" + "\nValidation Block[" + i + "] initialized. " +
                                "\nTotal Validation Block size is " + blocks.Count + ".", statusText);

            }

            // Execute the validation flow starting from the first block.
            if (blocks.Count > 0)
            {
                ValidationFlow.FlowProcessor.RunFlow(blocks[0], input, ref errorMessage);
            }

            // Display the outcome of the validation.
            if (string.IsNullOrEmpty(errorMessage))
            {
                // Create an array of messages showing the input details.
                string[] serverSetup = new string[]
                {
                    " ", // New line for clarity
                    "Configuration Set-up:",
                    "Ip address: " + ipAddMaskText.Text,
                    "Port Number: " + portText.Text,
                    "Save File Path: " + savePathText.Text
                };

                // Display each message.
                foreach (string line in serverSetup)
                {
                    StatusUpdater.ReceiveStatus(line, statusText);
                }

                StatusUpdater.ReceiveStatus("Input Validation succeeded.", statusText);
                ButtonHandler.StopButtonActive(btnStop, true);
                success = true; // Set success to true
            }
            else
            {
                // If there is an error, set the focus based on the last field validated.
                ValidationFlow.FocusPosition cursorPosition = input.LastPosFocus;

                switch (cursorPosition)
                {
                    case ValidationFlow.FocusPosition.IP:
                        ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, true);
                        ipAddMaskText.Focus();
                        break;
                    case ValidationFlow.FocusPosition.PORT:
                        ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, true);
                        portText.Focus();
                        break;
                    case ValidationFlow.FocusPosition.PATH:
                        ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, true);
                        savePathText.Focus();
                        break;
                    case ValidationFlow.FocusPosition.DEFAULT:
                        ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, true);
                        ipAddMaskText.Focus();
                        break;
                }

                // Report the error message.
                StatusUpdater.ReceiveStatus(errorMessage, statusText);
                return success;
            }
            return success;
        }
        //public static void ReceiveStatus(string greeting, TextBox statusText)
        //{
        //    if (statusText.InvokeRequired)
        //    {
        //        statusText.Invoke(new Action<string, TextBox>(ReceiveStatus), greeting, statusText);
        //    }
        //    else
        //    {
        //        SetTextBox_FontColor(statusText);
        //        statusText.AppendText(greeting + Environment.NewLine);
        //        statusText.Refresh(); // Force the UI to update immediately
        //    }

        //    System.Threading.Thread.Sleep(300);
        //}
        
        public static class ButtonHandler
        {
            public static void StartButtonActive(Button btnStart, bool isEnable)
            {
                btnStart.Enabled = isEnable;
            }
            public static void StopButtonActive(Button btnStop, bool isEnable)
            {
                btnStop.Enabled = isEnable;
            }
            public static void StartStopPathActive(Button btnStart, Button btnStop, Button btnSavePath, bool isEnable)
            {
                if (isEnable)
                {
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    btnSavePath.Enabled = true;
                }
                else
                {
                    btnStart.Enabled = false;
                    //btnStop.Enabled = true;
                    btnSavePath.Enabled = false;
                }
            }
            //public static void SavePath(Button btnSavePath, bool isEnable)
            //{
            //    btnSavePath.Enabled = isEnable;
            //}
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
                StatusUpdater.ReceiveStatus(message, statusText);

                System.Threading.Thread.Sleep(300);
                StatusUpdater.ReceiveStatus(ToString(), statusText);
                //statusText.AppendText(saveDialog.FileName + Environment.NewLine);
                message = saveDialog.FileName + "\n";
                StatusUpdater.ReceiveStatus(message, statusText);
                savePathText.Text = saveDialog.FileName;
            }
        }
        private async void btnStop_Click(object sender, EventArgs e)
        {
            StatusUpdater.ReceiveStatus(Environment.NewLine + "All connection has been stopped. \nSuspend." + Environment.NewLine, statusText);

            // Stop the server by calling the static StopServer method.
            SocketHandler.StopServer(); // This calls the instance Shutdown() method.

            // Await a 3 second delay without freezing the UI.
            await Task.Delay(3000);

            ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, true);

            // Option: close the form if that is desired:
            // this.Close();
        }
        public void SaveConfigData(MaskedTextBox ipAddMaskText, TextBox portText, TextBox savePathText, TextBox statusText)
        {
            try
            {
                // Determine the path for the INI file in the application's startup directory.
                string iniFilePath = Path.Combine(Application.StartupPath, "config.ini");

                // Create a StreamWriter to write to the INI file. 
                // The 'false' parameter indicates that the file should be overwritten if it exists.
                using (StreamWriter sw = new StreamWriter(iniFilePath, false))
                {
                    sw.WriteLine("[Settings]");
                    sw.WriteLine("IpAddress=" + ipAddMaskText.Text);
                    sw.WriteLine("Port=" + portText.Text);
                    sw.WriteLine("SaveFilePath=" + savePathText.Text);
                }

                // Update the UI status to indicate success.
                StatusUpdater.ReceiveStatus("Configuration saved to " + iniFilePath, statusText);
            }
            catch (Exception ex)
            {
                // Update the UI status in case of an error.
                StatusUpdater.ReceiveStatus("Error saving configuration: " + ex.Message, statusText);
            }
        }

        public void LoadConfigData(MaskedTextBox ipAddMaskText, TextBox portText, TextBox savePathText, TextBox statusText)
        {
            // Determine the INI file path (located in the application's startup directory).
            string iniFilePath = Path.Combine(Application.StartupPath, "config.ini");

            // Check if the configuration file exists.
            if (File.Exists(iniFilePath))
            {
                try
                {
                    // Open the file using a StreamReader.
                    using (StreamReader sr = new StreamReader(iniFilePath))
                    {
                        // Read until the end of the file.
                        while (!sr.EndOfStream)
                        {
                            // Read a line from the file.
                            string line = sr.ReadLine();

                            // Skip if the line is empty or contains a section header (e.g., [Settings]).
                            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("["))
                            {
                                continue;
                            }

                            // Split the line by '=' to separate the key and value.
                            string[] parts = line.Split('=');
                            if (parts.Length == 2)
                            {
                                string key = parts[0].Trim();
                                string value = parts[1].Trim();

                                // Update the corresponding control based on the key.
                                if (key == "IpAddress")
                                {
                                    ipAddMaskText.Text = value;
                                }
                                else if (key == "Port")
                                {
                                    portText.Text = value;
                                }
                                else if (key == "SaveFilePath")
                                {
                                    savePathText.Text = value;
                                }
                            }
                        }
                    }

                    // Update the UI status to indicate successful loading of the configuration.
                    TextBox_FontColorUpdater.SetTextBox_FontColor(savePathText);
                }
                catch (Exception ex)
                {
                    // Update the UI with an error message if reading fails.
                    StatusUpdater.ReceiveStatus("Error reading configuration: " + ex.Message, statusText);
                }
            }
            else
            {
                // Optionally update the status if the configuration file is not found.
                StatusUpdater.ReceiveStatus("Configuration file not found: " + iniFilePath, statusText);
            }
        }

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
        /*==========================================================================*
         *      Custom TextBox Properties
         *						@                                   				*
         *==========================================================================*/
        //private static void SetTextBox_FontColor(TextBox statusText)
        //{
        //    statusText.ForeColor = Color.Black;
        //}

    }
}
