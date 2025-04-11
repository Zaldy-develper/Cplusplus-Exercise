using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ipv4SocketCommunication
{
    class SocketHandler
    {
        // Fields
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
            IPAddress ipAddress = IPAddress.Parse(m_ipAdd);
            m_serverEndPoint = new IPEndPoint(ipAddress, m_port);

            // Initialize the list that will store connected client sockets.
            m_clientSocketList = new List<Socket>();

            // Set the static instance if needed
            Instance = this;
        }


        // Parameterless constructor
        public SocketHandler() : this(0, "") { }

        // Method to start the server
        public bool StartServer(CommonParams uiParams)
        {
            // Retrieve parameters from the object.
            CommonParams param = uiParams as CommonParams;
            if (param == null)
                return false;
            TextBox statusText = param.StatusText;
            TextBox savePathText = param.SavePathText;
            RichTextBox messageHistoryRichText = param.ChatLogRichText;
            TextBox serverText = param.ServerText;

            // If the server was previously stopped, restart it.
            if (!m_isServerRunning)
                m_isServerRunning = true;

            try
            {
                // Create a TCP socket
                m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the endpoint.
                m_serverSocket.Bind(m_serverEndPoint);
                StatusUpdater.TextBoxUpdater("Binding success", statusText);

                // Start listening with a backlog of 3.
                m_serverSocket.Listen(3);
                // Display the chat room name and listening status.
                StatusUpdater.TextBoxUpdater("Listening for connections", statusText);
            }
            catch (Exception ex)
            {
                StatusUpdater.TextBoxUpdater("ERROR: " + ex.Message, statusText);
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

                    // Update UI with client connection status.
                    StatusUpdater.TextBoxUpdater("StartServer@Client No." + m_clientNum + ": Client connected: " +
                        clientSocket.RemoteEndPoint.ToString(), statusText);

                    // Close the client connection if the maximum number of clients is exceeded.
                    if (m_clientNum > 3)
                    {
                        clientSocket.Close();
                        StatusUpdater.TextBoxUpdater("@Client No." + m_clientNum +
                            ": Client connection closed. Sorry, server is full.", statusText);
                        StatusUpdater.TextBoxUpdater("Maximum client limit reached. No longer accepting new connections.", statusText);
                    }
                    else
                    {
                        // Add the new client socket to the list.
                        m_clientSocketList.Add(clientSocket);

                        // Create a parameter object to pass data to the client thread.
                        ClientThreadParams clientParams = new ClientThreadParams(
                            clientSocket,
                            statusText,
                            savePathText,
                            m_clientNum,
                            messageHistoryRichText,
                            serverText);

                        // Optionally send a greeting to the client.
                        TestMessageSendToClient(clientParams);

                        // Spawn a new thread to handle the client's file transfer.
                        Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                        clientThread.Start(clientParams);

                        m_clientNum++;
                    }
                }
                catch (SocketException se)
                {
                    // If server is stopping, exit gracefully; otherwise, log error and continue.
                    if (!m_isServerRunning)
                        break;

                    StatusUpdater.TextBoxUpdater("ERROR: Failed to accept client connection: " + se.Message, statusText);
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
            ClientThreadParams clientParam = parameters as ClientThreadParams;
            if (clientParam == null)
                return;

            // Implement the client handling code here.

            // Call the file reception function for this client.
            bool success = ReceiveFile(clientParam);

            // Update status based on the result.
            string socketDataMessage = string.Format("Client No.{0} with Socket:[{1}]: ", clientParam.ClientNumber, clientParam.ClientSocket.Handle.ToInt32());
            if (success)
            {
                StatusUpdater.TextBoxUpdater(socketDataMessage + "No more reason to stay. Bye!", clientParam.StatusText);
            }
            else
            {
                StatusUpdater.TextBoxUpdater(socketDataMessage + "Yup! Error receiving file.", clientParam.StatusText);
            }

            // Clean up the client socket after processing.
            clientParam.ClientSocket.Close();

            // Remove the client socket from the list if it exists.
            if (m_clientSocketList.Contains(clientParam.ClientSocket))
            {
                m_clientSocketList.Remove(clientParam.ClientSocket);
            }

            // Decrement the client count.
            m_clientNum--;
        }
        public bool TestMessageSendToClient(ClientThreadParams clientParameter )
        {
            ClientThreadParams param = clientParameter as ClientThreadParams;
            if (param == null)
                return false;
            // Prepare the greeting message.
            string greeting = "Hello from server." + Environment.NewLine;
            try
            {
                // Convert the greeting to a byte array.
                byte[] data = Encoding.ASCII.GetBytes(greeting);

                // Send the data through the socket.
                // The Send method will throw a SocketException on error.
                int bytesSent = param.ClientSocket.Send(data);

                // Update status and success messages.
                m_statusMessage = "Greeting sent: " + greeting + " (to socket: " + param.GetSocketHandle() +
                                    ")";
                StatusUpdater.TextBoxUpdater(m_statusMessage, param.StatusText);
                return true;
            }
            catch (SocketException ex)
            {
                // Handle the error by updating the error and status messages.
                // Todo : Add logging or other error handling as needed.
                m_statusMessage = "ERROR: Failed to send greeting message: " + ex.Message;
                StatusUpdater.TextBoxUpdater(m_statusMessage, param.StatusText);
                return false;
            }
        }
        //public bool ReceiveFile(Socket socket, TextBox savePathText, TextBox statusText, 
        //                        RichTextBox messageHistoryRichText, int clientNumber)
        public bool ReceiveFile(ClientThreadParams clientParam)
        {
            // Retrieve parameters from the object.
            TextBox savePathText = clientParam.SavePathText;
            TextBox statusText = clientParam.StatusText;
            RichTextBox messageHistoryRichText = clientParam.ChatLogRichText;
            Socket socket = clientParam.ClientSocket;
            int clientNumber = clientParam.ClientNumber;
            int socketHandle = clientParam.GetSocketHandle();


            bool startToReceiveFlag = false;
            const int BUFFER_SIZE = 1024;
            byte[] buffer = new byte[BUFFER_SIZE];
            bool success = true;
            string statusMessage = string.Empty;

            // Set the timeout in microseconds (60 seconds = 60 * 1,000,000 µs).
            int pollTimeout = 60 * 1000000;



            while (true)
            {
                bool readyToRead = false;
                try
                {
                    // Poll for data; this may throw if the socket is closed.
                    readyToRead = socket.Poll(pollTimeout, SelectMode.SelectRead);
                }
                catch (SocketException ex)
                {
                    // If the socket was closed intentionally as part of StopServer,
                    // then m_isServerRunning should be false.
                    if (!m_isServerRunning)
                    {
                        // Optionally, log that the socket polling was interrupted
                        StatusUpdater.TextBoxUpdater("Server is stopping. Exiting receive loop.", statusText);
                        break;
                    }
                    else
                    {
                        // For any other socket exception, rethrow or handle as needed.
                        StatusUpdater.TextBoxUpdater("Suspend. Socket Error: " + ex.Message, statusText);
                        break;
                    }
                }

                // Use Poll to wait for data on the socket.
                // Poll returns true if there is data to read OR the connection has been closed.
                if (!readyToRead)
                {
                    // Timeout reached without any data.
                    if (startToReceiveFlag)
                    {
                        StatusUpdater.TextBoxUpdater("Client No." + clientNumber + " with Socket:" + socketHandle + " |Receiving no more data. 60s idle timeout reached. Ending receive.", statusText);
                    }
                    else
                    {
                        StatusUpdater.TextBoxUpdater("Client No." + clientNumber + " with Socket:" + socketHandle + " |Suspend. No transmission received after waiting 60s.", statusText);
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
                        StatusUpdater.TextBoxUpdater("Suspend. Socket Error: " + ex.Message, statusText);
                        break;
                    }
                    else if (startToReceiveFlag)
                    {
                        StatusUpdater.TextBoxUpdater("File receive has been stopped because the server is not running anymore.", statusText);
                        break;
                    }
                }

                if (bytesRead > 0)
                {
                    // Write the received data into the file.
                    if (!FileWriter.WriteBufferToFile(savePathText, buffer, bytesRead))
                    {
                        StatusUpdater.TextBoxUpdater("Failed to write message to file.", statusText);
                    }

                    // Convert the received bytes into a string.
                    // Here we add the client socket info (using RemoteEndPoint)
                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // Check if the received data (ignoring whitespace) is empty.
                    if (string.IsNullOrWhiteSpace(receivedData))
                    {
                        StatusUpdater.TextBoxUpdater("Received data is empty.", statusText);
                    }
                    else
                    {
                        // Construct the complete data string with client socket info.
                        string dataStr = string.Format("Client Socket: ({0})|[{1}]: {2}",
                                                       socket.RemoteEndPoint, socketHandle,
                                                       receivedData);
                        // Append the received text to the RichTextBox in a thread-safe way.
                        MessageLogger.MessageHistoryLog(messageHistoryRichText, dataStr);
                    }

                    if (!startToReceiveFlag)
                        startToReceiveFlag = true;

                    // Update the UI with the number of bytes read.
                    string bytesCount = string.Format("{0} bytes", bytesRead);
                    StatusUpdater.TextBoxUpdater("Received data. Size: " + bytesCount, statusText);
                }
                else if (bytesRead == 0)
                {
                    // The connection was closed gracefully.
                    if (startToReceiveFlag)
                    {
                        StatusUpdater.TextBoxUpdater("Suspend. Receiving of File Completed", statusText);
                        break;
                    }
                    else
                    {
                        StatusUpdater.TextBoxUpdater("Suspend. No Transmission received.", statusText);
                        break;
                    }
                }
                else
                {
                    // This branch is unlikely to be reached in C# because errors cause exceptions.
                    StatusUpdater.TextBoxUpdater("Suspend. Error encountered. Logic check needed.", statusText);
                    break;
                }
            }

            // Final UI update after the loop completes.
            statusMessage = string.Format("Client No.{1}|[{0}]: It has been suspended.", socketHandle, clientNumber);
            StatusUpdater.TextBoxUpdater(statusMessage, statusText);

            return success;
        }
        //public bool MessageSendToClient(Socket socket, TextBox serverText, RichTextBox messageHistoryRichText, TextBox statusText)
        public bool MessageSendToClient(ClientThreadParams clientServerParam)
        {
            // Retrieve parameters from the object.
            Socket socket = clientServerParam.ClientSocket;
            TextBox serverText = clientServerParam.ServerText;
            RichTextBox messageHistoryRichText = clientServerParam.ChatLogRichText;
            TextBox statusText = clientServerParam.StatusText;
            int socketHandle = clientServerParam.GetSocketHandle();

            // Append a newline to the message.
            DefaultItems.InitializeMessageToClient(serverText);
            string messageToSend = DefaultItems.MessageToSend;
            try
            {
                // Convert the message to a byte array.
                byte[] data = Encoding.ASCII.GetBytes(messageToSend);
                // Send the message through the socket.
                int sendResult = socket.Send(data);
                if (sendResult <= 0)
                {
                    StatusUpdater.TextBoxUpdater("Error sending message to client.", statusText);
                    return false;
                }
                else
                {
                    // Remove any trailing whitespace/newline characters.
                    string trimmedMessage = messageToSend.TrimEnd();
                    // Log the sent message to the chat log.
                    MessageLogger.MessageHistoryLog(messageHistoryRichText,
                        "    " + trimmedMessage +
                        "\n    (sent to socket:" + "[" + socketHandle + "]|" + socket.RemoteEndPoint.ToString() + ")" + Environment.NewLine);
                }
                return true;
            }
            catch (SocketException ex)
            {
                StatusUpdater.TextBoxUpdater("Error sending message to client: " + ex.Message, statusText);
                return false;
            }
        }

        // Getter
        public ReadOnlyCollection<Socket> GetClientSockets
        {
            get { return m_clientSocketList.AsReadOnly(); }
        }
    }

    // Class to pass parameters to the client handling thread.
    public class ClientThreadParams
    {
        // Fields
        private Socket m_clientSocket;
        // Auto-implemented properties for the parameters.
        public Socket ClientSocket { get; set; }
        public TextBox StatusText { get; private set; }
        public TextBox SavePathText { get; private set; }
        public TextBox ServerText { get; private set; }
        public int ClientNumber { get; set; }
        public RichTextBox ChatLogRichText { get; private set; }

        // Constructor that initializes the properties directly.
        public ClientThreadParams(Socket clientSocket, TextBox statusText, TextBox savePathText,
                                  int clientNumber, RichTextBox chatLogRichText, TextBox serverText)
        {
            ClientSocket = clientSocket;
            StatusText = statusText;
            SavePathText = savePathText;
            ClientNumber = clientNumber;
            ChatLogRichText = chatLogRichText;
            ServerText = serverText;
        }
        // Method to retrieve the socket handle.  
        public int GetSocketHandle()
        {
            return ClientSocket.Handle.ToInt32();
        }

    }
    public class CommonParams
    {
        public TextBox StatusText { get; private set; }
        public TextBox SavePathText { get; private set; }
        public RichTextBox ChatLogRichText { get; private set; }
        public TextBox ServerText { get; private set; }

        // Constructor that initializes the properties directly.
        public CommonParams(TextBox statusText, TextBox savePathText,
                            RichTextBox chatLogRichText, TextBox serverText)
        {
            StatusText = statusText;
            SavePathText = savePathText;
            ChatLogRichText = chatLogRichText;
            ServerText = serverText;
        }

    }
    public static class AppTimeOpen
    {
        // Static member to hold the timestamped save path.
        private static string timestampedSavePath = null;

        // Returns a file path with a timestamp appended to the file name.
        // The computation is done only once per run.
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


}



