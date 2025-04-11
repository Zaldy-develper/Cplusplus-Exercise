using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipv4SocketCommunication
{
    // The ValidationFlowBuilder centralizes the creation and configuration of the validation flow.
    // It registers validation blocks with unique identifiers and sets up transitions between them.
    public class ValidationFlowBuilder
    {
        // Use fully qualified type names because ValidationBlock and its derived types are declared in ValidationFlow.
        private readonly Dictionary<string, ValidationFlow.ValidationBlock> blocks = new Dictionary<string, ValidationFlow.ValidationBlock>();
        private string startBlockId = string.Empty;

        // Registers a validation block with a unique identifier.
        // The first block added becomes the starting block.
        public void AddBlock(string id, ValidationFlow.ValidationBlock block)
        {
            blocks[id] = block;
            if (string.IsNullOrEmpty(startBlockId))
            {
                startBlockId = id;
            }
        }

        // Sets the transition for a given block.
        // Parameters:
        // - fromId: Identifier of the originating block.
        // - onSuccessId: Identifier of the block to execute on success.
        // - onFailureId: (Optional) Identifier of the block to execute on failure.
        public void SetTransition(string fromId, string onSuccessId, string onFailureId = "")
        {
            if (blocks.ContainsKey(fromId))
            {
                var fromBlock = blocks[fromId];

                // Check if the block supports dynamic branching (e.g., ValidIpFormatBlock).
                if (fromBlock is ValidationFlow.ValidIpFormatBlock dynamicBlock)
                {
                    if (!string.IsNullOrEmpty(onSuccessId) && blocks.ContainsKey(onSuccessId))
                    {
                        dynamicBlock.SetNextSuccess(blocks[onSuccessId]);
                    }
                    if (!string.IsNullOrEmpty(onFailureId) && blocks.ContainsKey(onFailureId))
                    {
                        dynamicBlock.SetNextFailure(blocks[onFailureId]);
                    }
                }
                else
                {
                    // For blocks using default chaining, add to the onSuccess and onFailure lists.
                    if (!string.IsNullOrEmpty(onSuccessId) && blocks.ContainsKey(onSuccessId))
                    {
                        fromBlock.AddOnSuccess(blocks[onSuccessId]);
                    }
                    if (!string.IsNullOrEmpty(onFailureId) && blocks.ContainsKey(onFailureId))
                    {
                        fromBlock.AddOnFailure(blocks[onFailureId]);
                    }
                }
            }
        }

        // Returns the starting block of the validation flow.
        public ValidationFlow.ValidationBlock Build()
        {
            return blocks[startBlockId];
        }
    }
}
