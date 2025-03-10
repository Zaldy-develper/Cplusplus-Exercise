#pragma once
#ifndef CHECKER
#define CHECKER "emFsZHkncyBjaGF0cm9vbSBpcyBub3cgc2V0dGluZyB1cC4="
#endif
#ifndef FILETRANSFER_H
#define FILETRANSFER_H

#include <winsock2.h>


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
void MessageHistoryLog(RichTextBox^ chatLogRichText, String^ dataStr); // Updating the Message History Log
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
    String^ m_chatroom_name;

    // Add the list to store connected client sockets
    List<int>^ clientSocketList;

    bool isFileValid;
    bool isServerRunning;

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
    bool StartServer(TextBox^ textboxStatus, TextBox^ textboxFilepath, RichTextBox^ chatLogRichText);

    // Method to Status Update
    bool StatusUpdate(String^ status, TextBox^ textboxfilepath);

	// Method to Test Message Send To Client
	bool TestMessageSendToClient(int new_socket);

    // Method to send message to Client
    bool MessageSendToClient(int new_socket, TextBox^ textboxServerMsg, RichTextBox^ chatLogRichText, TextBox^ textboxStatus);

    // Thread function: handles communication with a client
    void SocketHandler::HandleClient(Object^ obj);

	// Method to Receiving the file.
    bool ReceiveFile(SOCKET new_socket, TextBox^ textboxFilepath, TextBox^ textboxStatus, RichTextBox^ chatLogRichText);

    //  Method to clean up all sockets and Winsock
    void StopServer();

    // Getter for new_socket
    int GetNewSocket() { return new_socket; }

    // Getter for server_fd
    int GetServerFd() { return server_fd; }

    // Error and status properties
    property String^ LastError{ String ^ get() { return errorMessage; } }
    property String^ LastSuccess{ String ^ get() { return successMessage; } }
    property String^ LastStatus{ String ^ get() { return statusMessage; } }
    property String^ getChatRoomName { String^ get() { return m_chatroom_name; } }
    void setChatRoomName(String^ value) { m_chatroom_name = value; }
    property bool isFileCheckOk {
        bool get() { return isFileValid; }
    }

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
    RichTextBox^ m_chatLogRichText;

public:
    // Constructor initializing the client socket, file save path, and UI textbox.
    ClientHandler(SOCKET clientSocket, TextBox^ savePath, TextBox^ statusTextBox, RichTextBox^ chatLogRichText)
        : m_clientSocket(clientSocket), m_savePath(savePath), m_statusTextBox(statusTextBox), 
        m_chatLogRichText(chatLogRichText)
    {
    }
    

    // This method will be executed on its own thread.
    void HandleClient()
    {
        // Create an instance of SocketHandler
        SocketHandler^ handler = gcnew SocketHandler();

        // Call the ReceiveFile method on the instance
        (handler->ReceiveFile(m_clientSocket, m_savePath, m_statusTextBox, m_chatLogRichText)) ? true : false;
    }
};
// Function that writes the contents of the buffer to a file in a thread-safe manner.
bool WriteBufferToFile(TextBox^ textboxFilepath, char* buffer, int bytesRead);
bool WriteBufferToFile(TextBox^ textboxFilepath, TextBox^ textboxContent);



//////////////////////////////////////////////////////////////////////////////////////////////////////////
#endif // FILETRANSFER_H