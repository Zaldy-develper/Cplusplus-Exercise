#pragma once
#include "ValidationFlow.h"
#include "FileTransfer.h"
#include "ClientSelectionForm.h"
#include <iostream>
#include <cstring>
#include <winsock2.h>
#include <ws2tcpip.h>
#pragma comment(lib, "Ws2_32.lib")
#include <msclr/marshal_cppstd.h>

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
	using namespace System::Threading;



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
	private: System::Windows::Forms::ToolTip^ toolTipIpOctet;
	private: System::Windows::Forms::MaskedTextBox^ maskedTextBoxIp;
	private: System::Windows::Forms::GroupBox^ groupBox1;
	private: System::Windows::Forms::RichTextBox^ chatLogRichText;
	private: System::Windows::Forms::Button^ btnSend;
	private: System::Windows::Forms::TextBox^ textboxServerMsg;




	protected:

	protected:

		// My Custom Instance
		// Declare ButtonHandler as a member of the MyForm class
	private: ButtonHandler^ buttonHandler = gcnew ButtonHandler();

	private: // Create the SocketHandler instance using the parameterless constructor.
			//so that it can be use by the Send button
			SocketHandler^ socketHandlerInstance = gcnew SocketHandler();

	private:
		void StartServerThread() {
			// Get the port (assumed to be already validated)
			int port = Convert::ToInt32(textboxPort->Text);
			// Create and start the server using the UI controls
			socketHandlerInstance = gcnew SocketHandler(port, maskedTextBoxIp->Text);
			socketHandlerInstance->StartServer(textboxStatus, textboxFilepath, chatLogRichText);
		}

	private: System::Windows::Forms::Label^ label1;
	protected:
	private: System::Windows::Forms::Label^ label2;
	private: System::Windows::Forms::Label^ label3;

	private: System::Windows::Forms::TextBox^ textboxPort;


	private: System::Windows::Forms::Label^ label4;
	private: System::Windows::Forms::TextBox^ textboxFilepath;
	private: System::Windows::Forms::Button^ btnBrowse;

	private: System::Windows::Forms::Button^ btnStartreceive;



	private: System::Windows::Forms::Label^ label5;
	private: System::Windows::Forms::TextBox^ textboxStatus;
	private: System::Windows::Forms::Button^ btnCancel;
	private: System::ComponentModel::IContainer^ components;


	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>


#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->label2 = (gcnew System::Windows::Forms::Label());
			this->label3 = (gcnew System::Windows::Forms::Label());
			this->textboxPort = (gcnew System::Windows::Forms::TextBox());
			this->label4 = (gcnew System::Windows::Forms::Label());
			this->textboxFilepath = (gcnew System::Windows::Forms::TextBox());
			this->btnBrowse = (gcnew System::Windows::Forms::Button());
			this->btnStartreceive = (gcnew System::Windows::Forms::Button());
			this->label5 = (gcnew System::Windows::Forms::Label());
			this->textboxStatus = (gcnew System::Windows::Forms::TextBox());
			this->btnCancel = (gcnew System::Windows::Forms::Button());
			this->toolTipIpOctet = (gcnew System::Windows::Forms::ToolTip(this->components));
			this->maskedTextBoxIp = (gcnew System::Windows::Forms::MaskedTextBox());
			this->groupBox1 = (gcnew System::Windows::Forms::GroupBox());
			this->chatLogRichText = (gcnew System::Windows::Forms::RichTextBox());
			this->btnSend = (gcnew System::Windows::Forms::Button());
			this->textboxServerMsg = (gcnew System::Windows::Forms::TextBox());
			this->SuspendLayout();
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Font = (gcnew System::Drawing::Font(L"Tahoma", 14.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->label1->Location = System::Drawing::Point(12, 9);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(199, 23);
			this->label1->TabIndex = 0;
			this->label1->Text = L"<Zaldy`s Chat Room>";
			// 
			// label2
			// 
			this->label2->AutoSize = true;
			this->label2->Font = (gcnew System::Drawing::Font(L"Tahoma", 15.75F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->label2->Location = System::Drawing::Point(12, 38);
			this->label2->Name = L"label2";
			this->label2->Size = System::Drawing::Size(120, 25);
			this->label2->TabIndex = 1;
			this->label2->Text = L"IP Address:";
			// 
			// label3
			// 
			this->label3->AutoSize = true;
			this->label3->Font = (gcnew System::Drawing::Font(L"Tahoma", 15.75F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->label3->Location = System::Drawing::Point(276, 37);
			this->label3->Name = L"label3";
			this->label3->Size = System::Drawing::Size(139, 25);
			this->label3->TabIndex = 2;
			this->label3->Text = L"Port Number:";
			// 
			// textboxPort
			// 
			this->textboxPort->Font = (gcnew System::Drawing::Font(L"Tahoma", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->textboxPort->Location = System::Drawing::Point(415, 37);
			this->textboxPort->Name = L"textboxPort";
			this->textboxPort->Size = System::Drawing::Size(51, 27);
			this->textboxPort->TabIndex = 4;
			this->textboxPort->Text = L"8080";
			this->textboxPort->TextAlign = System::Windows::Forms::HorizontalAlignment::Center;
			// 
			// label4
			// 
			this->label4->AutoSize = true;
			this->label4->Font = (gcnew System::Drawing::Font(L"Tahoma", 14.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->label4->Location = System::Drawing::Point(12, 76);
			this->label4->Name = L"label4";
			this->label4->Size = System::Drawing::Size(100, 23);
			this->label4->TabIndex = 5;
			this->label4->Text = L"Save Path:";
			// 
			// textboxFilepath
			// 
			this->textboxFilepath->Font = (gcnew System::Drawing::Font(L"Tahoma", 11.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->textboxFilepath->Location = System::Drawing::Point(129, 74);
			this->textboxFilepath->Name = L"textboxFilepath";
			this->textboxFilepath->Size = System::Drawing::Size(337, 26);
			this->textboxFilepath->TabIndex = 6;
			// 
			// btnBrowse
			// 
			this->btnBrowse->BackColor = System::Drawing::SystemColors::ButtonShadow;
			this->btnBrowse->FlatStyle = System::Windows::Forms::FlatStyle::Popup;
			this->btnBrowse->Font = (gcnew System::Drawing::Font(L"Tahoma", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->btnBrowse->Location = System::Drawing::Point(433, 74);
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
			this->btnStartreceive->Location = System::Drawing::Point(16, 110);
			this->btnStartreceive->Name = L"btnStartreceive";
			this->btnStartreceive->Size = System::Drawing::Size(96, 30);
			this->btnStartreceive->TabIndex = 8;
			this->btnStartreceive->Text = L"Start";
			this->btnStartreceive->TextAlign = System::Drawing::ContentAlignment::TopCenter;
			this->btnStartreceive->UseVisualStyleBackColor = false;
			this->btnStartreceive->Click += gcnew System::EventHandler(this, &MyForm::btnStartreceive_Click);
			// 
			// label5
			// 
			this->label5->AutoSize = true;
			this->label5->Font = (gcnew System::Drawing::Font(L"Tahoma", 14.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->label5->Location = System::Drawing::Point(524, 2);
			this->label5->Name = L"label5";
			this->label5->Size = System::Drawing::Size(145, 23);
			this->label5->TabIndex = 9;
			this->label5->Text = L"Message History";
			// 
			// textboxStatus
			// 
			this->textboxStatus->BackColor = System::Drawing::SystemColors::ControlLightLight;
			this->textboxStatus->BorderStyle = System::Windows::Forms::BorderStyle::None;
			this->textboxStatus->Font = (gcnew System::Drawing::Font(L"Tahoma", 9.75F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->textboxStatus->Location = System::Drawing::Point(135, 113);
			this->textboxStatus->Multiline = true;
			this->textboxStatus->Name = L"textboxStatus";
			this->textboxStatus->ReadOnly = true;
			this->textboxStatus->ScrollBars = System::Windows::Forms::ScrollBars::Vertical;
			this->textboxStatus->Size = System::Drawing::Size(326, 95);
			this->textboxStatus->TabIndex = 10;
			this->textboxStatus->Text = L"Suspend.";
			// 
			// btnCancel
			// 
			this->btnCancel->BackColor = System::Drawing::SystemColors::ButtonShadow;
			this->btnCancel->Enabled = false;
			this->btnCancel->FlatStyle = System::Windows::Forms::FlatStyle::Popup;
			this->btnCancel->Font = (gcnew System::Drawing::Font(L"Tahoma", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->btnCancel->ForeColor = System::Drawing::Color::Red;
			this->btnCancel->Location = System::Drawing::Point(17, 180);
			this->btnCancel->Name = L"btnCancel";
			this->btnCancel->Size = System::Drawing::Size(95, 28);
			this->btnCancel->TabIndex = 11;
			this->btnCancel->Text = L"🛑 STOP";
			this->btnCancel->UseVisualStyleBackColor = false;
			this->btnCancel->Click += gcnew System::EventHandler(this, &MyForm::btnCancel_Click);
			// 
			// maskedTextBoxIp
			// 
			this->maskedTextBoxIp->Font = (gcnew System::Drawing::Font(L"Tahoma", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->maskedTextBoxIp->Location = System::Drawing::Point(129, 37);
			this->maskedTextBoxIp->Mask = L"000.000.0.000";
			this->maskedTextBoxIp->Name = L"maskedTextBoxIp";
			this->maskedTextBoxIp->Size = System::Drawing::Size(144, 27);
			this->maskedTextBoxIp->TabIndex = 12;
			this->maskedTextBoxIp->TextAlign = System::Windows::Forms::HorizontalAlignment::Center;
			this->toolTipIpOctet->SetToolTip(this->maskedTextBoxIp, L"Please input on the range of 0-255 only");
			// 
			// groupBox1
			// 
			this->groupBox1->Location = System::Drawing::Point(129, 102);
			this->groupBox1->Name = L"groupBox1";
			this->groupBox1->Size = System::Drawing::Size(337, 110);
			this->groupBox1->TabIndex = 13;
			this->groupBox1->TabStop = false;
			this->groupBox1->Text = L"Status";
			// 
			// chatLogRichText
			// 
			this->chatLogRichText->BackColor = System::Drawing::Color::WhiteSmoke;
			this->chatLogRichText->Font = (gcnew System::Drawing::Font(L"Segoe UI", 9.75F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->chatLogRichText->Location = System::Drawing::Point(510, 23);
			this->chatLogRichText->Name = L"chatLogRichText";
			this->chatLogRichText->ReadOnly = true;
			this->chatLogRichText->Size = System::Drawing::Size(305, 225);
			this->chatLogRichText->TabIndex = 14;
			this->chatLogRichText->Text = L"";
			// 
			// btnSend
			// 
			this->btnSend->BackColor = System::Drawing::SystemColors::ButtonShadow;
			this->btnSend->FlatStyle = System::Windows::Forms::FlatStyle::Popup;
			this->btnSend->Font = (gcnew System::Drawing::Font(L"Tahoma", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->btnSend->Location = System::Drawing::Point(370, 214);
			this->btnSend->Name = L"btnSend";
			this->btnSend->Size = System::Drawing::Size(96, 30);
			this->btnSend->TabIndex = 15;
			this->btnSend->Text = L"Send";
			this->btnSend->TextAlign = System::Drawing::ContentAlignment::TopCenter;
			this->btnSend->UseVisualStyleBackColor = false;
			this->btnSend->Click += gcnew System::EventHandler(this, &MyForm::btnSend_Click);
			// 
			// textboxServerMsg
			// 
			this->textboxServerMsg->BackColor = System::Drawing::SystemColors::ControlLightLight;
			this->textboxServerMsg->BorderStyle = System::Windows::Forms::BorderStyle::None;
			this->textboxServerMsg->Font = (gcnew System::Drawing::Font(L"Tahoma", 14.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->textboxServerMsg->Location = System::Drawing::Point(16, 219);
			this->textboxServerMsg->Name = L"textboxServerMsg";
			this->textboxServerMsg->Size = System::Drawing::Size(348, 23);
			this->textboxServerMsg->TabIndex = 16;
			// 
			// MyForm
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(827, 256);
			this->Controls->Add(this->textboxServerMsg);
			this->Controls->Add(this->btnSend);
			this->Controls->Add(this->maskedTextBoxIp);
			this->Controls->Add(this->btnCancel);
			this->Controls->Add(this->textboxStatus);
			this->Controls->Add(this->label5);
			this->Controls->Add(this->btnStartreceive);
			this->Controls->Add(this->btnBrowse);
			this->Controls->Add(this->textboxFilepath);
			this->Controls->Add(this->label4);
			this->Controls->Add(this->textboxPort);
			this->Controls->Add(this->label3);
			this->Controls->Add(this->label2);
			this->Controls->Add(this->label1);
			this->Controls->Add(this->groupBox1);
			this->Controls->Add(this->chatLogRichText);
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
		// Load the Set-up Config Ini File
		LoadConfigData(maskedTextBoxIp, textboxPort, textboxFilepath, textboxStatus);
		// Set the initial focus on the IP address textbox
		ReceiveStatus("", textboxStatus);
		this->ActiveControl = maskedTextBoxIp;
	}
	private: System::Void btnStartreceive_Click(System::Object^ sender, System::EventArgs^ e) {

		// Disable the Startreceive button
		buttonHandler->StartCancelActive(btnStartreceive, btnCancel, false);
		ReceiveStatus("Performing Configuration Set-up.", textboxStatus);
		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
		/*==========================================================================*
		 *      Validation Flow of the Input Fields 
		 *						@Ip Address, Port, Save File Path					*
		 *==========================================================================*/
		std::string ip = msclr::interop::marshal_as<std::string>(maskedTextBoxIp->Text);
		std::string port = msclr::interop::marshal_as<std::string>(textboxPort->Text);
		std::string path = msclr::interop::marshal_as<std::string>(textboxFilepath->Text);

		InputHandlerNative input(ip, port, path);
		std::string errorMessage;

		// Create your validation blocks
		auto block1 = std::make_shared<IpNotEmptyBlock>();
		auto block2 = std::make_shared<ValidIpFormatBlock>();
		auto block3 = std::make_shared<PortNotEmptyBlock>();
		auto block4 = std::make_shared<PathNotEmptyBlock>();
		auto block5 = std::make_shared<PortIsValid>();
		auto block6 = std::make_shared<PathIsValid>();

		block1->addOnSuccess(block2);
		block2->addOnSuccess(block3);
		block3->addOnSuccess(block4);
		block4->addOnSuccess(block5);
		block5->addOnSuccess(block6);

		runFlow(block1, input, errorMessage); // Here we start to pass the Input and address of the string errorMessage

		if (!errorMessage.empty()) {
			// Report the error.
			// Set the focus on the last field that was filled
			FocusPosition cursorPosition = input.getLastPosFocus();

			switch (cursorPosition) {
			case FocusPosition::IP:
				buttonHandler->StartCancelActive(btnStartreceive, btnCancel, true);
				maskedTextBoxIp->Focus();
				break;
			case FocusPosition::PORT:
				buttonHandler->StartCancelActive(btnStartreceive, btnCancel, true);
				textboxPort->Focus();
				break;
			case FocusPosition::PATH:
				buttonHandler->StartCancelActive(btnStartreceive, btnCancel, true);
				textboxFilepath->Focus();
				break;
			case FocusPosition::DEFAULT:
				buttonHandler->StartCancelActive(btnStartreceive, btnCancel, true);
				maskedTextBoxIp->Focus();
				break;
			}
			ReceiveStatus(gcnew System::String(errorMessage.c_str()), textboxStatus);
			return;
		}

		// All validations passed

		array<String^>^ serverSetup = gcnew array<String^>{
			" ", // Create new line
			"Ip address: " + maskedTextBoxIp->Text,
			"Port Number: " + textboxPort->Text,
			"Save File Path: " + textboxFilepath->Text
		};

		for each (String ^ line in serverSetup) {
			ReceiveStatus(line, textboxStatus);
		}

		ReceiveStatus("All fields validated successfully.", textboxStatus);

		// Saving Set-up Configuration Ini File
		SaveConfigData(maskedTextBoxIp, textboxPort, textboxFilepath, textboxStatus);

		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
		/*===========================================================================*
		 *      Multi-Thread Logic
		 *			@Pending Locking the Status Update so that only one thread can update it.																*
		 *==========================================================================*/
		 // Start the server on a new background thread so that the UI thread remains free.
		Thread^ serverThread = gcnew Thread(gcnew ThreadStart(this, &MyForm::StartServerThread));
		serverThread->IsBackground = true;
		serverThread->Start();

		return;
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
		ReceiveStatus("All connection has been stopped", textboxStatus);
		// Call StopServer to clean up all sockets and Winsock
		if (socketHandlerInstance != nullptr) {
			socketHandlerInstance->StopServer();
		}
		Sleep(3000);
		// Re-enable the Startreceive button
		buttonHandler->StartCancelActive(btnStartreceive, btnCancel, true);
		//ReceiveStatus("System has been cleared to start again.", textboxStatus);
		//this->Close();
	}


	private: System::Void btnSend_Click(System::Object^ sender, System::EventArgs^ e) {
		// Check if any client sockets are available.
		if (socketHandlerInstance == nullptr || socketHandlerInstance->ClientSockets->Count == 0) {
			ReceiveStatus("No clients are currently connected.", textboxStatus);
			return;
		}

		// Optionally check if the message is empty
		if (String::IsNullOrWhiteSpace(textboxServerMsg->Text)) {
			ReceiveStatus("Please enter a message to send.", textboxStatus);
			return;
		}

		int selectedSocket;

		// If there is exactly one client, send the message directly.
		if (socketHandlerInstance->ClientSockets->Count == 1) {
			selectedSocket = socketHandlerInstance->ClientSockets[0];
		}
		else {
			// More than one client: let the user choose via a selection form.
			ClientSelectionForm^ selectionForm = gcnew ClientSelectionForm();

			// Populate the ListBox in the dialog with the connected sockets.
			// Assume ClientSelectionForm has a public ListBox named listBoxClients.
			int clientNumber = 1;
			for each(int sock in socketHandlerInstance->ClientSockets) {
				selectionForm->listBoxClients->Items->Add("Client No." + clientNumber.ToString() + " on Socket: " + sock.ToString());
				clientNumber++;
			}

			if (selectionForm->listBoxClients->Items->Count > 0)
				selectionForm->listBoxClients->SelectedIndex = 0;  // Select the first item by default
			// Show the form as a modal dialog.
			if (selectionForm->ShowDialog() == System::Windows::Forms::DialogResult::OK) {
				int selectedIndex = selectionForm->listBoxClients->SelectedIndex;
				if (selectedIndex >= 0) {
					selectedSocket = socketHandlerInstance->ClientSockets[selectedIndex];
				}
				else {
					ReceiveStatus("No client selected.", textboxStatus);
					return;
				}
			}
		}

		// Method to send the message.
		if (!socketHandlerInstance->MessageSendToClient(selectedSocket, textboxServerMsg, chatLogRichText, textboxStatus)) {
			ReceiveStatus("Failed to send message to the selected client.", textboxStatus);
		}
		else {
			// Write the server message to file using the overloaded WriteBufferToFile.
			// This will use the file path from textboxFilepath and the message from textboxServerMsg.
			if (!WriteBufferToFile(textboxFilepath, textboxServerMsg)) {
				ReceiveStatus("Failed to write message to file.", textboxStatus);
			}
			// Clear the TextBox after successfully sending the message.
			textboxServerMsg->Clear();
		}
	}

};
}