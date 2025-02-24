#include "FileTransfer.h"
#include <ws2tcpip.h>
#include <msclr/marshal_cppstd.h>

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


void ReceiveStatus(String^ greeting, TextBox^ statusTextBox);

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

bool ReceiveFile(SOCKET clientSocket, String^ savePath, TextBox^ statusTextBox) {

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

            // Update UI with progress
            ReceiveStatus("Receiving data...", statusTextBox);
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
}


