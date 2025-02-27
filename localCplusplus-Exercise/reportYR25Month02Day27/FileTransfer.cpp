#include "FileTransfer.h"
#include <ws2tcpip.h>
#include <msclr/marshal_cppstd.h>
#include <windows.h>
#include <tchar.h>
#include <strsafe.h>

using namespace msclr::interop;

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

// Responsible for receiving the file.
bool ReceiveFile(SOCKET clientSocket, String^ savePath, TextBox^ statusTextBox) {
	//start receiving file flag
	bool startToReceiveFlag = false;

	//TimeOut
	fd_set readfds;
	struct timeval timeout;

	// Initialize the file descriptor set
	FD_ZERO(&readfds);
	FD_SET(clientSocket, &readfds);

	// Set the timeout duration (e.g., 30 seconds)
	timeout.tv_sec = 5;
	timeout.tv_usec = 0;

	//End TimeOut

	// Convert String^ to const char*
	String^ newSavePath = MyClass::GetTimestampedSavePath(savePath);
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
		bytesRead = recv(clientSocket, buffer, BUFFER_SIZE, 0);
		String^ bytesCount = "";

		if (bytesRead > 0) {
			// Write received data to file
			fwrite(buffer, 1, bytesRead, file);
			if (!startToReceiveFlag) startToReceiveFlag = true;
				
			// Update UI with progress
			// Update UI with progress
			bytesCount = String::Format("{0} bytes", bytesRead);
			ReceiveStatus("Receiving data...Size: "+ bytesCount, statusTextBox);
		}
		else if (bytesRead == 0) {
			// Connection closed
			if (startToReceiveFlag) {
				ReceiveStatus("Suspend. Receiving of File Completed", statusTextBox);
				break;
			}
			else {
				ReceiveStatus("Suspend. No Transmission receive.", statusTextBox);
				break;
			}
		}
		else {
			// Connection closed (0) or error (-1)
			if (bytesRead == SOCKET_ERROR) {
				success = false;
			}
			// Update Error
			ReceiveStatus("Suspend. Socket Error", statusTextBox);
			break;
		}

		if (startToReceiveFlag && bytesRead <3) {
			// Call select to monitor the client socket for readability with the specified timeout
			int activity = select(0, &readfds, NULL, NULL, &timeout);
			String^ activityString = "";

			if (activity == 0) {
				// Timeout occurred
				ReceiveStatus("Receiving no more data. Connection Timeout. Data Size: " + bytesCount, statusTextBox);
				ReceiveStatus("Suspend.", statusTextBox);
				break;
			}
			else if (activity > 0) {
				// One or more file descriptors are ready (i.e., data is available to read)
				if (FD_ISSET(clientSocket, &readfds)) {
					// Handle the readable socket (e.g., call recv to read data)
				}
			}
			else {
				// An error occurred
				ReceiveStatus("Suspend. Select Error. Data Size: " + bytesCount, statusTextBox);
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
	ReceiveStatus("Suspend.", statusTextBox);

	return success;
}

// Textbox Received Status and Update
void ReceiveStatus(String^ greeting, TextBox^ statusTextBox) {
    statusTextBox->Text = greeting;
    statusTextBox->Refresh(); // Force the UI to update immediately
    Sleep(1000);
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

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *              Socket Handler		            *
 *												*
 *=============================================*/

// Update the SocketHandler constructor to use the Address struct
SocketHandler::SocketHandler(struct sockaddr_in Address, int Addrlen, int Server_fd, int Port, int New_socket)
	: addrlen(Addrlen), server_fd(Server_fd), errorMessage(nullptr), port(Port), new_socket(New_socket){
	address = new sockaddr_in(Address);
}

bool SocketHandler::IsSocketDescriptorFailed() {
	if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == 0) {
		perror("socket failed");
		errorMessage = "ERROR: socket failed";
		exit(EXIT_FAILURE);
		return true;
	}
	else
	{
		// サーバーアドレスの設定
		//Setting the server address
		address->sin_family = AF_INET;
		address->sin_addr.s_addr = INADDR_ANY;
		address->sin_port = htons(port);
		return false;
	}
}

bool SocketHandler::IsBindingFailed() {
	if (bind(server_fd, (struct sockaddr*)address, sizeof(*address)) < 0) {
		errorMessage = "ERROR: Binding failed";
		perror("bind failed");
		closesocket(server_fd);
		exit(EXIT_FAILURE);
		return true;
	}
	else {
		successMessage = "Binding success";
		statusMessage = "Binding success";
	}
}

bool SocketHandler::IsNotListening() {
	if (listen(server_fd, 3) < 0) {
		errorMessage = "ERROR: Not listening for connections.";//ERROR:接続をリッスンしません。
		statusMessage = "ERROR: Not listening for connections.";
		perror("listen");
		closesocket(server_fd);
		exit(EXIT_FAILURE);
		return true;
	}
	else {
		successMessage = "Listened for connections.";// 接続をリッスンしました。
		statusMessage = "Listened for connections.";
		return false;
	}
		
}

bool SocketHandler::IsClientNotAccepting() {
	// Pin the managed addrlen field so its address is fixed in memory.
	pin_ptr<int> pAddrlen = &addrlen;

	if ((new_socket = accept(server_fd, (struct sockaddr*)address, reinterpret_cast<socklen_t*>(pAddrlen))) < 0) {
		errorMessage = "ERROR: Not accepting connections from clients.";//ERROR:クライアントからの接続を受け入れません。
		statusMessage = "ERROR: Not accepting connections from clients.";
		perror("accept");
		closesocket(server_fd);
		exit(EXIT_FAILURE);
		return true;
	}
	else {
		successMessage = "A connection from a client has been accepted. Used socket: ";// クライアントからの接続を受け入れました。
		statusMessage = "A connection from a client has been accepted. Used socket: " + new_socket;
		return false;
	}

}

bool SocketHandler::TestMessageSendToClient(int current_socket){
	// --- Send Greeting to Client ---
	String^ greeting = "Hello from server.";
	marshal_context contextGreeting;
	const char* cGreeting = contextGreeting.marshal_as<const char*>(greeting);
	int sendResult = send(current_socket, cGreeting, static_cast<int>(strlen(cGreeting)), 0);
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