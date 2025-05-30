#pragma once
#include "FileTransfer.h"
#include <iostream>
#include <cstring>
#include <winsock2.h>
#include <ws2tcpip.h>
#pragma comment(lib, "Ws2_32.lib")
#include <msclr/marshal.h>
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
	
	// My Custom Instance
	// Declare ButtonHandler as a member of the MyForm class
	private: ButtonHandler^ buttonHandler = gcnew ButtonHandler();

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

		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
		/*===========================================================================*
		 *      Validate the input fields // Ip Address, Port, Save File Path
		 *																			*
		 *==========================================================================*/
		int port;
		InputHandler^ handler = gcnew InputHandler(
			textboxIp->Text,
			textboxPort->Text,
			textboxFilepath->Text
		);

		if (handler->IsNullIp() || !handler->IsValidIp() || handler->IsNullPort() || !handler->IsValidPort()
			|| handler->IsNullFilePath() || !handler->IsValidFilePath()) {
			ReceiveStatus(handler->LastError, textboxStatus);

			// Set the focus on the last field that was filled
			switch (handler->LastPosFocus) {
			case FocusPosition::IP:
				textboxIp->Focus();
				break;
			case FocusPosition::PORT:
				textboxPort->Focus();
				break;
			case FocusPosition::PATH:
				textboxFilepath->Focus();
				break;
			case FocusPosition::DEFAULT:
				textboxIp->Focus();
				break;
			}
			return;
		}
		else {
			port = Convert::ToInt32(textboxPort->Text);
			textboxIp->Focus();
		}

		// All validations passed
		// Disable the Startreceive button
		buttonHandler->StartCancelActive(btnStartreceive, btnCancel, false);

		array<String^>^ params = gcnew array<String^>{
			"Ip address: " + textboxIp->Text,
				"Port Number: " + textboxPort->Text,
				"Save File Path: " + textboxFilepath->Text
		};

		for each(String ^ line in params) {
			ReceiveStatus(line, textboxStatus);
		}

		ReceiveStatus("Success fill-up on all fields", textboxStatus);
		
		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
		/*===========================================================================*
		 *      Socket Logic
		 *			@Waiting, and Connecting to Client																*
		 *==========================================================================*/

		SocketHandler^ sockethandler = gcnew SocketHandler(port);
		sockethandler->StartServer(textboxStatus, textboxFilepath);
		
		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
		/*===========================================================================*
		 *      Receive File Transfer Logic 
		 *			
		 *			@Pending: Testing file size limit, etc.																	*
		 *==========================================================================*/
		sockethandler->ReceiveFile(sockethandler->GetNewSocket(), textboxFilepath, textboxStatus) ? true : false;

		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
		/*===========================================================================*
		 *      Closing Logic
		 *			@Deletion of Cancel Button																*
		 *==========================================================================*/
		// ソケットを閉じる and clean up Winsock
		ClosingCleanUp^ cleanup = gcnew ClosingCleanUp(sockethandler->GetNewSocket(), sockethandler->GetServerFd());
		cleanup->Close();
		buttonHandler->StartCancelActive(btnStartreceive, btnCancel, true);
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
		// Re-enable the Startreceive button
		// This Cancel button will be deleted in the future
		buttonHandler->StartCancelActive(btnStartreceive, btnCancel, true);
		this->Close();
	}
};
}