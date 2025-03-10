#pragma once
#ifndef VALIDATIONFLOWBUILDER_H
#define VALIDATIONFLOWBUILDER_H

#include <unordered_map>
#include <string>
#include <memory>
#include "ValidationFlow.h" // Includes definitions for ValidationBlock and its derived classes

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
/*======================================================================================================*
 *              Class: ValidationFlowBuilder
 *				@ Purpose: Centralizes the creation and configuration of the validation flow.
 *				           It allows you to register blocks by unique IDs and set transitions between them.
 *						   This minimizes editing errors when blocks are added, removed, or reordered.
 *======================================================================================================*/

class ValidationFlowBuilder {
public:
    // addBlock() registers a validation block with a unique identifier.
    // The first block added becomes the starting block.
    void addBlock(const std::string& id, std::shared_ptr<ValidationBlock> block) {
        blocks[id] = block;
        if (startBlockId.empty()) {
            startBlockId = id;
        }
    }

    // setTransition() sets the branching for a given block.
    // Parameters:
    // - fromId: The block from which the transition originates.
    // - onSuccessId: The block to run if execution succeeds.
    // - onFailureId: (Optional) The block to run if execution fails.
    //
    // How it works:
    // 1. If the block supports dynamic branching (e.g. ValidIpFormatBlock), it sets its
    //    nextSuccess and nextFailure pointers.
    // 2. Otherwise, it uses default chaining by adding the blocks to the onSuccess/onFailure vectors.
    // This makes it clear and easy to visualize what happens in each branch.

    void setTransition(const std::string& fromId,
        const std::string& onSuccessId,
        const std::string& onFailureId = "") {
        // Find the originating block.
        auto it = blocks.find(fromId);
        if (it != blocks.end()) {
            // Check if the block supports dynamic branching (example: ValidIpFormatBlock).
            if (auto dynamicBlock = std::dynamic_pointer_cast<ValidIpFormatBlock>(it->second)) {
                // For dynamic branching, set the next block on success.
                if (!onSuccessId.empty() && blocks.count(onSuccessId))
                    dynamicBlock->setNextSuccess(blocks[onSuccessId]);
                    //dynamicBlock->setNextSuccess(blocks[onSuccessId]);
                // And set the next block on failure.
                if (!onFailureId.empty() && blocks.count(onFailureId))
                    dynamicBlock->setNextFailure(blocks[onFailureId]);
            }
            else {
                // For blocks using default chaining, add to the onSuccess vector.
                if (!onSuccessId.empty() && blocks.count(onSuccessId))
                    it->second->addOnSuccess(blocks[onSuccessId]);
                // And optionally add to the onFailure vector.
                if (!onFailureId.empty() && blocks.count(onFailureId))
                    it->second->addOnFailure(blocks[onFailureId]);
            }
        }
    }

    // build() returns the starting block of the validation flow.
    std::shared_ptr<ValidationBlock> build() {
        return blocks[startBlockId];
    }

private:
    // A map that holds the validation blocks with their unique identifiers.
    std::unordered_map<std::string, std::shared_ptr<ValidationBlock>> blocks;
    // The unique identifier for the starting block.
    std::string startBlockId;
};


#endif // VALIDATIONFLOWBUILDER_H
