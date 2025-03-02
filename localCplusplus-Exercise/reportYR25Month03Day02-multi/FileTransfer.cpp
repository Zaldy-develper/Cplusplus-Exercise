#include "FileTransfer.h"
#include <ws2tcpip.h>
#include <msclr/marshal_cppstd.h>
#include <windows.h>
#include <tchar.h>
#include <strsafe.h>
#include <vcclr.h>  // Required for gcroot
#include <winsock2.h>

using namespace msclr::interop;
using namespace System::Threading;

// Check if the watermark macro is defined
#ifndef CHECKER
#error "No checker"
#endif

using namespace System;
using namespace System::Text;
using namespace System::IO;
using namespace System::Windows::Forms;
using namespace msclr::interop;

#define BUFFER_SIZE 1024
#define MAX_CLIENTS 3

//===============================
// Helper class for timestamped file naming
//===============================
public ref class MyClass {
public:
    static String^ GetTimestampedSavePath(String^ savePath) {
        String^ directory = Path::GetDirectoryName(savePath);
        String^ filenameWithoutExtension = Path::GetFileNameWithoutExtension(savePath);
        String^ extension = Path::GetExtension(savePath);
        String^ timeStamp = DateTime::Now.ToString("yyyyMMdd_HHmmss");
        String^ newFilename = String::Format("{0}_{1}{2}", filenameWithoutExtension, timeStamp, extension);
        return Path::Combine(directory, newFilename);
    }
};

//===============================
// Function to update UI status (with a brief delay)
//===============================
// Textbox Received Status and Update
void ReceiveStatus(String^ greeting, TextBox^ statusTextBox) {
	if (statusTextBox->InvokeRequired) {
		statusTextBox->Invoke(gcnew Action<String^, TextBox^>(ReceiveStatus), greeting, statusTextBox);
	}
	else {
		statusTextBox->Text = greeting;
		statusTextBox->Refresh(); // Force the UI to update immediately
	}
    
    Sleep(1000);
}

//====================================================
// Helper function to safely get text from a TextBox from any thread
//====================================================
// Define a static helper function that gets the text from a TextBox.
static String ^ GetTextFromTextBox(TextBox ^ tb) {
	return tb->Text;
}

// Revised GetTextThreadSafe function using the static helper
String^ GetTextThreadSafe(TextBox^ textbox) {
	if (textbox->InvokeRequired) {
		return (String^)textbox->Invoke(gcnew Func<TextBox^, String^>(GetTextFromTextBox), textbox);
	}
	return textbox->Text;
}

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*====================================================================*
 *              Ip Address, Port, Save File Path	  	              *
 *																	  *
 *===================================================================*/

InputHandler::InputHandler(String^ ip, String^ port, String^ path)
	: ipAdd(ip), portStr(port), filePath(path), errorMessage(nullptr) {
}

bool InputHandler::IsNullIp() {
	if (String::IsNullOrWhiteSpace(ipAdd)) {
		errorMessage = "Please fill the IP address field.";
		lastPosFocus = FocusPosition::IP; // Set the focus position
		return true;
	}
	return false;
}

bool InputHandler::IsValidIp() {
	marshal_context context;
	std::string ip = context.marshal_as<std::string>(ipAdd);
	sockaddr_in sa;
	if (inet_pton(AF_INET, ip.c_str(), &(sa.sin_addr)) != 1) {
		errorMessage = "Incorrect IP address has been entered.";
		lastPosFocus = FocusPosition::IP; // Set the focus position
		return false;
	}
	return true;
}

bool InputHandler::IsNullPort() {
	if (String::IsNullOrWhiteSpace(portStr)) {
		errorMessage = "Please fill the Port number field.";
		lastPosFocus = FocusPosition::PORT; // Set the focus position
		return true;
	}
	return false;
}

bool InputHandler::IsValidPort() {
	int port;
	if (!Int32::TryParse(portStr, port)) {
		errorMessage = "Invalid port number. Not a valid integer.";
		lastPosFocus = FocusPosition::PORT; // Set the focus position
		return false;
	}
	if (port < 1024 || port > 65535) {
		errorMessage = "Invalid port number. Allowable port (1024-65535)";
		lastPosFocus = FocusPosition::PORT; // Set the focus position
		return false;
	}
	return true;
}

bool InputHandler::IsNullFilePath() {
	if (String::IsNullOrWhiteSpace(filePath)) {
		errorMessage = "Please fill the Save File Path field.";
		lastPosFocus = FocusPosition::PATH; // Set the focus position
		return true;
	}
	return false;
}

bool InputHandler::IsValidFilePath() {
	if (filePath->IndexOfAny(Path::GetInvalidPathChars()) >= 0) {
		errorMessage = "Invalid characters in file path.";
		lastPosFocus = FocusPosition::PATH; // Set the focus position
		return false;
	}

	if (!Path::IsPathRooted(filePath)) {
		errorMessage = "File path is not absolute.";
		lastPosFocus = FocusPosition::PATH; // Set the focus position
		return false;
	}

	String^ directory = Path::GetDirectoryName(filePath);
	if (!Directory::Exists(directory)) {
		errorMessage = "Directory does not exist.";
		lastPosFocus = FocusPosition::PATH; // Set the focus position
		return false;
	}

	try {
		Directory::GetFiles(directory); // Check write access
	}
	catch (UnauthorizedAccessException^) {
		errorMessage = "No write permission to the directory.";
		lastPosFocus = FocusPosition::PATH; // Set the focus position
		return false;
	}
	catch (Exception^) {
		errorMessage = "Invalid directory.";
		lastPosFocus = FocusPosition::PATH; // Set the focus position
		return false;
	}

	return true;
}

//=====================================================
// New helper class to hold parameters for each client thread
//=====================================================
public ref class ClientThreadParams {
public:
	SOCKET clientSocket;
	TextBox^ textboxStatus;
	TextBox^ textboxFilepath;
	ClientThreadParams(SOCKET socket, TextBox^ status, TextBox^ filepath) {
		clientSocket = socket;
		textboxStatus = status;
		textboxFilepath = filepath;
	}
};

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*======================================================================================*
 *             SocketHandler class with multi-threading incorporated		            *
 *												*
 *======================================================================================*/

// Constructor: No need to pass address, addrlen, or other variables
SocketHandler::SocketHandler(int Port) {
	this->port = Port;
	this->addrlen = sizeof(sockaddr_in);

	// Allocate memory for the address struct
	address = new sockaddr_in(); // Allocate memory
	ZeroMemory(address, sizeof(sockaddr_in)); // Clear the memory

	// Initialize the address struct
	address->sin_family = AF_INET;
	address->sin_addr.s_addr = INADDR_ANY;
	address->sin_port = htons(port);

}


bool SocketHandler::StartServer(TextBox^ textboxStatus, TextBox^ textboxFilepath) {
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
	if (bind(server_fd, (struct sockaddr *)address, sizeof(*address)) < 0) {
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
	ReceiveStatus("Listening for connections", textboxStatus);

	// Accept clients until the maximum limit is reached (3 clients in this example)
	// Accept a connection from a client
	int clientCount = 0;

	while (clientCount < MAX_CLIENTS) {
		pin_ptr<int> pAddrlen = &addrlen;
		SOCKET clientSocket = accept(server_fd, (struct sockaddr*)address, reinterpret_cast<socklen_t*>(pAddrlen));
		if (clientSocket == INVALID_SOCKET) {
			ReceiveStatus("ERROR: Failed to accept client connection", textboxStatus);
			continue;
		}
		// Update UI with client connection status
		ReceiveStatus("Client connected: " + clientSocket.ToString(), textboxStatus);

		// Optionally, send a greeting to the client
		TestMessageSendToClient(clientSocket);

		// Create a parameter object to pass to the client thread
		ClientThreadParams^ params = gcnew ClientThreadParams(clientSocket, textboxStatus, textboxFilepath);

		// Spawn a new thread to handle the client's file transfer
		// Self Note:  ParameterizedThreadStart(this... standard code)
		Thread^ clientThread = gcnew Thread(gcnew ParameterizedThreadStart(this, &SocketHandler::HandleClient));
		clientThread->Start(params);

		clientCount++;
	}

	ReceiveStatus("Maximum client limit reached. No longer accepting new connections.", textboxStatus);
	return true;
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
	bool success = ReceiveFile(params->clientSocket, params->textboxFilepath, params->textboxStatus);
	if (success) {
		ReceiveStatus("File received successfully.", params->textboxStatus);
	}
	else {
		ReceiveStatus("Error receiving file.", params->textboxStatus);
	}

	// Clean up the client socket after processing
	closesocket(params->clientSocket);
	ReceiveStatus("Client connection closed.", params->textboxStatus);
}

bool SocketHandler::ReceiveFile(SOCKET new_socket, TextBox^ textboxFilepath, TextBox^ textboxStatus) {
	// Get the file path from the textbox in a thread-safe manner
	String^ filePathText = GetTextThreadSafe(textboxFilepath);
	String^ newSavePath = MyClass::GetTimestampedSavePath(filePathText);

	//start receiving file flag
	bool startToReceiveFlag = false;

	//TimeOut
	fd_set readfds;
	struct timeval timeout;

	// Initialize the file descriptor set
	FD_ZERO(&readfds);
	FD_SET(new_socket, &readfds);

	// Set the timeout duration (e.g., 30 seconds)
	timeout.tv_sec = 5;
	timeout.tv_usec = 0;

	//End TimeOut

	//// Convert String^ to const char*
	//String^ newSavePath = MyClass::GetTimestampedSavePath(textboxFilepath->Text);
	marshal_context context;
	const char* cSavePath = context.marshal_as < const char* >(newSavePath);

	// Open file for writing in binary mode
	FILE* file;
	fopen_s(&file, cSavePath, "wb");
	if (file == nullptr) {
		return false;
	}

	//const int BUFFER_SIZE = 1024;
	char buffer[BUFFER_SIZE];
	int bytesRead;
	bool success = true;

	while (true) {
		bytesRead = recv(new_socket, buffer, BUFFER_SIZE, 0);
		String^ bytesCount = "";

		if (bytesRead > 0) {
			// Write received data to file
			fwrite(buffer, 1, bytesRead, file);
			if (!startToReceiveFlag) startToReceiveFlag = true;

			// Update UI with progress
			// Update UI with progress
			bytesCount = String::Format("{0} bytes", bytesRead);
			ReceiveStatus("Receiving data...Size: " + bytesCount, textboxStatus);
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
		else {
			// Connection closed (0) or error (-1)
			if (bytesRead == SOCKET_ERROR) {
				success = false;
			}
			// Update Error
			ReceiveStatus("Suspend. Socket Error", textboxStatus);
			break;
		}

		if (startToReceiveFlag && bytesRead < 3) {
			// Call select to monitor the client socket for readability with the specified timeout
			int activity = select(0, &readfds, NULL, NULL, &timeout);
			String^ activityString = "";

			if (activity == 0) {
				// Timeout occurred
				ReceiveStatus("Receiving no more data. Connection Timeout. Data Size: " + bytesCount, textboxStatus);
				ReceiveStatus("Suspend.", textboxStatus);
				break;
			}
			else if (activity > 0) {
				// One or more file descriptors are ready (i.e., data is available to read)
				if (FD_ISSET(new_socket, &readfds)) {
					// Handle the readable socket (e.g., call recv to read data)
				}
			}
			else {
				// An error occurred
				ReceiveStatus("Suspend. Select Error. Data Size: " + bytesCount, textboxStatus);
				break;
			}
		}
	}

	fclose(file);

	// Clean up if there was an error
	if (!success) {
		remove(cSavePath);
	}

	// Update Completion
	ReceiveStatus("Suspend.", textboxStatus);

	return success;
}

bool SocketHandler::TestMessageSendToClient(int new_socket){
	// --- Send Greeting to Client ---
	String^ greeting = "Hello from server.";
	marshal_context contextGreeting;
	const char* cGreeting = contextGreeting.marshal_as<const char*>(greeting);
	int sendResult = send(new_socket, cGreeting, static_cast<int>(strlen(cGreeting)), 0);
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
		//MessageBox::Show("Error writing to file: " + ex->Message, "Error", MessageBoxButtons::OK, MessageBoxIcon::Error);
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

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*============================================================*
 *              Closing Cleanup 					          *
 *					@close sockets and clean up WinSock				
 *===========================================================*/

 // Update the SocketHandler constructor to use the Address struct
ClosingCleanUp::ClosingCleanUp(int New_socket, int Server_fd)
	: new_socket(New_socket), server_fd(Server_fd) {
}

void ClosingCleanUp::Close() {
	closesocket(new_socket);
	closesocket(server_fd);
	WSACleanup();
}

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*============================================================*
 *              Creating Multi Thread 					          *
 *					
 *===========================================================*/
