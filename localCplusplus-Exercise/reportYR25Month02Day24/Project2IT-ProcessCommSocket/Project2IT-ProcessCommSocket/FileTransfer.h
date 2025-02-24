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
