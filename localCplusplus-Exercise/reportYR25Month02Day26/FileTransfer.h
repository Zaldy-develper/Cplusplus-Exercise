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

//////////////////////////////////////////////
//                                          //
//      Input Handler Base Class            //
//                                          //
// ///////////////////////////////////////////
public ref class InputHandler {
private:
    String^ ipAdd;
    String^ portStr;
    String^ filePath;
    String^ errorMessage;

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
};


