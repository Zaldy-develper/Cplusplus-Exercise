using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ipv4SocketCommunication.Interfaces;

namespace Ipv4SocketCommunication
{
    // Implements the ISocketHandler interface.
    // Abstract dependencies (IUpdateNotifier and IClientParameters)

    class SocketHandler : ISocketHandler
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

        // Static instance for global access (if required)
        public static SocketHandler Instance { get; private set; }
        // Constructor with parameters
        public SocketHandler(int port, string ipAdd)
        {
            m_port = port;
            m_ipAdd = ipAdd;
            m_clientNum = 1;
            m_isServerRunning = true;

            // Create the endpoint using the provided IP address and port
            IPAddress ipAddress = IPAddress.Parse(ipAdd);
            m_serverEndPoint = new IPEndPoint(ipAddress, port);

            // Initialize the list that will store connected client sockets.
            m_clientSocketList = new List<Socket>();

            // Set the static instance if needed
            Instance = this;
        }

        #region ISocketHandler Implementation

        // Parameterless constructor
        //public SocketHandler() : this(0, "") { }

        // Starts the server and begins listening for connections.
        // Uses IUpdateNotifier to report status without directly referencing UI controls.
        public bool StartServer(IUpdateNotifier notifier)
        {
            // Ensure the notifier is not null.
            if (notifier == null)
                throw new ArgumentNullException(nameof(notifier));

            // If the server was previously stopped, restart it.
            if (!m_isServerRunning)
                m_isServerRunning = true;

            try
            {
                // Create a TCP socket
                m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the endpoint.
                m_serverSocket.Bind(m_serverEndPoint);
                notifier.UpdateStatus("Binding success.");

                // Start listening with a backlog of 3.
                m_serverSocket.Listen(MAX_CLIENTS);

                // Llistening status.
                notifier.UpdateStatus("Listening for connections.");
            }
            catch (Exception ex)
            {
                notifier.UpdateStatus("ERROR: " + ex.Message);
                return false;
            }

            // Accept clients in a loop.
            // Run in background thread to avoid blocking the UI.
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
                    notifier.UpdateStatus($"Client No.{m_clientNum}: Connected from {clientSocket.RemoteEndPoint}.");

                    // Send a greeting to the client.
                    TestMessageSendToClient(clientSocket, notifier);

                    // Close the client connection if the maximum number of clients is exceeded.
                    if (m_clientNum > MAX_CLIENTS)
                    {
                        clientSocket.Close();
                        notifier.UpdateStatus($"Client No.{m_clientNum}: Connection closed (server full).");
                        notifier.UpdateStatus("Maximum client limit reached. No longer accepting new connections.");
                    }
                    else
                    {
                        // Add the new client socket to the list.
                        m_clientSocketList.Add(clientSocket);

                        // Create an instance of IClientParameters
                        ClientParameters clientParams = new ClientParameters(clientSocket, notifier, m_clientNum);

                        // Spawn a new thread to handle the client's file transfer.
                        Thread clientThread = new Thread(() => HandleClient(clientParams));
                        clientThread.IsBackground = true;
                        clientThread.Start();

                        m_clientNum++;
                    }
                }
                catch (SocketException se)
                {
                    // If server is stopping, exit gracefully; otherwise, log error and continue.
                    if (!m_isServerRunning)
                        break;

                    notifier.UpdateStatus("ERROR: " + se.Message);
                    continue;
                }
            }

            return true;
        }

        // Stops the server and closes all sockets.
        public void StopServer()
        {
            Shutdown();
        }

        // Method to handle communication with a client. 
        public void HandleClient(IClientParameters clientParams)
        {
            if (clientParams == null)
                return;

            // Receive file/data from the client.
            bool success = ReceiveFile(clientParams);

            // Update status based on the result.
            string statusBase = $"Client No.{clientParams.ClientNumber} | Socket:[{clientParams.ClientSocket.Handle.ToInt32()}]: ";
            if (success)
                if (success)
            {
                clientParams.UpdateStatus(statusBase + "Finished receiving file.");
            }
            else
            {
                clientParams.UpdateStatus(statusBase + "Error receiving file.");
            }

            // Clean up the client socket after processing.
            clientParams.ClientSocket.Close();

            // Remove the client socket from the list if it exists.
            if (m_clientSocketList.Contains(clientParams.ClientSocket))
            {
                m_clientSocketList.Remove(clientParams.ClientSocket);
            }

            // Decrement the client count.
            m_clientNum--;
        }

        // Test sending a message to the client.
        public bool TestMessageSendToClient(Socket socket, IUpdateNotifier notifier)
        {
            // Prepare the greeting message.
            string greeting = "Hello from server." + Environment.NewLine;
            try
            {
                // Convert the greeting to a byte array.
                byte[] data = Encoding.ASCII.GetBytes(greeting);

                // Send the data through the socket.
                // The Send method will throw a SocketException on error.
                socket.Send(data);

                // Update status.
                notifier.UpdateStatus("Greeting sent: " + greeting);
                return true;
            }
            catch (SocketException ex)
            {
                // Handle the error by updating status messages.
                notifier.UpdateStatus("ERROR: Failed to send greeting: " + ex.Message);
                return false;
            }
        }
        // Receives data from a client and writes it to a file.
        public bool ReceiveFile(IClientParameters clientParams)
        {
            bool startToReceiveFlag = false;

            const int BUFFER_SIZE = 1024;
            byte[] buffer = new byte[BUFFER_SIZE];
            bool success = true;
            string statusMessage = string.Empty;

            // Set the timeout in microseconds (60 seconds = 60 * 1,000,000 µs).
            int pollTimeout = 60 * 1000000;

            Socket clientSocket = clientParams.ClientSocket;

            while (true)
            {
                bool readyToRead = false;
                try
                {
                    // Poll for data; this may throw if the socket is closed.
                    readyToRead = clientSocket.Poll(pollTimeout, SelectMode.SelectRead);
                }
                catch (SocketException ex)
                {
                    // If the socket was closed intentionally as part of StopServer,
                    // then m_isServerRunning should be false.
                    if (!m_isServerRunning)
                    {
                        // Optionally, log that the socket polling was interrupted
                        clientParams.UpdateStatus("Server is stopping. Exiting receive loop.");
                        break;
                    }
                    else
                    {
                        // For any other socket exception, rethrow or handle as needed.
                        clientParams.UpdateStatus("Suspend. Socket Error: " + ex.Message);
                        break;
                    }
                }

                // Use Poll to wait for data on the socket.
                // Poll returns true if there is data to read OR the connection has been closed.
                //bool readyToRead = socket.Poll(pollTimeout, SelectMode.SelectRead);
                if (!readyToRead)
                {
                    // Timeout reached without any data.
                    if (startToReceiveFlag)
                    {
                        clientParams.UpdateStatus($"Client No.{clientParams.ClientNumber} | Idle timeout reached. Ending receive.");
                    }
                    else
                    {
                        clientParams.UpdateStatus($"Client No.{clientParams.ClientNumber} | No data received after waiting.");
                    }
                    break;
                }

                int bytesRead = 0;
                try
                {
                    // Attempt to receive data from the socket.
                    bytesRead = clientSocket.Receive(buffer, 0, BUFFER_SIZE, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    // An exception indicates a socket error.
                    if (m_isServerRunning)
                    {
                        success = false;
                        clientParams.UpdateStatus("Suspend. Socket Error: " + ex.Message);
                        break;
                    }
                    else if (startToReceiveFlag)
                    {
                        clientParams.UpdateStatus("Receive stopped because the server is not running.");
                        break;
                    }
                }

                if (bytesRead > 0)
                {
                    // Write the received data into the file.
                    // WriteBufferToFile should be implemented to write the first 'bytesRead' bytes from buffer to the target file.
                    if (!FileWriter.WriteBufferToFile(clientParams, buffer, bytesRead))
                    {
                        clientParams.UpdateStatus("Failed to write data to file.");
                    }

                    // Convert the received bytes into a string.
                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // Check if the received data (ignoring whitespace) is empty.
                    if (string.IsNullOrWhiteSpace(receivedData))
                    {
                        clientParams.UpdateStatus("Received data is empty.");
                    }
                    else
                    {
                        // Construct the complete data string with client socket info.
                        clientParams.LogMessage($"Client Socket No.{clientParams.ClientNumber} :[{clientSocket.Handle.ToInt32()}] {receivedData}");
                    }

                    if (!startToReceiveFlag)
                        startToReceiveFlag = true;

                    // Number of bytes read.
                    clientParams.UpdateStatus($"Received data. Size: {bytesRead} bytes");
                    
                }
                else if (bytesRead == 0)
                {
                    // The connection was closed gracefully.
                    if (startToReceiveFlag)
                        clientParams.UpdateStatus("File reception completed.");
                    else
                        clientParams.UpdateStatus("No transmission received.");
                    break;
                    
                }
                else
                {
                    // This branch is unlikely to be reached in C# because errors cause exceptions.
                    clientParams.UpdateStatus("Suspend. Unexpected error encountered.");
                    break;
                }
            }

            // Final update after the loop completes.
            clientParams.UpdateStatus($"Client No.{clientParams.ClientNumber} | Socket:[{clientSocket.Handle.ToInt32()}] receive loop ended.");

            return success;
        }
        // Sends a message to the client.
        public bool MessageSendToClient(IClientParameters clientParams)
        {
            // Message to send.
            string messageToSend = clientParams.ServerMessage;
            try
            {
                // Convert the message to a byte array.
                byte[] data = Encoding.ASCII.GetBytes(messageToSend);
                // Send the message through the socket.
                int bytesSent = clientParams.ClientSocket.Send(data);
                if (bytesSent <= 0)
                {
                    clientParams.UpdateStatus("Error sending message to client.");
                    return false;
                }
                else
                {
                    // Remove any trailing whitespace/newline characters.
                    clientParams.LogMessage($"   Server: {messageToSend.TrimEnd()}\n   (sent to [{clientParams.ClientSocket.Handle.ToInt32()}])");
                }
                return true;
            }
            catch (SocketException ex)
            {
                clientParams.UpdateStatus("Error sending message: " + ex.Message);
                return false;
            }
        }
        public ReadOnlyCollection<Socket> GetClientSockets
        {
            get { return m_clientSocketList.AsReadOnly(); }
        }

        #endregion

        #region Helper Methods

        // Shuts down the server and cleans up sockets.
        private void Shutdown()
        {
            m_isServerRunning = false;
            try
            {
                m_serverSocket?.Close();
                foreach (var client in m_clientSocketList)
                    client.Close();
            }
            catch (Exception ex)
            {
                // Optionally log the exception.
            }
        }
        #endregion
        // Getter
        //public ReadOnlyCollection<Socket> GetClientSockets
        //{
        //    get { return m_clientSocketList.AsReadOnly(); }
        //}
    }

    // Implementation of IClientParameters for use in the SocketHandler.
    // This class wraps the client Socket along with the UI notifier and a client identifier.
    public class ClientParameters : IClientParameters
    {
        public Socket ClientSocket { get; private set; }
        public int ClientNumber { get; private set; }
        private readonly IUpdateNotifier _notifier;

        public ClientParameters(Socket socket, IUpdateNotifier notifier, int clientNumber)
        {
            ClientSocket = socket;
            _notifier = notifier;
            ClientNumber = clientNumber;
        }

        // Pass through the notifier interface.
        public void UpdateStatus(string message) => _notifier.UpdateStatus(message);
        public void LogMessage(string message) => _notifier.LogMessage(message);
        public string SaveFilePath => _notifier.SaveFilePath;
        public string ServerMessage => _notifier.ServerMessage;
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
        //// Get socket information for logging.
        //// Retrieve the underlying socket handle.
        //IntPtr socketHandle = ClientSocket.Handle;

        //// Convert to integer values based on platform needs.
        //int intSocketHandle = socketHandle.ToInt32();  // For 32-bit representation.
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
        // New overload accepting IUpdateNotifier (or any type with SaveFilePath)
        public static bool WriteBufferToFile(IUpdateNotifier notifier, byte[] buffer, int bytesRead)
        {
            bool success = true;
            FileMutexWrapper.fileMutex.WaitOne();
            try
            {
                string originalFilePath = notifier.SaveFilePath;
                string newSavePath = AppTimeOpen.GetTimestampedSavePath(originalFilePath);
                using (System.IO.FileStream fs = new System.IO.FileStream(newSavePath, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.None))
                {
                    fs.Write(buffer, 0, bytesRead);
                }
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                FileMutexWrapper.fileMutex.ReleaseMutex();
            }
            if (!success)
            {
                try
                {
                    string originalFilePath = notifier.SaveFilePath;
                    string newSavePath = AppTimeOpen.GetTimestampedSavePath(originalFilePath);
                    if (System.IO.File.Exists(newSavePath))
                    {
                        System.IO.File.Delete(newSavePath);
                    }
                }
                catch { }
            }
            return success;
        }
    }


}



