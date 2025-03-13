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
//                On failure, it goes to Block7 (TestingOfMyBranch).
//            - Block3 (PortNotEmptyBlock): Checks that the Port field is not empty.
// Returns: A vector containing the starting block of the validation flow.
inline std::vector<std::shared_ptr<ValidationBlock>> setupValidationBlocks() {
    // Create an instance of the builder.
    ValidationFlowBuilder builder;

    builder.addBlock("Block1", std::make_shared<isIPValid>());
    builder.addBlock("Block2", std::make_shared<PortIsValid>());
    builder.addBlock("Block3", std::make_shared<PathNotEmptyBlock>());
    builder.addBlock("Block4", std::make_shared<isNameValid>());

    builder.setTransition("Block1", "Block2");
    builder.setTransition("Block2", "", "Block4");
    builder.setTransition("Block1", "Block3", "Block2");
    builder.setTransition("Block2", "", "Block4");
    builder.setTransition("Block4", "Block3");

    // Return the starting block in a vector (runFlow() expects a vector).
    return { builder.build() };
}

#endif // VALIDATIONFLOWCONFIG_H
