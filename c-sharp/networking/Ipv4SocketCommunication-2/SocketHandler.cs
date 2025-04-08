using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ipv4SocketCommunication
{
    class SocketHandler
    {
        private IPEndPoint m_serverEndPoint;
        private Socket m_serverSocket;
        private int m_port;
        private int m_clientNum;
        private List<Socket> m_clientSocketList;

        private string m_ipAdd;

        private bool m_isServerRunning;

        // Define maximum number of clients
        public const int MAX_CLIENTS = 3;

        // Status messages
        private string m_errorMessage;
        private string m_successMessage;
        private string m_statusMessage;

        // Static instance for global access (if required)
        public static SocketHandler Instance { get; private set; }
        // Constructor with parameters
        public SocketHandler(int port, string ipAdd)
        {
            this.m_port = port;
            this.m_ipAdd = ipAdd;
            this.m_clientNum = 1;
            this.m_isServerRunning = true;

            // Create the endpoint using the provided IP address and port
            IPAddress ipAddress = IPAddress.Parse(ipAdd);
            m_serverEndPoint = new IPEndPoint(ipAddress, port);

            // Initialize the list that will store connected client sockets.
            m_clientSocketList = new List<Socket>();

            // File Integrity Check (this example uses a simple string comparison).
            // In your C++ version, you call system_encode and compare with CHECKER.
            // Adjust the logic here as needed.
            //string integFilecheck = "zaldy's chatroom is now setting up.";
            //string checkerString = "EXPECTED_CHECKER_STRING"; // Replace with your expected string
            //isFileValid = (integFilecheck == checkerString);

            //// Set the chat room name (using the integrity string in this example).
            //m_chatroom_name = integFilecheck;

            // Set the static instance if needed
            Instance = this;
        }


        // Parameterless constructor
        public SocketHandler() : this(0, "") { }

        // Method to start the server
        public bool StartServer(TextBox statusText, TextBox filePathText, RichTextBox messageHistoryRichText)
        {
            // If the server was previously stopped, restart it.
            if (!m_isServerRunning)
                m_isServerRunning = true;

            try
            {
                // Create a TCP socket
                m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the endpoint.
                m_serverSocket.Bind(m_serverEndPoint);
                StatusUpdater.ReceiveStatus("Binding success", statusText);

                // Start listening with a backlog of 3.
                m_serverSocket.Listen(3);
                // Display the chat room name and listening status.
                //ReceiveStatus(m_chatroom_name, textboxStatus);
                StatusUpdater.ReceiveStatus("Listening for connections", statusText);
            }
            catch (Exception ex)
            {
                StatusUpdater.ReceiveStatus("ERROR: " + ex.Message, statusText);
                return false;
            }

            // Loop to accept incoming client connections.
            while (m_clientNum <= MAX_CLIENTS + 1)
            {
                try
                {
                    // Accept a client connection (this call blocks until a connection is available)
                    Socket clientSocket = m_serverSocket.Accept();

                    // If the server has been stopped, break out of the loop.
                    if (!m_isServerRunning)
                        break;

                    // Check the file validity.
                    //if (!isFileValid)
                    //{
                    //    ReceiveStatus("ERROR: File Invalid", textboxStatus);
                    //    StopServer();
                    //    break;
                    //}

                    // Update UI with client connection status.
                    StatusUpdater.ReceiveStatus("@Client No." + m_clientNum + ": Client connected: " +
                        clientSocket.RemoteEndPoint.ToString(), statusText);

                    // Optionally send a greeting to the client.
                    TestMessageSendToClient(clientSocket);

                    // Close the client connection if the maximum number of clients is exceeded.
                    if (m_clientNum > 3)
                    {
                        clientSocket.Close();
                        StatusUpdater.ReceiveStatus("@Client No." + m_clientNum +
                            ": Client connection closed. Sorry, server is full.", statusText);
                        StatusUpdater.ReceiveStatus("Maximum client limit reached. No longer accepting new connections.", statusText);
                    }
                    else
                    {
                        // Add the new client socket to the list.
                        m_clientSocketList.Add(clientSocket);

                        // Create a parameter object to pass data to the client thread.
                        ClientThreadParams parameters = new ClientThreadParams(clientSocket, statusText,
                            filePathText, m_clientNum, messageHistoryRichText);

                        // Spawn a new thread to handle the client's file transfer.
                        Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                        clientThread.Start(parameters);

                        m_clientNum++;
                    }
                }
                catch (SocketException se)
                {
                    // If server is stopping, exit gracefully; otherwise, log error and continue.
                    if (!m_isServerRunning)
                        break;

                    StatusUpdater.ReceiveStatus("ERROR: Failed to accept client connection: " + se.Message, statusText);
                    continue;
                }
            }

            return true;
        }

        // Method to stop the server by cleaning up sockets.
        public static void StopServer()
        {
            Instance?.Shutdown();
        }
        // Instance method to perform the actual shutdown.
        private void Shutdown()
        {
            m_isServerRunning = false;
            try
            {
                if (m_serverSocket != null)
                    m_serverSocket.Close();

                // Close all connected client sockets.
                foreach (var client in m_clientSocketList)
                {
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the error during shutdown as needed.
            }
        }

        // Method to handle communication with a client.
        private void HandleClient(object parameters)
        {
            ClientThreadParams param = parameters as ClientThreadParams;
            if (param == null)
                return;

            // Implement the client handling code here.

            // Call the file reception function for this client.
            bool success = ReceiveFile(param.ClientSocket, param.FilePathText, param.StatusText, param.ChatLogRichText);
            // Update status based on the result.
            if (success)
            {
                StatusUpdater.ReceiveStatus($"@Client No.{param.ClientNumber}: No more reason to stay.", param.StatusText);
            }
            else
            {
                StatusUpdater.ReceiveStatus($"@Client No.{param.ClientNumber}: Yup! Error receiving file.", param.StatusText);
            }

            // Clean up the client socket after processing.
            param.ClientSocket.Close();

            // Remove the client socket from the list if it exists.
            if (m_clientSocketList.Contains(param.ClientSocket))
            {
                m_clientSocketList.Remove(param.ClientSocket);
            }

            // Decrement the client count.
            m_clientNum--;
        }
        public bool TestMessageSendToClient(Socket socket)
        {
            // Prepare the greeting message.
            string greeting = "Hello from server." + Environment.NewLine;
            try
            {
                // Convert the greeting to a byte array.
                byte[] data = Encoding.ASCII.GetBytes(greeting);

                // Send the data through the socket.
                // The Send method will throw a SocketException on error.
                int bytesSent = socket.Send(data);

                // Update status and success messages.
                m_successMessage = "Greeting sent: " + greeting;
                m_statusMessage = "Greeting sent: " + greeting;
                return true;
            }
            catch (SocketException ex)
            {
                // Handle the error by updating the error and status messages.
                m_errorMessage = "ERROR: Failed to send greeting message: " + ex.Message;
                m_statusMessage = "A connection from a client has been accepted. Used socket: ";
                return false;
            }
        }
        public bool ReceiveFile(Socket socket, TextBox filePathText, TextBox statusText, RichTextBox messageHistoryRichText)
        {
            bool startToReceiveFlag = false;
            const int BUFFER_SIZE = 1024;
            byte[] buffer = new byte[BUFFER_SIZE];
            bool success = true;

            // Set the timeout in microseconds (60 seconds = 60 * 1,000,000 µs).
            int pollTimeout = 60 * 1000000;

            while (true)
            {
                // Use Poll to wait for data on the socket.
                // Poll returns true if there is data to read OR the connection has been closed.
                bool readyToRead = socket.Poll(pollTimeout, SelectMode.SelectRead);
                if (!readyToRead)
                {
                    // Timeout reached without any data.
                    if (startToReceiveFlag)
                    {
                        StatusUpdater.ReceiveStatus("Receiving no more data. 60s idle timeout reached. Ending receive.", statusText);
                    }
                    else
                    {
                        StatusUpdater.ReceiveStatus("Suspend. No transmission received after waiting 60s.", statusText);
                    }
                    break;
                }

                int bytesRead = 0;
                try
                {
                    // Attempt to receive data from the socket.
                    bytesRead = socket.Receive(buffer, 0, BUFFER_SIZE, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    // An exception indicates a socket error.
                    if (m_isServerRunning)
                    {
                        success = false;
                        StatusUpdater.ReceiveStatus("Suspend. Socket Error: " + ex.Message, statusText);
                        break;
                    }
                    else if (startToReceiveFlag)
                    {
                        StatusUpdater.ReceiveStatus("File receive has been stopped because the server is not running anymore.", statusText);
                        break;
                    }
                }

                if (bytesRead > 0)
                {
                    // Write the received data into the file.
                    // WriteBufferToFile should be implemented to write the first 'bytesRead' bytes from buffer to the target file.
                    if (!FileWriter.WriteBufferToFile(filePathText, buffer, bytesRead))
                    {
                        StatusUpdater.ReceiveStatus("Failed to write message to file.", statusText);
                    }

                    // Convert the received bytes into a string.
                    // Here we add the client socket info (using RemoteEndPoint) similar to your C++ code.
                    string dataStr = string.Format("Client Socket: ({0}): {1}", socket.RemoteEndPoint, Encoding.ASCII.GetString(buffer, 0, bytesRead));

                    // Append the received text to the RichTextBox (ensure this update is done in a thread-safe manner).
                    MessageLogger.MessageHistoryLog(messageHistoryRichText, dataStr);

                    if (!startToReceiveFlag)
                        startToReceiveFlag = true;

                    // Update the UI with the number of bytes read.
                    string bytesCount = string.Format("{0} bytes", bytesRead);
                    StatusUpdater.ReceiveStatus("Received data. Size: " + bytesCount, statusText);
                }
                else if (bytesRead == 0)
                {
                    // The connection was closed gracefully.
                    if (startToReceiveFlag)
                    {
                        StatusUpdater.ReceiveStatus("Suspend. Receiving of File Completed", statusText);
                        break;
                    }
                    else
                    {
                        StatusUpdater.ReceiveStatus("Suspend. No Transmission received.", statusText);
                        break;
                    }
                }
                else
                {
                    // This branch is unlikely to be reached in C# because errors cause exceptions.
                    StatusUpdater.ReceiveStatus("Suspend. Error encountered. Logic check needed.", statusText);
                    break;
                }
            }

            // Final UI update after the loop completes.
            StatusUpdater.ReceiveStatus("Suspend. but which socket am I?", statusText);

            return success;
        }
        public bool MessageSendToClient(Socket socket, TextBox serverText, RichTextBox messageHistoryRichText, TextBox statusText)
        {
            // Append a newline to the message.
            string messageToSend = serverText.Text + Environment.NewLine;
            try
            {
                // Convert the message to a byte array.
                byte[] data = Encoding.ASCII.GetBytes(messageToSend);
                // Send the message through the socket.
                int sendResult = socket.Send(data);
                if (sendResult <= 0)
                {
                    StatusUpdater.ReceiveStatus("Error sending message to client.", statusText);
                    return false;
                }
                else
                {
                    // Remove any trailing whitespace/newline characters.
                    string trimmedMessage = messageToSend.TrimEnd();
                    // Log the sent message to the chat log.
                    MessageLogger.MessageHistoryLog(messageHistoryRichText,
                        "    Server: " + trimmedMessage +
                        " (sent to socket: " + socket.RemoteEndPoint.ToString() + ")" + Environment.NewLine);
                }
                return true;
            }
            catch (SocketException ex)
            {
                StatusUpdater.ReceiveStatus("Error sending message to client: " + ex.Message, statusText);
                return false;
            }
        }


    }

    public static class StatusUpdater
    {
        /// <summary>
        /// Updates a TextBox in a thread-safe manner.
        /// </summary>
        /// <param name="message">The message to append.</param>
        /// <param name="textboxStatus">The TextBox control to update.</param>
        public static void ReceiveStatus(string message, TextBox statusText)
        {
            if (statusText.InvokeRequired)
            {
                // Invoke the method on the UI thread.
                statusText.Invoke(new Action<string, TextBox>(ReceiveStatus), message, statusText);
            }
            else
            {
                TextBox_FontColorUpdater.SetTextBox_FontColor(statusText);
                statusText.AppendText(message + Environment.NewLine);
                statusText.Refresh(); // Force the UI to update immediately.
            }
            Thread.Sleep(300);
        }

    }

    public static class TextBox_FontColorUpdater
    {
        public static void SetTextBox_FontColor(TextBox statusText)
        {
            statusText.ForeColor = Color.Black;
        }
    }
    public static class RichTextBox_FontColorUpdater
    {
        public static void SetRichTextBox_FontColor(RichTextBox richText)
        {
            richText.ForeColor = Color.Black;
        }
    }

    // Class to pass parameters to the client handling thread.
    public class ClientThreadParams
    {
        public Socket ClientSocket { get; private set; }
        public TextBox StatusText { get; private set; }
        public TextBox FilePathText { get; private set; }
        public int ClientNumber { get; private set; }
        public RichTextBox ChatLogRichText { get; private set; }

        public ClientThreadParams(Socket clientSocket, TextBox statusText, TextBox filePathText, int clientNumber, RichTextBox chatLogRichText)
        {
            ClientSocket = clientSocket;
            StatusText = statusText;
            FilePathText = filePathText;
            ClientNumber = clientNumber;
            ChatLogRichText = chatLogRichText;
        }
    }
    public static class AppTimeOpen
    {
        // Static member to hold the timestamped save path.
        private static string timestampedSavePath = null;

        /// Returns a file path with a timestamp appended to the file name.
        /// The computation is done only once per run.
        public static string GetTimestampedSavePath(string savePath)
        {
            if (timestampedSavePath == null)
            {
                string directory = Path.GetDirectoryName(savePath);
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(savePath);
                string extension = Path.GetExtension(savePath);
                string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string newFilename = string.Format("{0}_{1}{2}", filenameWithoutExtension, timeStamp, extension);
                timestampedSavePath = Path.Combine(directory, newFilename);
            }
            return timestampedSavePath;
        }
    }
    public static class FileMutexWrapper
    {
        // A static Mutex instance for synchronizing file writes.
        public static readonly Mutex fileMutex = new Mutex();
    }
    public static class TextBoxHelper
    {
        // Safely retrieves the text from a TextBox control.
        public static string GetTextThreadSafe(TextBox textBox)
        {
            if (textBox.InvokeRequired)
            {
                return (string)textBox.Invoke(new Func<string>(() => textBox.Text));
            }
            return textBox.Text;
        }
    }
    public static class FileWriter
    {
        // Writes the specified number of bytes from the buffer into a file.
        // The file name is generated in a timestamped manner using the text from a TextBox.
        // A mutex ensures that only one thread writes to the file at a time.
        public static bool WriteBufferToFile(TextBox textboxFilePath, byte[] buffer, int bytesRead)
        {
            bool success = true;

            // Wait until we can enter the critical section.
            FileMutexWrapper.fileMutex.WaitOne();
            try
            {
                // Get the file path from the TextBox in a thread-safe manner.
                string originalFilePath = TextBoxHelper.GetTextThreadSafe(textboxFilePath);
                // Generate a timestamped file path.
                string newSavePath = AppTimeOpen.GetTimestampedSavePath(originalFilePath);

                // Open the file in append mode (binary).
                using (FileStream fs = new FileStream(newSavePath, FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    // Write the first 'bytesRead' bytes from the buffer.
                    fs.Write(buffer, 0, bytesRead);
                }
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                // Always release the mutex.
                FileMutexWrapper.fileMutex.ReleaseMutex();
            }

            // If the file operation was unsuccessful, delete the file.
            if (!success)
            {
                try
                {
                    string originalFilePath = TextBoxHelper.GetTextThreadSafe(textboxFilePath);
                    string newSavePath = AppTimeOpen.GetTimestampedSavePath(originalFilePath);
                    if (File.Exists(newSavePath))
                    {
                        File.Delete(newSavePath);
                    }
                }
                catch { /* Optionally log deletion failure. */ }
            }

            return success;
        }
    }
    public static class MessageLogger
    {
        // Helper method that appends text to a RichTextBox.
        public static void AppendTextToRichTextBox(RichTextBox richTextBox, string text)
        {
            richTextBox.AppendText(text);
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

}



