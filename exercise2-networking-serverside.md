#pragma once
#include <iostream>
#include <cstring>
#include <winsock2.h>
#include <ws2tcpip.h>
#pragma comment(lib, "Ws2_32.lib")
#include <msclr/marshal.h>

#define BUFFER_SIZE 1024

namespace Project2ITProcessCommSocket {

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace System::IO; // header for Streamwriter


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


	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container ^components;

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
			// MyForm
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(827, 220);
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
	}
	private: System::Void btnStartreceive_Click(System::Object^ sender, System::EventArgs^ e) {

		
		int server_fd, new_socket;
		struct sockaddr_in address;
		int addrlen = sizeof(address);
		char buffer[BUFFER_SIZE + 1] = { 0 };

		if (String::IsNullOrWhiteSpace(textboxIp->Text) ||
			String::IsNullOrWhiteSpace(textboxPort->Text) ||
			String::IsNullOrWhiteSpace(textboxFilepath->Text)) {
			MessageBox::Show("Please fill all fields");
			return;
		}

		int port;

		if (!Int32::TryParse(textboxPort->Text, port) || port < 1024 || port > 65535) {
			MessageBox::Show("Invalid port number (1024-65535)");
			textboxPort->Clear();
			return;
		}
		else MessageBox::Show("You have entered the correct data type for the port number\n"
			"Working in Progress: Accepting input from the Path File Textbox.");

		
		array<String^>^ params = gcnew array<String^>{ 
			textboxIp->Text,
			textboxPort->Text,
			textboxFilepath->Text
		};

		MessageBox::Show("Success fill-up on all fields");

		// Disable the Startreceive button
		StartButtonActive(false);

		String^ message = String::Join(Environment::NewLine, params);
		MessageBox::Show(message, "Receive Settings", MessageBoxButtons::OK, MessageBoxIcon::Information);

		
		WSADATA wsaData;
		if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
			MessageBox::Show("WSAStartup failed");
			return;
		}
		
		
		// ソケットファイルディスクリプタの作成
		if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == 0) {
			perror("socket failed");
			exit(EXIT_FAILURE);
		}
		else MessageBox::Show("ソケットファイルディスクリプタの作成しました。", "更新情報", MessageBoxButtons::OK, MessageBoxIcon::Information);

		// サーバーアドレスの設定
		address.sin_family = AF_INET;
		address.sin_addr.s_addr = INADDR_ANY;
		address.sin_port = htons(port);
		MessageBox::Show("サーバーアドレスの設定。", "更新情報", MessageBoxButtons::OK, MessageBoxIcon::Information);

		// ソケットをポートにバインドする
		if (bind(server_fd, (struct sockaddr*)&address, sizeof(address)) < 0) {
			ReceiveStatus("ERROR: Binding failed");
			// MessageBox::Show("ERROR:binding failed", "更新情報", MessageBoxButtons::OK, MessageBoxIcon::Information);
			perror("bind failed");
			closesocket(server_fd);
			exit(EXIT_FAILURE);
		}
		else ReceiveStatus("Binding success");

		// 接続をリッスンする
		// 3 : The backlog parameter specifies the maximum number 
		//of pending connections that can be queued up before the server starts refusing new incoming connection requests.
		if (listen(server_fd, 3) < 0) { 
			//MessageBox::Show("ERROR:接続をリッスンしません。", "更新情報", MessageBoxButtons::OK, MessageBoxIcon::Information);
			ReceiveStatus("ERROR:接続をリッスンしません。");
			perror("listen");
			closesocket(server_fd);
			exit(EXIT_FAILURE);
		}
		else ReceiveStatus("接続をリッスンしました。");

		//Waiting for connection
		ReceiveStatus("Waiting to receive...");

		// クライアントからの接続を受け入れる
		if ((new_socket = accept(server_fd, (struct sockaddr*)&address, (socklen_t*)&addrlen)) < 0) {
			//MessageBox::Show("ERROR:クライアントからの接続を受け入れません。", "更新情報", MessageBoxButtons::OK, MessageBoxIcon::Information);
			ReceiveStatus("ERROR:クライアントからの接続を受け入れません。");
			StatusUpdate("Error:Cannot connect from Client achieved.");
			perror("accept");
			closesocket(server_fd);
			exit(EXIT_FAILURE);
		}
		else {
			ReceiveStatus("クライアントからの接続を受け入れました。");
			StatusUpdate("Connection from Client achieved.");
		}
			

		/*
		// データを受信する (Received the data)
		int bytesRead = recv(new_socket, buffer, BUFFER_SIZE, 0);
		if (bytesRead == SOCKET_ERROR) {
			//std::cerr << "recv failed" << std::endl;
			StatusUpdate("Received Failed.");
			ReceiveStatus("Received Failed.");
			closesocket(new_socket);
			WSACleanup();
			return;
		}
		else {
			buffer[bytesRead] = '\0'; // Null-terminate the received data
			// std::cout << "Server received: " << buffer << std::endl;
			//MessageBox::Show("Server received: ", "更新情報", MessageBoxButtons::OK, MessageBoxIcon::Information);

		}
		*/
		
		/* TESTING*/
		// Send "Hello world" to the client
		// --- Send Greeting to Client ---
		std::string greeting = "Hello world";
		int sendResult = send(new_socket, greeting.c_str(), static_cast<int>(greeting.length()), 0);
		if (sendResult == SOCKET_ERROR) {
			ReceiveStatus("ERROR: Failed to send greeting message");
			closesocket(new_socket);
			closesocket(server_fd);
			WSACleanup();
			return;
		}
		ReceiveStatus("Greeting sent: " + gcnew System::String(greeting.c_str()));

		/* Testing End*/

		/* ======= MODIFIED SECTION: File Transfer Logic ======= */
				/* ======= MODIFIED SECTION: File Transfer Logic ======= */
// STEP 1: Receive the file size header (4 bytes, network byte order)
		uint32_t netFileSize = 0;
		int result = recv(new_socket, reinterpret_cast<char*>(&netFileSize), sizeof(netFileSize), 0);
		if (result != sizeof(netFileSize)) {
			//MessageBox::Show("Step 1 Done", "更新情報", MessageBoxButtons::OK, MessageBoxIcon::Information);
			ReceiveStatus("ERROR: Failed to receive file size header");
			closesocket(new_socket);
			closesocket(server_fd);
			WSACleanup();
			// Re-enable the Startreceive button if needed
			btnStartreceive->Enabled = true;
			return;
		}
		uint32_t fileSize = ntohl(netFileSize);
		if (fileSize == 0) {
			ReceiveStatus("ERROR: Invalid file size (0 bytes)");
			closesocket(new_socket);
			closesocket(server_fd);
			WSACleanup();
			return;
		}
		

		ReceiveStatus("File size received: " + fileSize.ToString() + " bytes");
		

		// STEP 2: Open the file
		System::IO::FileStream^ fs = nullptr;
		try {
			fs = gcnew System::IO::FileStream(textboxFilepath->Text,
				System::IO::FileMode::Create,
				System::IO::FileAccess::Write);
		}
		catch (Exception^ ex) {
			ReceiveStatus("ERROR: Unable to open file: " + ex->Message);
			closesocket(new_socket);
			closesocket(server_fd);
			WSACleanup();
			return;
		}

		// STEP 3: Receive file data
		const int chunkSize = BUFFER_SIZE;
		uint32_t totalBytesReceived = 0;
		bool transferError = false;

		while (totalBytesReceived < fileSize && !transferError) {
			int bytesToRecv = min(chunkSize, static_cast<int>(fileSize - totalBytesReceived));

			array<Byte>^ managedBuffer = gcnew array<Byte>(bytesToRecv);
			pin_ptr<Byte> pinnedBuffer = &managedBuffer[0];

			int bytesRead = recv(new_socket, reinterpret_cast<char*>(pinnedBuffer), bytesToRecv, 0);
			if (bytesRead <= 0) {
				ReceiveStatus("ERROR: Connection closed prematurely");
				transferError = true;
				break;
			}

			try {
				fs->Write(managedBuffer, 0, bytesRead);
			}
			catch (Exception^ ex) {
				ReceiveStatus("ERROR: File write failed: " + ex->Message);
				transferError = true;
				break;
			}

			totalBytesReceived += bytesRead;
			ReceiveStatus("Progress: " + totalBytesReceived.ToString() + "/" + fileSize.ToString());
		}

		fs->Close();

		// Final validation
		if (totalBytesReceived != fileSize) {
			ReceiveStatus("ERROR: Incomplete transfer. Received " +
				totalBytesReceived.ToString() + "/" + fileSize.ToString());
			System::IO::File::Delete(textboxFilepath->Text);
		}
		else {
			ReceiveStatus("File received successfully!");
		}
		/* ======= End of File Transfer Logic ======= */

		// Send a confirmation response back to the client
		const char* response = "File received successfully";
		send(new_socket, response, strlen(response), 0);
		StatusUpdate("Response sent: File received successfully");

		/*
		
		// std::cout << "Server received: " << buffer << std::endl;

		// クライアントに応答を送信する
		const char* response = "Hello from server";
		send(new_socket, response, strlen(response), 0);
		std::cout << "Response sent\n";
		StatusUpdate("Response sent");
		*/

		// ソケットを閉じる and clean up Winsock
		closesocket(new_socket);
		closesocket(server_fd);
		WSACleanup();

		// Re-enable the Startreceive button if needed
		btnStartreceive->Enabled = true;
	}

	void userInput() {


	}
private: System::Void btnFilepath_Click(System::Object^ sender, System::EventArgs^ e) {
	// Create the SaveFileDialog and set properties individually.
	SaveFileDialog^ saveDialog = gcnew SaveFileDialog();
	saveDialog->Filter = "All Files (*.*)|*.*";
	saveDialog->InitialDirectory = Application::StartupPath;
	saveDialog->ValidateNames = true;
	saveDialog->CheckPathExists = true;

	if (saveDialog->ShowDialog() == System::Windows::Forms::DialogResult::OK) {
		this->textboxStatus->Text = saveDialog->FileName;
		this->textboxFilepath->Text = saveDialog->FileName;
	}

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

	   // Textbox Status and Update
	   void ReceiveStatus(String^ status) {
		   textboxStatus->Text = status;
		   textboxStatus->Refresh(); // Force the UI to update immediately
	   }
	   
};
}

