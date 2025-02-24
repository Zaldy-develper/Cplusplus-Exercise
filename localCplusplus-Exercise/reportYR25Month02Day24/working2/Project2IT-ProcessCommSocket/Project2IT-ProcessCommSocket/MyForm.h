#pragma once
#include "FileTransfer.h"
#include <iostream>
#include <cstring>
#include <winsock2.h>
#include <ws2tcpip.h>
#pragma comment(lib, "Ws2_32.lib")
#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>


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
			this->MaximizeBox = false;
			this->MinimizeBox = false;
			this->Name = L"MyForm";
			this->StartPosition = System::Windows::Forms::FormStartPosition::CenterScreen;
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
		
		// Check if all fields have been filled and set focus on the first empty one
		if (String::IsNullOrWhiteSpace(textboxIp->Text)) {
			ReceiveStatus("Please fill the IP address field.");
			textboxIp->Focus();
			return;
		}

		// Validate the IP address
		std::string textboxIpText = marshal_as<std::string>(textboxIp->Text);
		if (!IsValidIPv4Address(textboxIpText)) {
			ReceiveStatus("Incorrect IP address has been entered.");
			textboxIp->Focus();
			return;
		}

		if (String::IsNullOrWhiteSpace(textboxPort->Text)) {
			ReceiveStatus("Please fill the Port number field.");
			textboxPort->Focus();
			return;
		}

		if (String::IsNullOrWhiteSpace(textboxFilepath->Text)) {
			ReceiveStatus("Please fill the Save File Path field");
			textboxFilepath->Focus();
			return;
		}

		if (!ValidateFilePath(textboxFilepath->Text)) {
			ReceiveStatus("You type an invalid save file path.");
			textboxFilepath->Focus();
			return;
		}
			

		// All fields are filled, proceed on next logic from here

		// Check for invalid port number
		int port;

		if (!Int32::TryParse(textboxPort->Text, port) || port < 1024 || port > 65535) {
			ReceiveStatus("Invalid port number. Allowable port (1024-65535)");
			textboxPort->Clear();
			textboxPort->Focus();
			return;
		}

		array<String^>^ params = gcnew array<String^>{
			"Ip address: " + textboxIp->Text,
			"Port Number: " + textboxPort->Text,
			"Save File Path: " + textboxFilepath->Text
		};
		ReceiveStatus("Success fill-up on all fields");

		// Disable the Startreceive button
		StartCancelActive(false);

		String^ message = String::Join(Environment::NewLine, params);
		MessageBox::Show(message, "Receive Settings", MessageBoxButtons::OK, MessageBoxIcon::Information);


		WSADATA wsaData;
		if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
			MessageBox::Show("WSAStartup failed");
			return;
		}


		// �\�P�b�g�t�@�C���f�B�X�N���v�^�̍쐬
		if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == 0) {
			perror("socket failed");
			exit(EXIT_FAILURE);
		}
		else MessageBox::Show("�\�P�b�g�t�@�C���f�B�X�N���v�^�̍쐬���܂����B", "�X�V���", MessageBoxButtons::OK, MessageBoxIcon::Information);

		// �T�[�o�[�A�h���X�̐ݒ�
		address.sin_family = AF_INET;
		address.sin_addr.s_addr = INADDR_ANY;
		address.sin_port = htons(port);

		MessageBox::Show("�T�[�o�[�A�h���X�̐ݒ�B", "�X�V���", MessageBoxButtons::OK, MessageBoxIcon::Information);

		// �\�P�b�g���|�[�g�Ƀo�C���h����
		if (bind(server_fd, (struct sockaddr*)&address, sizeof(address)) < 0) {
			ReceiveStatus("ERROR: Binding failed");
		    perror("bind failed");
			closesocket(server_fd);
			exit(EXIT_FAILURE);
			StartCancelActive(true);
			return;
		}
		else ReceiveStatus("Binding success");

		// �ڑ������b�X������
		// 3 : The backlog parameter specifies the maximum number 
		//of pending connections that can be queued up before the server starts refusing new incoming connection requests.
		if (listen(server_fd, 3) < 0) {
			ReceiveStatus("ERROR:�ڑ������b�X�����܂���B");
			perror("listen");
			closesocket(server_fd);
			exit(EXIT_FAILURE);
			StartCancelActive(true);
		}
		else ReceiveStatus("�ڑ������b�X�����܂����B");

		//Waiting for connection
		ReceiveStatus("Waiting to connect...");

		// �N���C�A���g����̐ڑ����󂯓����
		if ((new_socket = accept(server_fd, (struct sockaddr*)&address, (socklen_t*)&addrlen)) < 0) {
			//MessageBox::Show("ERROR:�N���C�A���g����̐ڑ����󂯓���܂���B", "�X�V���", MessageBoxButtons::OK, MessageBoxIcon::Information);
			ReceiveStatus("ERROR:�N���C�A���g����̐ڑ����󂯓���܂���B");
			StatusUpdate("Error:Cannot connect from Client achieved.");
			perror("accept");
			closesocket(server_fd);
			exit(EXIT_FAILURE);
			// Re-enable the Startreceive button if needed
			btnStartreceive->Enabled = true;
		}
		else {
			//ReceiveStatus("�N���C�A���g����̐ڑ����󂯓���܂����B");
			//StatusUpdate("Connection from Client achieved.");
			// Test to send message from Server to Client
			if (!testingMessage()) {
				CloseSocketsAndCleanup(new_socket, server_fd);
				StartCancelActive(true);
				return;
			}
			// END -- Test to send message from Server to Client

			ReceiveStatus("Waiting to receive�c");
			StatusUpdate("Waiting to receive�c");
		}

		/* ======= MODIFIED SECTION: File Transfer Logic ======= */
		// Receive the file from the client
		// Pending: connection timeout, file size limit, etc.
		bool success = ReceiveFile(new_socket, textboxFilepath->Text, textboxStatus);
		/* ======= End of File Transfer Logic ======= */

		// �\�P�b�g����� and clean up Winsock
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

	// Textbox Status and Update
	void ReceiveStatus(String^ status) {
		textboxStatus->Text = status;
		textboxStatus->Refresh(); // Force the UI to update immediately
	}

	// Function to close sockets and clean up WinSock
	void CloseSocketsAndCleanup(int new_socket, int server_fd) {
		closesocket(new_socket);
		closesocket(server_fd);
		WSACleanup();
	}

	// Ipv4 Address Validation
	bool IsValidIPv4Address(const std::string& ipAddress) {
		sockaddr_in sa;
		// inet_pton returns 1 on success.
		return inet_pton(AF_INET, ipAddress.c_str(), &(sa.sin_addr)) == 1;
	}

	bool ValidateFilePath(System::String^ path)
	{
		if (System::String::IsNullOrWhiteSpace(path))
			return false;

		if (!IsValidPath(path))
			return false;

		if (!HasValidStructure(path))
			return false;

		if (!DoesDirectoryExist(path))
			return false;

		if (!CanWriteToDirectory(path))
			return false;

		return true;
	}

	bool IsValidPath(System::String^ path)
	{
		array<System::Char>^ invalidChars = System::IO::Path::GetInvalidPathChars();
		return path->IndexOfAny(invalidChars) == -1;
	}

	bool HasValidStructure(System::String^ path)
	{
		return System::IO::Path::IsPathRooted(path);
	}

	bool DoesDirectoryExist(System::String^ path)
	{
		System::String^ directory = System::IO::Path::GetDirectoryName(path);
		return System::IO::Directory::Exists(directory);
	}

	bool CanWriteToDirectory(System::String^ path)
	{
		try
		{
			System::String^ directory = System::IO::Path::GetDirectoryName(path);
			// Attempt to get a list of files as a write permission check
			array<System::String^>^ files = System::IO::Directory::GetFiles(directory);
			return true;
		}
		catch (System::UnauthorizedAccessException^)
		{
			return false;
		}
		catch (System::Exception^)
		{
			return false;
		}
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
			ReceiveStatus("ERROR: Failed to send greeting message");
			closesocket(new_socket);
			closesocket(server_fd);
			WSACleanup();
			return false;
		}
		else ReceiveStatus("Greeting sent: " + gcnew System::String(greeting.c_str()));

		return true;
		/* Testing End*/
	}
	
	


};
}