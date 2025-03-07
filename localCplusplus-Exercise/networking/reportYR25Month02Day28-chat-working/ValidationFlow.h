#pragma once
#ifndef VALIDATIONFLOW_H
#define VALIDATIONFLOW_H

#include "FileTransfer.h"
#include <iostream>
#include <string>
#include <vector>
#include <memory>
#include <queue>
#include <functional>
#include <ws2tcpip.h> //inet_pton
//#include <msclr/marshal.h>

// For inet_pton on POSIX systems. On Windows, use winsock2.h
#ifdef _WIN32
#include <winsock2.h> // For inet_pton on Windows
#else
#include <arpa/inet.h> // For inet_pton on POSIX systems
#endif


//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *              Input Base Class
 *												*
 *=============================================*/
 // The InputHandler class holds the basic input values.
class InputHandlerNative {
private:
	std::string m_ipAddress;
	std::string m_portStr;
	std::string m_filePath;
	FocusPosition m_lastPosFocus;

public:
	InputHandlerNative(const std::string& ip, const std::string& port, const std::string& path)
		: m_ipAddress(ip), m_portStr(port), m_filePath(path), m_lastPosFocus(FocusPosition::DEFAULT) {
	}

	virtual ~InputHandlerNative() = default;

	const std::string& getIpAddress() const { return m_ipAddress; }
	const std::string& getPortStr() const { return m_portStr; }
	const std::string& getFilePath() const { return m_filePath; }

	// Added setter for lastPosFocus
	void setLastPosFocus(FocusPosition pos) { m_lastPosFocus = pos; }
	FocusPosition getLastPosFocus()  const { return m_lastPosFocus; }

};

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *            Extra Field Input Derived Class
 *												*
 *=============================================*/
// Can derive a new class from InputHandler.
// Example:
//
// class AdvancedInputHandler : public InputHandler {
// public:
//     AdvancedInputHandler(const std::string& ip, const std::string& port, const std::string& path, const std::string& username)
//         : InputHandler(ip, port, path), username(username) {}
//
//     const std::string& getUsername() const { return username; }
//
// private:
//     std::string username;
// };

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *             Validation Flow Class
 *												*
 *=============================================*/
 // Base class for validation blocks.
 // Using std::enable_shared_from_this helps when sharing pointers.
class ValidationBlock : public std::enable_shared_from_this<ValidationBlock> {
public:
	virtual ~ValidationBlock() = default;

	// Each block implements execute() to perform its check.
	// It returns true if the check passes, false if it fails.
	virtual bool execute(InputHandlerNative& input, std::string& errorMessage) = 0;

	// Add a block to the success branch.
	void addOnSuccess(std::shared_ptr<ValidationBlock> block) {
		onSuccess.push_back(block);
	}

	// Add a block to the failure branch.
	void addOnFailure(std::shared_ptr<ValidationBlock> block) {
		onFailure.push_back(block);
	}

	// Accessors for the branch containers.
	const std::vector<std::shared_ptr<ValidationBlock>>& getOnSuccess() const {
		return onSuccess;
	}
	const std::vector<std::shared_ptr<ValidationBlock>>& getOnFailure() const {
		return onFailure;
	}

protected:
	std::vector<std::shared_ptr<ValidationBlock>> onSuccess;
	std::vector<std::shared_ptr<ValidationBlock>> onFailure;
};

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *             Utility function
 *												*
 *=============================================*/
 // Checks if a string is empty or contains only whitespace.
inline bool isEmptyOrWhitespace(const std::string& str) {
	return str.find_first_not_of(" \t\n\r") == std::string::npos;
}

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *       --- Derived Validation Blocks ---
 *												*
 *=============================================*/
 // Block 1: Checks that the IP address field is not empty.
class IpNotEmptyBlock : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		if (isEmptyOrWhitespace(input.getIpAddress())) {
			errorMessage = "Please fill the IP address field.";
			input.setLastPosFocus(FocusPosition::IP);
			return false;
		}
		return true;
	}
};

// Block 2: Validates the IP address format using inet_pton.
class ValidIpFormatBlock : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		const std::string& ip = input.getIpAddress();
		sockaddr_in sa;
		if (inet_pton(AF_INET, ip.c_str(), &(sa.sin_addr)) != 1) {
			errorMessage = "Incorrect IP address has been entered.";
			input.setLastPosFocus(FocusPosition::IP);
			return false;
		}
		return true;
	}
};

// Block 3: Checks that the Port field is not empty.
class PortNotEmptyBlock : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		if (isEmptyOrWhitespace(input.getPortStr())) {
			errorMessage = "Please fill the Port number field.";
			input.setLastPosFocus(FocusPosition::PORT);
			return false;
		}
		return true;
	}
};

// Block 4: Checks that the Save Path field is not empty.
class PathNotEmptyBlock : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		if (isEmptyOrWhitespace(input.getFilePath())) {
			errorMessage = "Please fill the Port number field.";
			input.setLastPosFocus(FocusPosition::PATH);
			return false;
		}
		return true;

	}
};

// Block 5: Checks that the Port is a valid entry.
class PortIsValid : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		//System::String^ portStr = msclr::interop::marshal_as<System::String^>(input.getPortStr());
		System::String^ portStr = gcnew System::String(input.getPortStr().c_str());

		int port;
		if (!Int32::TryParse(portStr, port)) {
			errorMessage = "Invalid port number. Not a valid integer.";
			input.setLastPosFocus(FocusPosition::PORT); // Directly modify the original input
			// Non-const copy of input to call setLastPosFocus
			/*InputHandlerNative nonConstInput = input;
			nonConstInput.setLastPosFocus(FocusPosition::PORT);*/
			return false;
		}
		if (port < 1024 || port > 65535) {
			errorMessage = "Invalid port number. Allowable port (1024-65535)";
			input.setLastPosFocus(FocusPosition::PORT); // Directly modify the original input
			return false;
		}
		return true;
	}
};

// Block 6: Checks that the Path is Valid.
class PathIsValid : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		// Convert the native file path to a managed string.
		//System::String^ filePathStr = msclr::interop::marshal_as<System::String^>(input.getFilePath());
		System::String^ filePathStr = gcnew System::String(input.getFilePath().c_str());

		if (filePathStr->IndexOfAny(Path::GetInvalidPathChars()) >= 0) {
			errorMessage = "Invalid characters in file path.";
			input.setLastPosFocus(FocusPosition::PATH);
			return false;
		}

		if (!Path::IsPathRooted(filePathStr)) {
			errorMessage = "File path is not absolute.";
			input.setLastPosFocus(FocusPosition::PATH);
			return false;
		}

		String^ directory = Path::GetDirectoryName(filePathStr);
		if (!Directory::Exists(directory)) {
			errorMessage = "Directory does not exist.";
			input.setLastPosFocus(FocusPosition::PATH);
			return false;
		}
		try {
			Directory::GetFiles(directory); // Check write access
		}
		catch (UnauthorizedAccessException^) {
			errorMessage = "No write permission to the directory.";
			input.setLastPosFocus(FocusPosition::PATH);
			return false;
		}
		catch (Exception^) {
			errorMessage = "Invalid directory.";
			input.setLastPosFocus(FocusPosition::PATH);
			return false;
		}
		return true;

	}
};

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *       --- Derived Validation Blocks ---
 *				@ Custom Logic								*
 *=============================================*/
// CustomLogicBlock: Wraps a custom function (std::function) that implements user-defined logic.
// The custom function returns true (success) or false (failure) and may update errorMessage.
class CustomLogicBlock : public ValidationBlock {
public:
	using LogicFunc = std::function<bool(const InputHandlerNative&, std::string&)>;
	CustomLogicBlock(LogicFunc func) : logicFunc(func) {}

	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		return logicFunc(input, errorMessage);
	}

private:
	LogicFunc logicFunc;
};

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*==============================================*
 *       --- Flow Processing ---
 *												*
 *=============================================*/
 // The runFlow() function processes the validation flow iteratively using a queue.
 // It supports branches and loops, and uses a maximum iteration count to guard against infinite cycles.
inline void runFlow(std::shared_ptr<ValidationBlock> start,
	InputHandlerNative& input,
	std::string& errorMessage,
	int maxIterations = 1000) {

	std::queue<std::shared_ptr<ValidationBlock>> workQueue;
	workQueue.push(start);
	int iterations = 0;

	while (!workQueue.empty() && iterations < maxIterations) {
		auto block = workQueue.front();
		workQueue.pop();
		bool result = block->execute(input, errorMessage);
		if (result) {
			for (auto& child : block->getOnSuccess())
				workQueue.push(child);
		}
		else {
			for (auto& child : block->getOnFailure())
				workQueue.push(child);
		}
		iterations++;
	}
	if (iterations >= maxIterations)
		std::cout << "Max iterations reached, potential infinite loop detected." << std::endl;
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////
#endif // VALIDATIONFLOW_H
