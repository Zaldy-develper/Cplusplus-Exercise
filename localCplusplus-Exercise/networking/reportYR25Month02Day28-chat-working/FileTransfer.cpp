#include "FileTransfer.h"
#include <ws2tcpip.h>
#include <msclr/marshal_cppstd.h>
#include <windows.h>
#include <tchar.h>
#include <strsafe.h>
#include <vcclr.h>  // Required for gcroot
#include <winsock2.h>

using namespace System;
using namespace System::Text;
using namespace System::IO;
using namespace System::Windows::Forms;
using namespace msclr::interop;
using namespace System::Threading;

#define BUFFER_SIZE 1024
#define MAX_CLIENTS 3

std::string system_encode(const std::string& input);

// Global mutex for file writing.
ref class FileMutexWrapper {
public:
	static initonly Mutex^ fileMutex = gcnew Mutex(); // So that it will be available to all function
};

//===============================
// Helper class for timestamped file naming
//===============================
public ref class AppTimeOpen {
private:
	// Static member to hold the timestamped save path. It is initialized to nullptr.
	static String^ timestampedSavePath = nullptr;

public:
	static String^ GetTimestampedSavePath(String^ savePath) {
		// Compute the timestamped path only once.
		if (timestampedSavePath == nullptr) {
			String^ directory = Path::GetDirectoryName(savePath);
			String^ filenameWithoutExtension = Path::GetFileNameWithoutExtension(savePath);
			String^ extension = Path::GetExtension(savePath);
			String^ timeStamp = DateTime::Now.ToString("yyyyMMdd_HHmmss");
			String^ newFilename = String::Format("{0}_{1}{2}", filenameWithoutExtension, timeStamp, extension);
			timestampedSavePath = System::IO::Path::Combine(directory, newFilename);
		}
		
		return timestampedSavePath;
	}
};

//====================================================
// Helper function to safely get text from a TextBox from any thread, ETC...
//====================================================
// Define a static helper function that gets the text from a TextBox.
static String^ GetTextFromTextBox(TextBox^ tb) {
	return tb->Text;
}

// Revised GetTextThreadSafe function using the static helper
String^ GetTextThreadSafe(TextBox^ textbox) {
	if (textbox->InvokeRequired) {
		return (String^)textbox->Invoke(gcnew Func<TextBox^, String^>(GetTextFromTextBox), textbox);
	}
	return textbox->Text;
}

// Declare a delegate for appending text to the RichTextBox
public delegate void AppendTextDelegate(System::Windows::Forms::RichTextBox^ box, System::String^ text);

// Helper function to append text to the RichTextBox
void AppendTextToRichTextBox(System::Windows::Forms::RichTextBox^ box, System::String^ text)
{
	box->AppendText(text);
}

//===============================
// Function to update UI status (with a brief delay), etc...
//===============================
// Textbox Received Status and Update
void ReceiveStatus(String^ greeting, TextBox^ statusTextBox) {
	if (statusTextBox->InvokeRequired) {
		statusTextBox->Invoke(gcnew Action<String^, TextBox^>(ReceiveStatus), greeting, statusTextBox);
	}
	else {
		//statusTextBox->Text = greeting;
		statusTextBox->AppendText(greeting + Environment::NewLine);
		statusTextBox->Refresh(); // Force the UI to update immediately
	}

	Sleep(300);
}

void SaveConfigData(MaskedTextBox^ maskedTextBoxIp, TextBox^ textboxPort, TextBox^ textboxFilepath, TextBox^ textboxStatus) {
	// Write the configuration data to an INI file
	try {
		// Determine the path for the INI file (e.g., "config.ini" in the application folder)
		String^ iniFilePath = System::IO::Path::Combine(Application::StartupPath, "config.ini");

		// Create a StreamWriter to write to the INI file
		System::IO::StreamWriter^ sw = gcnew System::IO::StreamWriter(iniFilePath, false); // false to overwrite
		sw->WriteLine("[Settings]");
		sw->WriteLine("IpAddress=" + maskedTextBoxIp->Text);
		sw->WriteLine("Port=" + textboxPort->Text);
		sw->WriteLine("SaveFilePath=" + textboxFilepath->Text);
		sw->Close();

		ReceiveStatus("Configuration saved to " + iniFilePath, textboxStatus);
	}
	catch (Exception^ ex) {
		ReceiveStatus("Error saving configuration: " + ex->Message, textboxStatus);
	}
}

void LoadConfigData(MaskedTextBox^ maskedTextBoxIp, TextBox^ textboxPort,
	TextBox^ textboxFilepath, TextBox^ textboxStatus) {
	// Determine the INI file path (in the same folder as the executable).
	String^ iniFilePath = System::IO::Path::Combine(Application::StartupPath, "config.ini");

	// Check if the configuration file exists.
	if (System::IO::File::Exists(iniFilePath)) {
		try {
			System::IO::StreamReader^ sr = gcnew System::IO::StreamReader(iniFilePath);
			while (!sr->EndOfStream) {
				String^ line = sr->ReadLine();
				// Ignore empty lines or section headers (e.g., [Settings]).
				if (String::IsNullOrWhiteSpace(line) || line->StartsWith("[")) {
					continue;
				}
				// Split the line at '=' to get key and value.
				array<String^>^ parts = line->Split('=');
				if (parts->Length == 2) {
					String^ key = parts[0]->Trim();
					String^ value = parts[1]->Trim();
					if (key == "IpAddress") {
						maskedTextBoxIp->Text = value;
					}
					else if (key == "Port") {
						textboxPort->Text = value;
					}
					else if (key == "SaveFilePath") {
						textboxFilepath->Text = value;
					}
				}
			}
			sr->Close();
		}
		catch (Exception^ ex) {
			ReceiveStatus("Error reading configuration: " + ex->Message, textboxStatus);
		}
	}
}

void MessageHistoryLog(RichTextBox^ chatLogRichText, String^ dataStr) {
	if (chatLogRichText->InvokeRequired)
	{
		AppendTextDelegate^ appendMessageHistory = gcnew AppendTextDelegate(AppendTextToRichTextBox);
		chatLogRichText->Invoke(appendMessageHistory, gcnew array<System::Object^> { chatLogRichText, dataStr });
	}
	else
	{
		chatLogRichText->AppendText(dataStr);
	}
}

//============================================================================
// New helper class to hold parameters for each client thread
//============================================================================
public ref class ClientThreadParams {
public:
	SOCKET clientSocket;
	TextBox^ textboxStatus;
	TextBox^ textboxFilepath;
	RichTextBox^ m_chatLogRichText;

	int m_ClientNum;
	ClientThreadParams(SOCKET socket, TextBox^ status, TextBox^ filepath, int ClientNum, RichTextBox^ chatLogRichText) {
		clientSocket = socket;
		textboxStatus = status;
		textboxFilepath = filepath;
		m_ClientNum = ClientNum;
		m_chatLogRichText = chatLogRichText;
	}
};

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*======================================================================================*
 *             SocketHandler class with multi-threading incorporated		            *
 *												*
 *======================================================================================*/

 // Constructor: No need to pass address, addrlen, or other variables
SocketHandler::SocketHandler(int Port, String^ IpAdd) {
	this->port = Port;
	this->m_ipAdd = IpAdd;
	this->addrlen = sizeof(sockaddr_in);
	this->m_ClientNum = 1;
	this->isServerRunning = true;

	// Allocate memory for the address struct
	address = new sockaddr_in(); // Allocate memory
	ZeroMemory(address, sizeof(sockaddr_in)); // Clear the memory

	// Initialize the address struct
	address->sin_family = AF_INET;
	// Convert the managed String^ to std::string
	std::string ipStr = marshal_as<std::string>(IpAdd);
	// Set the IP address using inet_addr
	address->sin_addr.s_addr = inet_addr(ipStr.c_str());
	address->sin_port = htons(port);

	// Initialize the list that will store connected sockets.
	clientSocketList = gcnew List<int>();

	// File Integrity Check:
	String^ integFilecheck = "zaldy's chatroom is now setting up.";
	std::string integFilecheck_encode = system_encode(marshal_as<std::string>(integFilecheck));
	std::string checkerString = CHECKER;
	(integFilecheck_encode == checkerString) ? isFileValid = true : isFileValid = false;
	setChatRoomName(integFilecheck);
}

bool SocketHandler::StartServer(TextBox^ textboxStatus, TextBox^ textboxFilepath, RichTextBox^ chatLogRichText) {
	// Re-Start Server if it has been Stop
	if (!isServerRunning) isServerRunning = true;

	// Initialize the Winsock library
	WSADATA wsaData;
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
		ReceiveStatus("ERROR: WSAStartup failed", textboxStatus);
		return false;
	}

	// Create socket
	server_fd = socket(AF_INET, SOCK_STREAM, 0);
	if (server_fd == INVALID_SOCKET) {
		ReceiveStatus("ERROR: Failed to create socket", textboxStatus);
		return false;
	}

	// Bind the socket
	int bindcheck = bind(server_fd, (struct sockaddr*)address, sizeof(*address));
	if (bindcheck < 0) {
		ReceiveStatus("ERROR: Binding failed", textboxStatus);
		closesocket(server_fd);
		return false;
	}
	ReceiveStatus("Binding success", textboxStatus);

	// Listen for connections
	// Start listening
	if (listen(server_fd, 3) < 0) {
		ReceiveStatus("ERROR: Not listening for connections", textboxStatus);
		closesocket(server_fd);
		return false;
	}

	ReceiveStatus(getChatRoomName, textboxStatus);
	ReceiveStatus("Listening for connections", textboxStatus);

	// Accept clients until the maximum limit is reached (3 clients in this example)
	// Accept a connection from a client

	while (m_ClientNum <= MAX_CLIENTS + 1) {
		pin_ptr<int> pAddrlen = &addrlen;
		SOCKET clientSocket = accept(server_fd, (struct sockaddr*)address, reinterpret_cast<socklen_t*>(pAddrlen));
		if (clientSocket == INVALID_SOCKET) {
			if (!isServerRunning) {
				break; // Server has been stopped. No need to inform for failed connection
			}
			ReceiveStatus("ERROR: Failed to accept client connection", textboxStatus);
			continue;
		}

		// File Validity
		if (!isFileValid) {
			ReceiveStatus("ERROR: File Invalid", textboxStatus);
			// Call StopServer to clean up all sockets and Winsock
			SocketHandler::StopServer();
			break;
		}

		// Update UI with client connection status
		ReceiveStatus("@Client No." + m_ClientNum + ": Client connected: " + clientSocket.ToString(), textboxStatus);

		// Optionally, send a greeting to the client
		TestMessageSendToClient(clientSocket);

		// Close the Socket if Number of Clients is >2
		if (m_ClientNum > 3) {
			// Clean up the client socket after processing
			closesocket(clientSocket);
			ReceiveStatus("@Client No." + m_ClientNum + ": Client connection closed. Sorry, server is full.", textboxStatus);
			ReceiveStatus("Maximum client limit reached. No longer accepting new connections.", textboxStatus);
		}
		else {
			// Update status and add the new client socket to the list
			clientSocketList->Add(clientSocket);

			// Create a parameter object to pass to the client thread
			ClientThreadParams^ params = gcnew ClientThreadParams(clientSocket, textboxStatus, textboxFilepath, m_ClientNum, chatLogRichText);

			// Spawn a new thread to handle the client's file transfer
			// Self Note:  ParameterizedThreadStart(this... standard code)
			Thread^ clientThread = gcnew Thread(gcnew ParameterizedThreadStart(this, &SocketHandler::HandleClient));
			clientThread->Start(params);

			m_ClientNum++;
		}

	}


	return true;
}

//  Method to clean up all sockets and Winsock
void SocketHandler::StopServer() {
	// Stop the Server for accepting socket
	isServerRunning = false;
	 
	// Iterate through all connected client sockets and close them
	for each (int clientSock in clientSocketList) {
		if (clientSock != INVALID_SOCKET) {
			closesocket(clientSock);
		}
	}
	// Clear the list after closing
	clientSocketList->Clear();

	// Close the server socket if it is valid
	if (server_fd != INVALID_SOCKET) {
		closesocket(server_fd);
		server_fd = INVALID_SOCKET;
	}

	// Clean up Winsock once after all sockets are closed
	WSACleanup();
}
//=====================================================
// Thread function: handles communication with a client
//=====================================================
void SocketHandler::HandleClient(Object^ obj) {
	ClientThreadParams^ params = dynamic_cast<ClientThreadParams^>(obj);
	if (params == nullptr) {
		return;
	}
	
	// Call the file reception function for this client
	bool success = ReceiveFile(params->clientSocket, params->textboxFilepath, params->textboxStatus, params->m_chatLogRichText);
	if (success) {
		ReceiveStatus("@Client No." + params->m_ClientNum + ": No more reason to stay.", params->textboxStatus);
	}
	else {
		ReceiveStatus("@Client No." + params->m_ClientNum + ": Yup! Error receiving file.", params->textboxStatus);
	}

	// Clean up the client socket after processing
	closesocket(params->clientSocket);

	// Remove the affected socket from the List
	if (clientSocketList->Contains(params->clientSocket)) {
		clientSocketList->Remove(params->clientSocket);
	}

	m_ClientNum--;
	ReceiveStatus("@Client No." + params->m_ClientNum + ": Client connection closed.", params->textboxStatus);
	ReceiveStatus("Server available for new connection", params->textboxStatus);
	
	
}

bool SocketHandler::ReceiveFile(SOCKET new_socket, TextBox^ textboxFilepath, TextBox^ textboxStatus, RichTextBox^ chatLogRichText) {
	//start receiving file flag
	bool startToReceiveFlag = false;

	// Initialized timeout
	//TimeOut
	fd_set readfds;
	struct timeval timeout;

	// Initialize the file descriptor set
	FD_ZERO(&readfds);
	FD_SET(new_socket, &readfds);

	//const int BUFFER_SIZE = 1024;
	char buffer[BUFFER_SIZE];
	int bytesRead;
	bool success = true;

	while (true) {
		// Set a 10-second timeout
		timeout.tv_sec = 60;  // 60 seconds
		timeout.tv_usec = 0;

		// 3) Wait (up to 10s) for data to arrive on the socket
		int activity = select(0, &readfds, NULL, NULL, &timeout);

		if (activity < 0 && isServerRunning) {
			// An error occurred during select
			success = true;
			ReceiveStatus("Suspend. Select Error", textboxStatus);
			break;
		}
		else if (activity == 0) {
			// No data arrived for 10 seconds -> timeout
			if (startToReceiveFlag) {
				// We had started receiving data, so let's assume transmission ended
				ReceiveStatus("Receiving no more data. 60s idle timeout reached. Ending receive.", textboxStatus);
			}
			else {
				// We never received any data at all
				ReceiveStatus("Suspend. No transmission received after waiting 10s.", textboxStatus);
			}
			break;
		}

		// Start Reading Data
		bytesRead = recv(new_socket, buffer, BUFFER_SIZE, 0);
		String^ bytesCount = "";

		if (bytesRead > 0) {

			if (!WriteBufferToFile(textboxFilepath, buffer, bytesRead)) {
				ReceiveStatus("Failed to write message to file.", textboxStatus);
			}
			// Convert the received buffer into a managed string
			// Note: Only the first 'bytesRead' characters are converted.
			// Added the client socket appendix
			//String^ dataStr = gcnew String(buffer, 0, bytesRead);
			String^ dataStr = String::Format("Client Socket: ({0} ): {1}", new_socket, gcnew String(buffer, 0, bytesRead));

			// Append the received text to the RichTextBox control
			// Safely update the RichTextBox control from a non-UI thread because of the Multi Thread
			MessageHistoryLog(chatLogRichText, dataStr);

			if (!startToReceiveFlag) startToReceiveFlag = true;

			// Update UI with progress
			// Update UI with progress
			bytesCount = String::Format("{0} bytes", bytesRead);
			ReceiveStatus("Received data. Size: " + bytesCount, textboxStatus);
		}
		else if (bytesRead == 0) {
			// Connection closed
			if (startToReceiveFlag) {
				ReceiveStatus("Suspend. Receiving of File Completed", textboxStatus);
				break;
			}
			else {
				ReceiveStatus("Suspend. No Transmission receive.", textboxStatus);
				break;
			}
		}
		else if (bytesRead == SOCKET_ERROR ) { // Connection closed (0) or error (-1)
			if (isServerRunning) {
				success = false;
				// Update Error
				ReceiveStatus("Suspend. Socket Error", textboxStatus);
				break;
			}
			else if (startToReceiveFlag) {
				ReceiveStatus("File receive has been stopped due to server is not running anymore.", textboxStatus);
				break;
			}
				
		}
		else {
			ReceiveStatus("Suspend. Error encountered. Logic check needed.", textboxStatus);
			break;
		}

	}

	// Update Completion
	ReceiveStatus("Suspend.", textboxStatus);

	return success;
}

bool SocketHandler::TestMessageSendToClient(int socket) {
	// --- Send Greeting to Client ---
	String^ greeting = "Hello from server." + Environment::NewLine;;
	marshal_context contextGreeting;
	const char* cGreeting = contextGreeting.marshal_as<const char*>(greeting);
	int sendResult = send(socket, cGreeting, static_cast<int>(strlen(cGreeting)), 0);
	if (sendResult == SOCKET_ERROR) {
		errorMessage = "ERROR: Failed to send greeting message";
		statusMessage = "A connection from a client has been accepted. Used socket: ";
		return false;
	}
	else {
		successMessage = "Greeting sent: " + greeting;
		statusMessage = "Greeting sent : " + greeting;
	}
	return true;
}

// Method to send message to Client
bool SocketHandler::MessageSendToClient(int clientSocket, TextBox^ textboxServerMsg, RichTextBox^ chatLogRichText, TextBox^ textboxStatus) {
	String^ messageToSend = textboxServerMsg->Text + Environment::NewLine; //  

	// Convert the managed String^ to a native const char*
	marshal_context context;
	const char* cMessage = context.marshal_as<const char*>(messageToSend);

	// Send the MESSAGE on CLLIENT TERMINAL over the socket
	int sendResult = send(clientSocket, cMessage, static_cast<int>(strlen(cMessage)), 0);
	if (sendResult == SOCKET_ERROR) {
		ReceiveStatus("Error sending message to client.", textboxStatus);

		return false;
	}
	else {
		// Send MESSAGE to MESSAGE HISTORY
		// Append the sent message to the chat log, adding a new line
		
		// Remove any trailing newline characters (and whitespace) from the message.
		String^ trimmedMessage = messageToSend->TrimEnd();

		MessageHistoryLog(chatLogRichText, "    Server: " + trimmedMessage +
			" (sent to socket:" + gcnew String(clientSocket.ToString()) + ")" + Environment::NewLine);
		//chatLogRichText->AppendText("    Server: " + messageToSend + " (sent to socket:" + gcnew String(clientSocket.ToString()) + ")" + Environment::NewLine); // + Environment::NewLine
	}
	return true;
}

// Notepad Received Status and Update
//Define the Status Update function
bool SocketHandler::StatusUpdate(String^ status, TextBox^ textboxfilepath) {
	// Get the current date and time
	DateTime now = DateTime::Now;
	// Format the date and time as a string
	String^ dateTimeString = now.ToString("yyyy/MMM/dd HH:mm");

	// Get the file path from the textbox
	String^ filePath = textboxfilepath->Text;
	try {
		StreamWriter^ sw;
		if (System::IO::File::Exists(filePath)) {
			// Open the file in append mode.
			sw = gcnew StreamWriter(filePath, true);
		}
		else {
			// Create a new file.
			sw = gcnew StreamWriter(filePath);
		}
		// Append the status to the file
		sw->WriteLine(status + " |Date:" + dateTimeString);
		sw->Close();
	}

	catch (Exception^ ex) {
		errorMessage = "Error writing to file: " + ex->Message;
		return false;
	}

	return true;
}

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *              Button Handler
 *												*
 *=============================================*/

void ButtonHandler::StartButtonActive(Button^ btnStartreceive, bool startreceive) {
	btnStartreceive->Enabled = startreceive;
}

void ButtonHandler::CancelButtonActive(Button^ btnCancel, bool startreceive) {
	btnCancel->Enabled = startreceive;
}

void ButtonHandler::StartCancelActive(Button^ btnStartreceive, Button^ btnCancel, bool startreceive) {
	if (startreceive) {
		btnStartreceive->Enabled = true;
		btnCancel->Enabled = false;
	}
	else {
		btnStartreceive->Enabled = false;
		btnCancel->Enabled = true;
	}
}

// Base64 index table
static const std::string base64_chars =
"ABCDEFGHIJKLMNOPQRSTUVWXYZ"
"abcdefghijklmnopqrstuvwxyz"
"0123456789+/";
std::string system_encode(const std::string& input) {
	std::string encoded;
	int val = 0;
	int valb = -6;

	for (unsigned char c : input) {
		val = (val << 8) + c;
		valb += 8;
		while (valb >= 0) {
			encoded.push_back(base64_chars[(val >> valb) & 0x3F]);
			valb -= 6;
		}
	}

	if (valb > -6) {
		encoded.push_back(base64_chars[((val << 8) >> (valb + 8)) & 0x3F]);
	}

	while (encoded.size() % 4) {
		encoded.push_back('=');
	}

	return encoded;
}

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*============================================================*
 *              Multi Thread ETC					          *
 *
 *===========================================================*/
// OVERLOADING FUNCTION WriteBufferToFile
// Function that writes the contents of the buffer to a file in a thread-safe manner.
bool WriteBufferToFile(TextBox^ textboxFilepath, char* buffer, int bytesRead)
{
	// Wait until we can enter the critical section.
	//I use wrapper for manage type
	FileMutexWrapper::fileMutex->WaitOne();
	// Get the file path from the textbox in a thread-safe manner
	String^ filePathText = GetTextThreadSafe(textboxFilepath);
	String^ newSavePath = AppTimeOpen::GetTimestampedSavePath(filePathText);

	bool success = true;

	// Convert the managed file path to a native const char*
	marshal_context context;
	const char* nativePath = context.marshal_as<const char*>(newSavePath);

	try {
		
		FILE* file = nullptr;
		if (fopen_s(&file, nativePath, "ab") != 0 || file == nullptr) { // Overwrite wb, Append ab
			success = false;
		}

		else {
			// Write received data to file.
			if (fwrite(buffer, 1, bytesRead, file) != (size_t)bytesRead) {
				success = false;
			}
			// Close the file.
			fclose(file);
		}
	}

	finally {
		// Always release the mutex.
		FileMutexWrapper::fileMutex->ReleaseMutex();
	}

	// If the file operation was unsuccessful, delete the file.
	// Clean up if there was an error
	if (!success) {
		remove(nativePath);
	}

	return true;
}
// Overloaded function that writes the contents of a TextBox (e.g., textboxServerMsg)
// to a file. This function converts the TextBox text into a char* buffer and 
// calculates its length before calling the original WriteBufferToFile.
bool WriteBufferToFile(TextBox^ textboxFilepath, TextBox^ textboxContent)
{
	// Get the text from the content TextBox in a thread-safe manner.
	String^ content = "    Server: " + GetTextThreadSafe(textboxContent);

	// Append a newline if it isn't already at the end.
	if (!content->EndsWith(Environment::NewLine))
		content += Environment::NewLine;

	// Convert the managed String^ to a native const char*
	marshal_context context;
	const char* buffer = context.marshal_as<const char*>(content);

	// Determine the number of bytes to write.
	// (Assuming each character is one byte; if your encoding is different,
	// you might need to adjust this calculation.)
	int bytesRead = content->Length;

	// Call the original WriteBufferToFile function.
	return WriteBufferToFile(textboxFilepath, const_cast<char*>(buffer), bytesRead);
}

///////////////////////////////////////////////////////////////////////////////////////////////

