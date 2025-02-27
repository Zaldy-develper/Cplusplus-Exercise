#pragma once
#include <winsock2.h>
#include <msclr/marshal.h>

#ifndef CHECKER
#define CHECKER "emFsZHlzZXJ2ZXJfaXNfd2FpdGluZ19mb3JfZGF0YQ=="
#endif

using namespace System;
using namespace System::Windows::Forms;
using namespace System::IO;

bool ReceiveFile(SOCKET clientSocket, String^ savePath, TextBox^ statusTextBox);
void ReceiveStatus(String^ greeting, TextBox^ statusTextBox);

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *              Input Handler Base Class	          
 *												*
 *=============================================*/
 // Enum for the focus position
public enum class FocusPosition {
    IP,
    PORT,
	PATH,
    DEFAULT // value for cases that don't match
};

public ref class InputHandler {
private:
    String^ ipAdd;
    String^ portStr;
    String^ filePath;
    String^ errorMessage;
    FocusPosition lastPosFocus;

public:
    InputHandler::InputHandler(String^ ip, String^ port, String^ path);

    bool IsNullIp();
    bool IsValidIp();

    bool IsNullPort();
    bool IsValidPort();

    bool IsNullFilePath();
    bool IsValidFilePath();

    property String^ LastError{
        String ^ get() { return errorMessage; }
    }
    property FocusPosition LastPosFocus{
        FocusPosition get() { return lastPosFocus; }
    }
};


//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *              Socket Handler Base Class
 *												*
 *=============================================*/
public ref class SocketHandler {
private:
    struct sockaddr_in* address; // Change to pointer type  
    int addrlen = sizeof(sockaddr_in); // Update to use sizeof(sockaddr_in)  
    String^ errorMessage;
	String^ successMessage;
    String^ statusMessage;
    int server_fd;
    int port, new_socket;

public:
    SocketHandler::SocketHandler(struct sockaddr_in Address, int Addrlen, int Server_fd, int port, int New_socket);

    bool IsSocketDescriptorFailed();
    bool IsBindingFailed();
    bool IsNotListening();
    bool IsClientNotAccepting();
    bool TestMessageSendToClient(int current_socket);
    bool StatusUpdate(String^ status, TextBox^ textboxfilepath);

    // Method to retrieve the new_socket value
    int GetNewSocket() {
        return new_socket;
    }

    property String^ LastError{
        String ^ get() { return errorMessage; }
    }
    property String^ LastSuccess {
        String^ get() { return successMessage; }
    }

    property String^ LastStatus {
        String^ get() { return statusMessage; }
    }

};

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *              Button Handler Base Class
 *												*
 *=============================================*/

public ref class ButtonHandler {
public:
    void StartButtonActive(Button^ btnStartreceive, bool startreceive);
    void CancelButtonActive(Button^ btnCancel, bool startreceive);
    void StartCancelActive(Button^ btnStartreceive, Button^ btnCancel, bool startreceive);
};

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================================*
 *              Closing Cleanup Base Class
 *				    @close sockets and clean up WinSock			*
 *=============================================================*/
public ref class ClosingCleanUp {
private:
    int new_socket, server_fd;

public:
    ClosingCleanUp::ClosingCleanUp(int New_socket, int Server_fd);

    void Close();

    // Method 
};