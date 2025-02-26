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


		if (bytesRead > 0) {
			// Write received data to file
			fwrite(buffer, 1, bytesRead, file);
			if (!startToReceiveFlag) startToReceiveFlag = true;
				
			// Update UI with progress
			ReceiveStatus("Receiving data...", statusTextBox);
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

//++++++++++++++++INPUTHANDLER++++++++++++++++++++++++++++++//
//															//
//		Ip Address, Port, Save File Path					//
//       													//
//++++++++++++++++INPUTHANDLER.H++++++++++++++++++++++++++++++

InputHandler::InputHandler(String^ ip, String^ port, String^ path)
	: ipAdd(ip), portStr(port), filePath(path), errorMessage(nullptr) {
}

bool InputHandler::IsNullIp() {
	if (String::IsNullOrWhiteSpace(ipAdd)) {
		errorMessage = "Please fill the IP address field.";
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
		return false;
	}
	return true;
}

bool InputHandler::IsNullPort() {
	if (String::IsNullOrWhiteSpace(portStr)) {
		errorMessage = "Please fill the Port number field.";
		return true;
	}
	return false;
}

bool InputHandler::IsValidPort() {
	int port;
	if (!Int32::TryParse(portStr, port)) {
		errorMessage = "Invalid port number. Not a valid integer.";
		return false;
	}
	if (port < 1024 || port > 65535) {
		errorMessage = "Invalid port number. Allowable port (1024-65535)";
		return false;
	}
	return true;
}

bool InputHandler::IsNullFilePath() {
	if (String::IsNullOrWhiteSpace(filePath)) {
		errorMessage = "Please fill the Save File Path field.";
		return true;
	}
	return false;
}

bool InputHandler::IsValidFilePath() {
	if (filePath->IndexOfAny(Path::GetInvalidPathChars()) >= 0) {
		errorMessage = "Invalid characters in file path.";
		return false;
	}

	if (!Path::IsPathRooted(filePath)) {
		errorMessage = "File path is not absolute.";
		return false;
	}

	String^ directory = Path::GetDirectoryName(filePath);
	if (!Directory::Exists(directory)) {
		errorMessage = "Directory does not exist.";
		return false;
	}

	try {
		Directory::GetFiles(directory); // Check write access
	}
	catch (UnauthorizedAccessException^) {
		errorMessage = "No write permission to the directory.";
		return false;
	}
	catch (Exception^) {
		errorMessage = "Invalid directory.";
		return false;
	}

	return true;
}

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//


