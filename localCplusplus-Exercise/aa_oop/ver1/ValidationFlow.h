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
 *				@ Abstract Input								*
 *=============================================*/
 // The InputHandler class holds the basic input values.
class InputHandlerNative {
public:
	virtual ~InputHandlerNative() = default;
	/*const : It prevents the caller from accidentally
	changing the string value that might be part of the object's internal state.
	For example, if the IP address is stored internally and you simply want to provide
	read access without allowing modifications, marking it as const is the correct approach.
	Abstract inpur needed at least one virtual function*/
	virtual const std::string& getIpAddress() const = 0;
	/*virtual const std::string& getPortStr() const = 0;
	virtual const std::string& getFilePath() const = 0;*/
};

// A concrete implementation for a specific validation flow.
class CustomInput : public InputHandlerNative {
private:
	std::string m_ipAddress;
	std::string m_portStr;
	std::string m_filePath;
	FocusPosition m_lastPosFocus;
	// const std::string& ip, const std::string& port, const std::string& path, int extra
public:
	CustomInput(const std::string& ip, const std::string& port, const std::string& path)
		: m_ipAddress(ip), m_portStr(port), m_filePath(path), m_lastPosFocus(FocusPosition::DEFAULT) {
	}

	const std::string& getIpAddress() const override { return m_ipAddress; }
	const std::string& getPortStr() const  { return m_portStr; } // overrride is not needed because it is not declare on the base class
	const std::string& getFilePath() const  { return m_filePath; }

	// Additional getter for the extra parameter.
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

	// execute() performs the check on the input.
	// It returns true if the check passes; false if it fails.
	virtual bool execute(InputHandlerNative& input, std::string& errorMessage) = 0;

	// nextBlocks() returns the next block(s) to run based on the outcome.
	// The default implementation returns blocks added via addOnSuccess or addOnFailure.
	virtual std::vector<std::shared_ptr<ValidationBlock>> nextBlocks(bool success) const {
		return success ? onSuccess : onFailure;
	}

	// Helper function to add a block to run on success.
	void addOnSuccess(std::shared_ptr<ValidationBlock> block) {
		onSuccess.push_back(block);
	}
	// Helper function to add a block to run on failure.
	void addOnFailure(std::shared_ptr<ValidationBlock> block) {
		onFailure.push_back(block);
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
	bool execute(InputHandlerNative& input, std::string & errorMessage) override {
		// Dynamic cast to our specialized input.
		// assign customInput
		CustomInput* inputHandler = dynamic_cast<CustomInput*>(&input);
		if (!inputHandler) {
			errorMessage = "Input is not of type inputHandler.";
			return false;
		}
		if (isEmptyOrWhitespace(inputHandler->getIpAddress())) {
			errorMessage = "Please fill the IP address field.";
			inputHandler->setLastPosFocus(FocusPosition::IP);
			return false;
		}
		return true;
	}
};

// Block 2: Validates the IP address format using inet_pton.
class ValidIpFormatBlock : public ValidationBlock {
private:
	std::shared_ptr<ValidationBlock> nextSuccess; // e.g. Block3
	std::shared_ptr<ValidationBlock> nextFailure; // e.g. Block5
public:
	// Set the block to go to when validation succeeds.
	void setNextSuccess(std::shared_ptr<ValidationBlock> block) { nextSuccess = block; }
	// Set the block to go to when validation fails.
	void setNextFailure(std::shared_ptr<ValidationBlock> block) { nextFailure = block; }

	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		// Dynamic cast to our specialized input.
		// assign customInput
		CustomInput* inputHandler = dynamic_cast<CustomInput*>(&input);
		if (!inputHandler) {
			errorMessage = "Input is not of type inputHandler.";
			return false;
		}
	
		const std::string& ip = inputHandler->getIpAddress();
		sockaddr_in sa;
		if (inet_pton(AF_INET, ip.c_str(), &(sa.sin_addr)) != 1) {
			errorMessage = "Incorrect IP address has been entered.";
			inputHandler->setLastPosFocus(FocusPosition::IP);
			return false;
		}
		return true;
	}
	// nextBlocks() chooses which branch to follow based on the execution result.
	std::vector<std::shared_ptr<ValidationBlock>> nextBlocks(bool success) const override {
		std::vector<std::shared_ptr<ValidationBlock>> result;
		if (success && nextSuccess)
			result.push_back(nextSuccess);
		else if (!success && nextFailure)
			result.push_back(nextFailure);
		return result;
	}
};

// Block 3: Checks that the Port field is not empty.
class PortNotEmptyBlock : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		// Dynamic cast to our specialized input.
		// assign customInput
		CustomInput* inputHandler = dynamic_cast<CustomInput*>(&input);
		if (!inputHandler) {
			errorMessage = "Input is not of type inputHandler.";
			return false;
		}
		if (isEmptyOrWhitespace(inputHandler->getPortStr())) {
			errorMessage = "Please fill the Port number field.";
			inputHandler->setLastPosFocus(FocusPosition::PORT);
			return false;
		}
		return true;
	}
};

// Block 4: Checks that the Save Path field is not empty.
class PathNotEmptyBlock : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		// Dynamic cast to our specialized input.
		// assign customInput
		CustomInput* inputHandler = dynamic_cast<CustomInput*>(&input);
		if (!inputHandler) {
			errorMessage = "Input is not of type inputHandler.";
			return false;
		}

		if (isEmptyOrWhitespace(inputHandler->getFilePath())) {
			errorMessage = "Please fill the Port number field.";
			inputHandler->setLastPosFocus(FocusPosition::PATH);
			return false;
		}
		return true;

	}
};

// Block 5: Checks that the Port is a valid entry.
class PortIsValid : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		// Dynamic cast to our specialized input.
		// assign customInput
		CustomInput* inputHandler = dynamic_cast<CustomInput*>(&input);
		if (!inputHandler) {
			errorMessage = "Input is not of type inputHandler.";
			return false;
		}

		//System::String^ portStr = msclr::interop::marshal_as<System::String^>(input.getPortStr());
		System::String^ portStr = gcnew System::String(inputHandler->getPortStr().c_str());

		int port;
		if (!Int32::TryParse(portStr, port)) {
			errorMessage = "Invalid port number. Not a valid integer.";
			inputHandler->setLastPosFocus(FocusPosition::PORT); // Directly modify the original input
			// Non-const copy of input to call setLastPosFocus
			/*InputHandlerNative nonConstInput = input;
			nonConstInput.setLastPosFocus(FocusPosition::PORT);*/
			return false;
		}
		if (port < 1024 || port > 65535) {
			errorMessage = "Invalid port number. Allowable port (1024-65535)";
			inputHandler->setLastPosFocus(FocusPosition::PORT); // Directly modify the original input
			return false;
		}
		return true;
	}
};

// Block 6: Checks that the Path is Valid.
class PathIsValid : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		// Dynamic cast to our specialized input.
		// assign customInput
		CustomInput* inputHandler = dynamic_cast<CustomInput*>(&input);
		if (!inputHandler) {
			errorMessage = "Input is not of type inputHandler.";
			return false;
		}

		// Convert the native file path to a managed string.
		//System::String^ filePathStr = msclr::interop::marshal_as<System::String^>(input.getFilePath());
		System::String^ filePathStr = gcnew System::String(inputHandler->getFilePath().c_str());

		if (filePathStr->IndexOfAny(Path::GetInvalidPathChars()) >= 0) {
			errorMessage = "Invalid characters in file path.";
			inputHandler->setLastPosFocus(FocusPosition::PATH);
			return false;
		}

		if (!Path::IsPathRooted(filePathStr)) {
			errorMessage = "File path is not absolute.";
			inputHandler->setLastPosFocus(FocusPosition::PATH);
			return false;
		}

		String^ directory = Path::GetDirectoryName(filePathStr);
		if (!Directory::Exists(directory)) {
			errorMessage = "Directory does not exist.";
			inputHandler->setLastPosFocus(FocusPosition::PATH);
			return false;
		}
		try {
			Directory::GetFiles(directory); // Check write access
		}
		catch (UnauthorizedAccessException^) {
			errorMessage = "No write permission to the directory.";
			inputHandler->setLastPosFocus(FocusPosition::PATH);
			return false;
		}
		catch (Exception^) {
			errorMessage = "Invalid directory.";
			inputHandler->setLastPosFocus(FocusPosition::PATH);
			return false;
		}
		return true;

	}
};

// Block 7: Checks that the Save Path field is not empty.
class TestingOfMyBranch : public ValidationBlock {
public:
	bool execute(InputHandlerNative& input, std::string& errorMessage) override {
		// Dynamic cast to our specialized input.
		// assign customInput
		CustomInput* inputHandler = dynamic_cast<CustomInput*>(&input);
		if (!inputHandler) {
			errorMessage = "Input is not of type inputHandler.";
			return false;
		}

		errorMessage = "Yeah, my custom branching works but sadly, IP format is not Valid.";
		inputHandler->setLastPosFocus(FocusPosition::IP);
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
 // runFlow() processes the validation blocks iteratively using a queue.
// It calls nextBlocks() on each block to determine the subsequent blocks.
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
		// Determine next block(s) based on success or failure.
		std::vector<std::shared_ptr<ValidationBlock>> nexts = block->nextBlocks(result);
		for (auto& nextBlock : nexts) {
			workQueue.push(nextBlock);
		}
		iterations++;
	}
	if (iterations >= maxIterations)
		std::cout << "Max iterations reached, potential infinite loop detected." << std::endl;
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////
#endif // VALIDATIONFLOW_H
