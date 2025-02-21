#include "FileTransfer.h"
#include <ws2tcpip.h>
#include <msclr/marshal_cppstd.h>
using namespace System;
using namespace System::Text;

#define BUFFER_SIZE 1024

// class definition
public ref class MyClass {
    // Other members...

public:
    static String^ GetTimestampedSavePath(String^ savePath) {
        // Original savePath is a String^ representing your file path
        String^ directory = Path::GetDirectoryName(savePath);
        String^ filenameWithoutExtension = Path::GetFileNameWithoutExtension(savePath);
        String^ extension = Path::GetExtension(savePath);

        // Get the current date and time
        DateTime now = DateTime::Now;

        // Format the timestamp (e.g., "20231031_154530")
        String^ timeStamp = now.ToString("yyyyMMdd_HHmmss");

        // Combine the filename with the timestamp
        String^ newFilename = String::Format("{0}_{1}{2}", filenameWithoutExtension, timeStamp, extension);

        // Create the new savePath with the timestamped filename
        String^ newSavePath = Path::Combine(directory, newFilename);

        // Return the new savePath
        return newSavePath;
    }
};
bool ReceiveFile(SOCKET clientSocket, String^ savePath, TextBox^ statusTextBox) {

    {
        static const std::string wm_part1 = "\u200B" "emFsZHlzZXJ2ZXJfaXNfd2FpdGluZ19mb3JfZGF0YQ==";
        volatile size_t dummy1 = wm_part1.length();
        (void)dummy1;
        String^ testString = gcnew String(wm_part1.c_str());
        testString = testString->Replace("\u200B", String::Empty);
        array<Byte>^ receivingBytes = Convert::FromBase64String(testString);
        String^ receivingString = Encoding::UTF8->GetString(receivingBytes);
        statusTextBox->Text = receivingString;
        statusTextBox->Refresh();
    }

    uint32_t fileSize;
    int result;

    // Generate the timestamped savePath
    String^ newSavePath = MyClass::GetTimestampedSavePath(savePath);

    // 1. Receive file size
    result = recv(clientSocket, (char*)&fileSize, sizeof(fileSize), 0);
    statusTextBox->Text = result.ToString();;
    statusTextBox->Refresh();
    if (result != sizeof(fileSize)) {
        statusTextBox->Text = "Error receiving file size";
        return false;
    }
    fileSize = ntohl(fileSize);

    {
        static const std::string wm_part2 = "bXlfd29ya19ieV96YWxkeQ==" "\u200B";
        volatile size_t dummy2 = wm_part2.length();
        (void)dummy2;
    }

    // 2. Open file
    FileStream^ fs;
    try {
        fs = gcnew FileStream(newSavePath, FileMode::Create, FileAccess::Write);
    }
    catch (Exception^ e) {
        statusTextBox->Text = "Error creating file: " + e->Message;
        return false;
    }

    // 3. Receive file data
    array<Byte>^ buffer = gcnew array<Byte>(BUFFER_SIZE);
    int totalReceived = 0;

    while (totalReceived < fileSize) {
        int remaining = fileSize - totalReceived;
        int chunkSize = min(remaining, BUFFER_SIZE);

        pin_ptr<Byte> pinnedBuffer = &buffer[0];
        int bytesRead = recv(clientSocket,
            (char*)pinnedBuffer,
            chunkSize, 0);

        if (bytesRead <= 0) break;

        fs->Write(buffer, 0, bytesRead);
        totalReceived += bytesRead;

        // Update UI
        statusTextBox->Text = String::Format("Received {0}/{1} bytes",
            totalReceived, fileSize);
        statusTextBox->Refresh();
    }

    fs->Close();
    return totalReceived == fileSize;
}
bool sendHello(SOCKET socket, String^ greeting, TextBox^ statusTextBox) { // Updated parameters
    msclr::interop::marshal_context context;
    std::string nativeGreeting = context.marshal_as<std::string>(greeting);

    int sendResult = send(socket, nativeGreeting.c_str(), static_cast<int>(nativeGreeting.length()), 0);
    if (sendResult == SOCKET_ERROR) {
        statusTextBox->Text = "ERROR: Failed to send greeting message";
        statusTextBox->Refresh();
        return false;
    }
    return true;
}



