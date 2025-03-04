#pragma once
#include <winsock2.h>
#include <msclr/marshal.h>


#ifndef CHECKER
#define CHECKER "emFsZHlzZXJ2ZXJfaXNfd2FpdGluZ19mb3JfZGF0YQ=="
#endif

using namespace System;
using namespace System::Windows::Forms;
using namespace System::IO;
using namespace System::Collections::Generic; // necessary for the List

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *           Function Status Update etc..
 *												*
 *=============================================*/
void ReceiveStatus(String^ greeting, TextBox^ statusTextBox);
void SaveConfigData(MaskedTextBox^ maskedTextBoxIp, TextBox^ textboxPort, TextBox^ textboxFilepath, TextBox^ textboxStatus);
void LoadConfigData(MaskedTextBox^ maskedTextBoxIp, TextBox^ textboxPort, TextBox^ textboxFilepath, TextBox^ textboxStatus);

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

   /* bool IsAllInputValid();*/

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
    sockaddr_in* address;

    int addrlen;
    int server_fd;
    int port;
    int new_socket;
    int m_ClientNum;

    String^ errorMessage;
    String^ successMessage;
    String^ statusMessage;
    String^ m_ipAdd;

    // Add the list to store connected client sockets
    List<int>^ clientSocketList;

public:
    // Static instance for global access
    static SocketHandler^ Instance;

    // Constructor: No need to pass address, addrlen, or other variables
    SocketHandler(int port, String^ ipAdd);
        
    //SocketHandler(); // Constructor without parameters
    SocketHandler() : port(0) {
        clientSocketList = gcnew List<int>();
    }

    // Method to start the server
    bool StartServer(TextBox^ textboxStatus, TextBox^ textboxFilepath);

    // Method to Status Update
    bool StatusUpdate(String^ status, TextBox^ textboxfilepath);

	// Method to Test Message Send To Client
	bool TestMessageSendToClient(int new_socket);

    // Method to send message to Client
    bool MessageSendToClient(int new_socket, TextBox^ textboxServerMsg, RichTextBox^ chatLogRichText, TextBox^ textboxStatus);

    // Thread function: handles communication with a client
    void SocketHandler::HandleClient(Object^ obj);

	// Method to Receiving the file.
    bool ReceiveFile(SOCKET new_socket, TextBox^ textboxFilepath, TextBox^ textboxStatus);

    // Getter for new_socket
    int GetNewSocket() { return new_socket; }

    // Getter for server_fd
    int GetServerFd() { return server_fd; }

    // Error and status properties
    property String^ LastError{ String ^ get() { return errorMessage; } }
    property String^ LastSuccess{ String ^ get() { return successMessage; } }
    property String^ LastStatus{ String ^ get() { return statusMessage; } }

    // Optionally, expose a property to get the list
    property List<int>^ ClientSockets{
        List<int> ^ get() { return clientSocketList; }
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

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*============================================================*
 *              Creating Multi Thread 					          *
 *
 *===========================================================*/
 // This class handles a single client's connection.
public ref class ClientHandler
{
private:
    SOCKET m_clientSocket;
    TextBox^ m_savePath;
    TextBox^ m_statusTextBox;

public:
    // Constructor initializing the client socket, file save path, and UI textbox.
    ClientHandler(SOCKET clientSocket, TextBox^ savePath, TextBox^ statusTextBox)
        : m_clientSocket(clientSocket), m_savePath(savePath), m_statusTextBox(statusTextBox)
    {
    }
    

    // This method will be executed on its own thread.
    void HandleClient()
    {
        // Create an instance of SocketHandler
        SocketHandler^ handler = gcnew SocketHandler();

        // Call the ReceiveFile method on the instance
        (handler->ReceiveFile(m_clientSocket, m_savePath, m_statusTextBox)) ? true : false;
    }
};