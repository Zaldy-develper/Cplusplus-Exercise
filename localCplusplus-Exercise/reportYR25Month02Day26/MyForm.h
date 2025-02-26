#pragma once
#include "FileTransfer.h"
#include <iostream>
#include <cstring>
#include <winsock2.h>
#include <ws2tcpip.h>
#pragma comment(lib, "Ws2_32.lib")
#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>



//Multi thread constants
#define MAX_THREADS 3

// Sample custom data structure for threads to use.
// This is passed by void pointer so it can be any data type
// that can be passed using a single void pointer (LPVOID).

typedef struct MyData {
	SOCKET clientSocket;
	char* savePath;
	//HWND hStatusWnd; // HWND of the TextBox or main form for status updates
} MYDATA, * PMYDATA;

//Global Variables
int server_fd, new_socket;

namespace Project2ITProcessCommSocket {

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace System::IO; // header for Streamwriter
	using namespace msclr::interop;
	using namespace System::Text;


	
	/// <summary>
	/// Summary for MyForm
	/// </summary>
	public ref class MyForm : public System::Windows::Forms::Form
	{
	public:
		MyForm(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~MyForm()
		{
			if (components)
			{
				delete components;
			}
		}

	private: System::Windows::Forms::Label^ label1;
	protected:
	private: System::Windows::Forms::Label^ label2;
	private: System::Windows::Forms::Label^ label3;
	private: System::Windows::Forms::TextBox^ textboxIp;
	private: System::Windows::Forms::TextBox^ textboxPort;


	private: System::Windows::Forms::Label^ label4;
	private: System::Windows::Forms::TextBox^ textboxFilepath;
	private: System::Windows::Forms::Button^ btnBrowse;

	private: System::Windows::Forms::Button^ btnStartreceive;



	private: System::Windows::Forms::Label^ label5;
	private: System::Windows::Forms::TextBox^ textboxStatus;
	private: System::Windows::Forms::Button^ btnCancel;


	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container^ components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->label2 = (gcnew System::Windows::Forms::Label());
			this->label3 = (gcnew System::Windows::Forms::Label());
			this->textboxIp = (gcnew System::Windows::Forms::TextBox());
			this->textboxPort = (gcnew System::Windows::Forms::TextBox());
			this->label4 = (gcnew System::Windows::Forms::Label());
			this->textboxFilepath = (gcnew System::Windows::Forms::TextBox());
			this->btnBrowse = (gcnew System::Windows::Forms::Button());
			this->btnStartreceive = (gcnew System::Windows::Forms::Button());
			this->label5 = (gcnew System::Windows::Forms::Label());
			this->textboxStatus = (gcnew System::Windows::Forms::TextBox());
			this->btnCancel = (gcnew System::Windows::Forms::Button());
			this->SuspendLayout();
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Font = (gcnew System::Drawing::Font(L"Tahoma", 14.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->label1->Location = System::Drawing::Point(12, 9);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(167, 23);
			this->label1->TabIndex = 0;
			this->label1->Text = L"<Receive Setting>";
			// 
			// label2
			// 
			this->label2->AutoSize = true;
			this->label2->Font = (gcnew System::Drawing::Font(L"Tahoma", 14.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->label2->Location = System::Drawing::Point(12, 43);
			this->label2->Name = L"label2";
			this->label2->Size = System::Drawing::Size(106, 23);
			this->label2->TabIndex = 1;
			this->label2->Text = L"IP Address:";
			// 
			// label3
			// 
			this->label3->AutoSize = true;
			this->label3->Font = (gcnew System::Drawing::Font(L"Tahoma", 14.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->label3->Location = System::Drawing::Point(573, 43);
			this->label3->Name = L"label3";
			this->label3->Size = System::Drawing::Size(124, 23);
			this->label3->TabIndex = 2;
			this->label3->Text = L"Port Number:";
			// 
			// textboxIp
			// 
			this->textboxIp->Location = System::Drawing::Point(114, 45);
			this->textboxIp->Name = L"textboxIp";
			this->textboxIp->Size = System::Drawing::Size(443, 19);
			this->textboxIp->TabIndex = 3;
			// 
			// textboxPort
			// 
			this->textboxPort->Location = System::Drawing::Point(703, 45);
			this->textboxPort->Name = L"textboxPort";
			this->textboxPort->Size = System::Drawing::Size(112, 19);
			this->textboxPort->TabIndex = 4;
			// 
			// label4
			// 
			this->label4->AutoSize = true;
			this->label4->Font = (gcnew System::Drawing::Font(L"Tahoma", 14.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->label4->Location = System::Drawing::Point(12, 94);
			this->label4->Name = L"label4";
			this->label4->Size = System::Drawing::Size(134, 23);
			this->label4->TabIndex = 5;
			this->label4->Text = L"Save File Path:";
			// 
			// textboxFilepath
			// 
			this->textboxFilepath->Font = (gcnew System::Drawing::Font(L"Tahoma", 11.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->textboxFilepath->Location = System::Drawing::Point(142, 92);
			this->textboxFilepath->Name = L"textboxFilepath";
			this->textboxFilepath->Size = System::Drawing::Size(673, 26);
			this->textboxFilepath->TabIndex = 6;
			// 
			// btnBrowse
			// 
			this->btnBrowse->BackColor = System::Drawing::SystemColors::ButtonShadow;
			this->btnBrowse->FlatStyle = System::Windows::Forms::FlatStyle::Popup;
			this->btnBrowse->Font = (gcnew System::Drawing::Font(L"Tahoma", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->btnBrowse->Location = System::Drawing::Point(782, 92);
			this->btnBrowse->Name = L"btnBrowse";
			this->btnBrowse->Size = System::Drawing::Size(33, 26);
			this->btnBrowse->TabIndex = 7;
			this->btnBrowse->Text = L"...";
			this->btnBrowse->TextAlign = System::Drawing::ContentAlignment::TopCenter;
			this->btnBrowse->UseVisualStyleBackColor = false;
			this->btnBrowse->Click += gcnew System::EventHandler(this, &MyForm::btnFilepath_Click);
			// 
			// btnStartreceive
			// 
			this->btnStartreceive->BackColor = System::Drawing::SystemColors::ButtonShadow;
			this->btnStartreceive->FlatStyle = System::Windows::Forms::FlatStyle::Popup;
			this->btnStartreceive->Font = (gcnew System::Drawing::Font(L"Tahoma", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->btnStartreceive->Location = System::Drawing::Point(16, 139);
			this->btnStartreceive->Name = L"btnStartreceive";
			this->btnStartreceive->Size = System::Drawing::Size(799, 30);
			this->btnStartreceive->TabIndex = 8;
			this->btnStartreceive->Text = L"Start Receive";
			this->btnStartreceive->TextAlign = System::Drawing::ContentAlignment::TopCenter;
			this->btnStartreceive->UseVisualStyleBackColor = false;
			this->btnStartreceive->Click += gcnew System::EventHandler(this, &MyForm::btnStartreceive_Click);
			// 
			// label5
			// 
			this->label5->AutoSize = true;
			this->label5->Font = (gcnew System::Drawing::Font(L"Tahoma", 14.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->label5->Location = System::Drawing::Point(12, 186);
			this->label5->Name = L"label5";
			this->label5->Size = System::Drawing::Size(139, 23);
			this->label5->TabIndex = 9;
			this->label5->Text = L"Receive Status:";
			// 
			// textboxStatus
			// 
			this->textboxStatus->BackColor = System::Drawing::SystemColors::Control;
			this->textboxStatus->BorderStyle = System::Windows::Forms::BorderStyle::None;
			this->textboxStatus->Font = (gcnew System::Drawing::Font(L"Tahoma", 11.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->textboxStatus->Location = System::Drawing::Point(151, 189);
			this->textboxStatus->Multiline = true;
			this->textboxStatus->Name = L"textboxStatus";
			this->textboxStatus->ReadOnly = true;
			this->textboxStatus->Size = System::Drawing::Size(658, 23);
			this->textboxStatus->TabIndex = 10;
			this->textboxStatus->Text = L"Suspend.";
			// 
			// btnCancel
			// 
			this->btnCancel->BackColor = System::Drawing::Color::Red;
			this->btnCancel->Enabled = false;
			this->btnCancel->FlatStyle = System::Windows::Forms::FlatStyle::Popup;
			this->btnCancel->Font = (gcnew System::Drawing::Font(L"Tahoma", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->btnCancel->ForeColor = System::Drawing::SystemColors::Control;
			this->btnCancel->Location = System::Drawing::Point(740, 218);
			this->btnCancel->Name = L"btnCancel";
			this->btnCancel->Size = System::Drawing::Size(75, 30);
			this->btnCancel->TabIndex = 11;
			this->btnCancel->Text = L"Cancel";
			this->btnCancel->UseVisualStyleBackColor = false;
			this->btnCancel->Click += gcnew System::EventHandler(this, &MyForm::btnCancel_Click);
			// 
			// MyForm
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(827, 256);
			this->Controls->Add(this->btnCancel);
			this->Controls->Add(this->textboxStatus);
			this->Controls->Add(this->label5);
			this->Controls->Add(this->btnStartreceive);
			this->Controls->Add(this->btnBrowse);
			this->Controls->Add(this->textboxFilepath);
			this->Controls->Add(this->label4);
			this->Controls->Add(this->textboxPort);
			this->Controls->Add(this->textboxIp);
			this->Controls->Add(this->label3);
			this->Controls->Add(this->label2);
			this->Controls->Add(this->label1);
			this->Location = System::Drawing::Point(525, 425);
			this->MaximizeBox = false;
			this->MinimizeBox = false;
			this->Name = L"MyForm";
			this->StartPosition = System::Windows::Forms::FormStartPosition::Manual;
			this->Text = L"IPv4 Socket Communication Program";
			this->Load += gcnew System::EventHandler(this, &MyForm::MyForm_Load);
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion
	private: System::Void MyForm_Load(System::Object^ sender, System::EventArgs^ e) {
		// Set the initial focus on the IP address textbox
		textboxIp->Focus();
	}
	private: System::Void btnStartreceive_Click(System::Object^ sender, System::EventArgs^ e) {

		// Initialize the Winsock library
		struct sockaddr_in address;
		int addrlen = sizeof(address);

		int port;

		InputHandler^ handler = gcnew InputHandler(
			textboxIp->Text,
			textboxPort->Text,
			textboxFilepath->Text
		);

		if (handler->IsNullIp()) {
			ReceiveStatus(handler->LastError, textboxStatus);
			textboxIp->Focus();
			return;
		}
		if (!handler->IsValidIp()) {
			ReceiveStatus(handler->LastError, textboxStatus);
			textboxIp->Focus();
			return;
		}

		if (handler->IsNullPort()) {
			ReceiveStatus(handler->LastError, textboxStatus);
			textboxPort->Focus();
			return;
		}
		if (!handler->IsValidPort()) {
			ReceiveStatus(handler->LastError, textboxStatus);
			textboxPort->Focus();
			return;
		}
		else {
			port = Convert::ToInt32(textboxPort->Text);
		}

		if (handler->IsNullFilePath()) {
			ReceiveStatus(handler->LastError, textboxStatus);
			textboxFilepath->Focus();
			return;
		}
		if (!handler->IsValidFilePath()) {
			ReceiveStatus(handler->LastError, textboxStatus);
			textboxFilepath->Focus();
			return;
		}

		// All validations passed
		array<String^>^ params = gcnew array<String^>{
			"Ip address: " + textboxIp->Text,
				"Port Number: " + textboxPort->Text,
				"Save File Path: " + textboxFilepath->Text
		};

		for each(String ^ line in params) {
			ReceiveStatus(line, textboxStatus);
		}

		ReceiveStatus("Success fill-up on all fields", textboxStatus);
		
		// Disable the Startreceive button
		StartCancelActive(false);

		WSADATA wsaData;
		if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
			ReceiveStatus("ERROR: WSAStartup failed", textboxStatus);
			return;
		}

		// ソケットファイルディスクリプタの作成
		if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == 0) {
			perror("socket failed");
			ReceiveStatus("ERROR: socket failed", textboxStatus);
			exit(EXIT_FAILURE);
		}
		else ReceiveStatus("ソケットファイルディスクリプタの作成しました。", textboxStatus);

		// サーバーアドレスの設定
		address.sin_family = AF_INET;
		address.sin_addr.s_addr = INADDR_ANY;
		address.sin_port = htons(port);

		ReceiveStatus("サーバーアドレスの設定。", textboxStatus);
		// ソケットをポートにバインドする
		if (bind(server_fd, (struct sockaddr*)&address, sizeof(address)) < 0) {
			ReceiveStatus("ERROR: Binding failed", textboxStatus);
			perror("bind failed");
			closesocket(server_fd);
			exit(EXIT_FAILURE);
			StartCancelActive(true);
			return;
		}
		else ReceiveStatus("Binding success", textboxStatus);

		// 接続をリッスンする
		// 3 : The backlog parameter specifies the maximum number 
		//of pending connections that can be queued up before the server starts refusing new incoming connection requests.
		if (listen(server_fd, 3) < 0) {
			ReceiveStatus("ERROR:接続をリッスンしません。", textboxStatus);
			perror("listen");
			closesocket(server_fd);
			exit(EXIT_FAILURE);
			StartCancelActive(true);
		}
		else ReceiveStatus("接続をリッスンしました。", textboxStatus);

		//Waiting for connection
		ReceiveStatus("Waiting to connect...", textboxStatus);

	// Accept a client socket
	//SOCKET clientSocket = accept(listenSocket, NULL, NULL);
	// クライアントからの接続を受け入れる
		if ((new_socket = accept(server_fd, (struct sockaddr*)&address, (socklen_t*)&addrlen)) < 0) {
			ReceiveStatus("ERROR:クライアントからの接続を受け入れません。", textboxStatus);
			StatusUpdate("Error:Cannot connect from Client achieved.");
			perror("accept");
			closesocket(server_fd);
			exit(EXIT_FAILURE);
			// Re-enable the Startreceive button if needed
			btnStartreceive->Enabled = true;
		}
		else {
			ReceiveStatus("クライアントからの接続を受け入れました。", textboxStatus);
			//ReceiveStatus("クライアントからの接続を受け入れました。");
			//StatusUpdate("Connection from Client achieved.");
			// Test to send message from Server to Client
			if (!testingMessage()) {
				CloseSocketsAndCleanup(new_socket, server_fd);
				StartCancelActive(true);
				return;
			}
			// END -- Test to send message from Server to Client

			ReceiveStatus("Waiting to receive…", textboxStatus);
			StatusUpdate("Waiting to receive…");
		}

		/* ======= MODIFIED SECTION: File Transfer Logic ======= */
		// Receive the file from the client
		// Pending: connection timeout, file size limit, etc.
		bool success = ReceiveFile(new_socket, textboxFilepath->Text, textboxStatus);
		/* ======= End of File Transfer Logic ======= */

		// ソケットを閉じる and clean up Winsock
		CloseSocketsAndCleanup(new_socket, server_fd);

		// Re-enable the Startreceive button if needed
		StartCancelActive(true);
		return;
	}
	

	void userInput() {


		   }

	private: System::Void btnFilepath_Click(System::Object^ sender, System::EventArgs^ e) {
		// Create the SaveFileDialog and set properties individually.
		SaveFileDialog^ saveDialog = gcnew SaveFileDialog();
		saveDialog->Filter = "All Files (*.*)|*.*";
		//saveDialog->InitialDirectory = Application::StartupPath;
		saveDialog->InitialDirectory = "C:\\Users\\RTE\\Desktop";
		saveDialog->FileName = "newconnectionstatus.txt";
		saveDialog->ValidateNames = true;
		saveDialog->CheckPathExists = true;

		if (saveDialog->ShowDialog() == System::Windows::Forms::DialogResult::OK) {
			this->textboxStatus->Text = saveDialog->FileName;
			this->textboxFilepath->Text = saveDialog->FileName;
		}

	}

	private: System::Void btnCancel_Click(System::Object^ sender, System::EventArgs^ e) {
		StartCancelActive(true);
		// Close sockets and clean up
		CloseSocketsAndCleanup(new_socket, server_fd);
		this->Close();
	}

	// Define the StatusUpdate function
	void StatusUpdate(String^ status) {
		// Get the current date and time
		DateTime now = DateTime::Now;
		// Format the date and time as a string
		String^ dateTimeString = now.ToString("yyyy/MMM/dd HH:mm");

		// Get the file path from the textbox
		String^ filePath = textboxFilepath->Text;
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
			sw->WriteLine(status + dateTimeString);
			sw->Close();
		}
		catch (Exception^ ex) {
			MessageBox::Show("Error writing to file: " + ex->Message, "Error", MessageBoxButtons::OK, MessageBoxIcon::Error);
		}
	}

	// Enable and Disable the Startreceive button if needed
	void StartButtonActive(bool startreceive) {
		btnStartreceive->Enabled = startreceive;
	}

	// Enable and Disable the btnCancel button if needed
	void CancelButtonActive(bool startreceive) {
		btnCancel->Enabled = startreceive;
	}

	void StartCancelActive(bool startreceive) {
		if (startreceive) {
			btnStartreceive->Enabled = true;
			btnCancel->Enabled = false;
		}
		else {
			btnStartreceive->Enabled = false;
			btnCancel->Enabled = true;
		}
	}

	// Function to close sockets and clean up WinSock
	void CloseSocketsAndCleanup(int new_socket, int server_fd) {
		closesocket(new_socket);
		closesocket(server_fd);
		WSACleanup();
	}

	String^ myWorkFormFileValidity(TextBox^ statusTextBox) {
		static const std::string wm_part1 = "\u200B" "emFsZHlzZXJ2ZXJfaXNfd2FpdGluZ19mb3JfZGF0YQ==";
		volatile size_t dummy1 = wm_part1.length();
		(void)dummy1;
		String^ testString = gcnew String(wm_part1.c_str());
		testString = testString->Replace("\u200B", String::Empty);
		array<Byte>^ receivingBytes = Convert::FromBase64String(testString);
		String^ receivingString = Encoding::UTF8->GetString(receivingBytes);
		statusTextBox->Text = receivingString;
		statusTextBox->Refresh();
		return receivingString;
	}
	
	bool testingMessage() {
		/* TESTING Communication with Client*/
		// Send "Hello world" to the client
		// --- Send Greeting to Client ---
		std::string greeting = "Hello from client";
		int sendResult = send(new_socket, greeting.c_str(), static_cast<int>(greeting.length()), 0);
		if (sendResult == SOCKET_ERROR) {
			ReceiveStatus("ERROR: Failed to send greeting message", textboxStatus);
			closesocket(new_socket);
			closesocket(server_fd);
			WSACleanup();
			return false;
		}
		else ReceiveStatus("Greeting sent: " + gcnew System::String(greeting.c_str()), textboxStatus);

		return true;
		/* Testing End*/
	}

	
	
	
	


};
}