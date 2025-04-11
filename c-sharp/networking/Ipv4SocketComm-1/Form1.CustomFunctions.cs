using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Ipv4SocketCommunication.ValidationFlow;

namespace Ipv4SocketCommunication
{
    partial class Form1
    {
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
        /*==========================================================================*
         *      Custom Functions
         *						@                                   				*
         *==========================================================================*/
        // Start the server thread.
        private void StartServerThread()
        {
            // Assuming SocketHandler.Instance was correctly initialized earlier,
            // you call its StartServer method. Since StartServer blocks while waiting for connections,
            // running it on a separate thread prevents the UI from freezing.
            CommonParams serverParam = new CommonParams(statusText, savePathText, messageHistoryRichText, serverText);
            SocketHandler.Instance.StartServer(serverParam);
        }

        //Validation Flow of the Input Fields 
        private bool isValidEntry()
        {
            bool success = false; // Initialize success to true
            // Setting the status text to indicate the start of the process.
            StatusUpdater.TextBoxUpdater("Performing Configuration Set-up." + Environment.NewLine, statusText);

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
                StatusUpdater.TextBoxUpdater("Administator checking only:" + "\nValidation Block[" + i + "] initialized. " +
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
                    StatusUpdater.TextBoxUpdater(line, statusText);
                }

                StatusUpdater.TextBoxUpdater("Input Validation succeeded.", statusText);
                success = true; // Set success to true
            }
            else
            {
                // If there is an error, set the focus based on the last field validated.
                ValidationFlow.FocusPosition cursorPosition = input.LastPosFocus;

                switch (cursorPosition)
                {
                    case ValidationFlow.FocusPosition.IP:
                        ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, btnSend, true);
                        ipAddMaskText.Focus();
                        break;
                    case ValidationFlow.FocusPosition.PORT:
                        ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, btnSend, true);
                        portText.Focus();
                        break;
                    case ValidationFlow.FocusPosition.PATH:
                        ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, btnSend, true);
                        savePathText.Focus();
                        break;
                    case ValidationFlow.FocusPosition.DEFAULT:
                        ButtonHandler.StartStopPathActive(btnStart, btnStop, btnSavePath, btnSend, true);
                        ipAddMaskText.Focus();
                        break;
                }

                // Report the error message.
                StatusUpdater.TextBoxUpdater(errorMessage, statusText);
                return success;
            }
            return success;
        }
        //Button Handler to enable or disable buttons based on the server state.
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
            public static void SendButtonActive(Button btnSend, bool isEnable)
            {
                btnSend.Enabled = isEnable;
            }
            public static void StartStopPathActive(Button btnStart, Button btnStop, Button btnSavePath, Button btnSend, bool isEnable)
            {
                if (isEnable)
                {
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    btnSavePath.Enabled = true;
                    btnSend.Enabled = false;
                }
                else
                {
                    btnStart.Enabled = false;
                    //btnStop.Enabled = true;
                    btnSavePath.Enabled = false;
                    //btnSend.Enabled = true;
                }
            }
        }
        //Save the configuration data to an INI file.
        public void SaveConfigData()
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
                StatusUpdater.TextBoxUpdater("Configuration saved to " + iniFilePath, statusText);
            }
            catch (Exception ex)
            {
                // Update the UI status in case of an error.
                StatusUpdater.TextBoxUpdater("Error saving configuration: " + ex.Message, statusText);
            }
        }
        //Load the configuration data from an INI file.
        public void LoadConfigData()
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
                    StatusUpdater.TextBoxUpdater("Error reading configuration: " + ex.Message, statusText);
                }
            }
            else
            {
                // Optionally update the status if the configuration file is not found.
                StatusUpdater.TextBoxUpdater("Configuration file not found: " + iniFilePath, statusText);
            }
        }
        //Set the default text for the TextBox when it is empty.
        private void serverText_Enter(object sender, EventArgs e)
        {
            // Check if the TextBox has the default placeholder text.
            if (serverText.Text == DefaultItems.StatusDefaultText)
            {
                // Clear the text and change the font color (typically to Black).
                serverText.Text = "";
                TextBox_FontColorUpdater.SetTextBox_FontColor(serverText);
                serverText.ForeColor = Color.Black;
            }
        }
        //Set the default text for the TextBox when it loses focus.
        private void serverText_Leave(object sender, EventArgs e)
        {
            // If no input is provided, restore the default placeholder text.
            if (string.IsNullOrWhiteSpace(serverText.Text))
            {
                serverText.Text = DefaultItems.StatusDefaultText;
                serverText.ForeColor = Color.Gray;
            }
        }
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
    /*==========================================================================*
     *      Custom TextBox Properties
     *						@                                   				*
     *==========================================================================*/
    // TextBox and RichTextBox update methods.
    public static class StatusUpdater
    {
        // Updates a TextBox in a thread-safe manner.
        public static void TextBoxUpdater(string message, TextBox statusText)
        {
            if (statusText.InvokeRequired)
            {
                // Invoke the method on the UI thread.
                statusText.Invoke(new Action<string, TextBox>(TextBoxUpdater), message, statusText);
            }
            else
            {
                TextBox_FontColorUpdater.SetTextBox_FontColor(statusText);
                statusText.AppendText(message + Environment.NewLine);
                // Set the caret position to the end of the text.
                statusText.SelectionStart = statusText.Text.Length;
                // Scroll the caret into view.
                statusText.ScrollToCaret();
                statusText.Refresh(); // Force the UI to update immediately.
            }
            Thread.Sleep(300);
        }

    }
    public static class MessageLogger
    {
        // Helper method that appends text to a RichTextBox.
        public static void AppendTextToRichTextBox(RichTextBox richTextBox, string text)
        {
            richTextBox.AppendText(text);

            // Set the caret position to the end of the text.
            richTextBox.SelectionStart = richTextBox.Text.Length;
            // Scroll the caret into view.
            richTextBox.ScrollToCaret();
        }

        // Thread-safe method for logging messages to the RichTextBox.
        public static void MessageHistoryLog(RichTextBox chatLogRichText, string dataStr, bool clearMessageLog = false)
        {
            if (chatLogRichText.InvokeRequired)
            {
                chatLogRichText.Invoke(new Action<RichTextBox, string>(AppendTextToRichTextBox),
                                       new object[] { chatLogRichText, dataStr });
            }
            else
            {
                if (clearMessageLog)
                {
                    chatLogRichText.Text = dataStr;
                }
                else
                    chatLogRichText.AppendText(dataStr);
            }
        }
    }
    public static class TextBox_FontColorUpdater
    {
        public static void SetTextBox_FontColor(TextBox statusText)
        {
            SetTextBox_FontColor(statusText, Color.Black);
        }
        public static void SetTextBox_FontColor(TextBox statusText, Color color)
        {
            //statusText.ForeColor = Color.Black;
            statusText.ForeColor = color;
        }
    }
    public static class RichTextBox_FontColorUpdater
    {
        public static void SetRichTextBox_FontColor(RichTextBox richText)
        {
            richText.ForeColor = Color.Black;
        }
    }
    public static class DefaultItems
    {
        private static string m_statusDefaultText = "Message...";
        private static string m_messageToSend;

        // Getter
        public static string StatusDefaultText
        {
            get { return m_statusDefaultText; }
            //set { m_defaultText = value; }
        }
        public static string MessageToSend
        {
            get { return m_messageToSend; }
        }
        // Added a method to handle the functionality that was incorrectly placed in the static constructor.
        public static void InitializeMessageToClient(TextBox textbox)
        {
            m_messageToSend = "Server: " + textbox.Text + Environment.NewLine;
        }
    }
}
