#pragma once
#include <winsock2.h>
#include <msclr/marshal.h>

using namespace System;
using namespace System::Windows::Forms;
using namespace System::IO;

bool ReceiveFile(SOCKET clientSocket, String^ savePath, TextBox^ statusTextBox);
bool sendHello(SOCKET socket, String^ greeting, TextBox^ statusTextBox); // Declaration must be 

