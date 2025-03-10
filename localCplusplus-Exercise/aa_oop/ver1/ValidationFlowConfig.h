#pragma once
#ifndef VALIDATIONFLOWCONFIG_H
#define VALIDATIONFLOWCONFIG_H

#include <memory>
#include <vector>
#include "ValidationFlow.h"        // Contains definitions for InputHandler, ValidationBlock, etc.
#include "ValidationFlowBuilder.h" // Contains the builder class to configure the flow

// -----------------------------------------------------------------------------
// Function: setupValidationBlocks()
// Purpose: Uses the ValidationFlowBuilder to create and configure the validation blocks
//          with proper branching.
//          The configuration in this example creates the following chain:
//            - Block1 (IpNotEmptyBlock): Checks that the IP address is not empty.
//                On success, it proceeds to Block2.
//            - Block2 (ValidIpFormatBlock): Validates the IP address format.
//                On success, it goes to Block3.
//                On failure, it goes to Block5 (DummyBlock).
//            - Block3 (PortNotEmptyBlock): Checks that the Port field is not empty.
//            - Block5 (DummyBlock): A dummy block executed when Block2 fails.
// Returns: A vector containing the starting block of the validation flow.
inline std::vector<std::shared_ptr<ValidationBlock>> setupValidationBlocks() {
    // Create an instance of the builder.
    ValidationFlowBuilder builder;

    // Register each block with a unique identifier.
    builder.addBlock("Block1", std::make_shared<IpNotEmptyBlock>());
    builder.addBlock("Block2", std::make_shared<ValidIpFormatBlock>());
    builder.addBlock("Block3", std::make_shared<PortNotEmptyBlock>());
    builder.addBlock("Block4", std::make_shared<PathNotEmptyBlock>());
    builder.addBlock("Block5", std::make_shared<PortIsValid>());
    builder.addBlock("Block6", std::make_shared<PathIsValid>());
    builder.addBlock("Block7", std::make_shared<TestingOfMyBranch>());

    // Configure transitions between blocks:
    // From Block1: on success -> Block2.
    builder.setTransition("Block1", "Block2");
    // From Block2: on success -> Block3; on failure -> Block7.
    builder.setTransition("Block2", "Block3", "Block7");
    builder.setTransition("Block3", "Block4");
    builder.setTransition("Block4", "Block5");
    builder.setTransition("Block5", "Block6");

    // Example: To add another block (e.g., Block4) after Block3, you could do:
    // builder.addBlock("Block4", std::make_shared<CustomLogicBlock>([](const InputHandler& input, std::string& errorMessage) {
    //     // Custom logic goes here.
    //     return true;
    // }));
    // builder.setTransition("Block3", "Block4");

    // Return the starting block in a vector (runFlow() expects a vector).
    return { builder.build() };
}

#endif // VALIDATIONFLOWCONFIG_H

