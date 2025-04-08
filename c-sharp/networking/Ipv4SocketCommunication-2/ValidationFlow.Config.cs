using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipv4SocketCommunication
{
    // The ValidationFlowConfig class is responsible for creating and configuring the validation flow.
    // It uses the ValidationFlowBuilder to register blocks and set transitions between them.
    public static class ValidationFlowConfig
    {
        // Creates and configures the validation blocks with proper branching.
        // Returns a list containing the starting block of the validation flow.
        public static List<ValidationFlow.ValidationBlock> SetupValidationBlocks()
        {
            var builder = new ValidationFlowBuilder();

            // Register each block with a unique identifier.
            builder.AddBlock("Block1", new ValidationFlow.IpNotEmptyBlock());
            builder.AddBlock("Block2", new ValidationFlow.ValidIpFormatBlock());
            builder.AddBlock("Block3", new ValidationFlow.PortNotEmptyBlock());
            builder.AddBlock("Block4", new ValidationFlow.PathNotEmptyBlock());
            builder.AddBlock("Block5", new ValidationFlow.PortIsValid());
            builder.AddBlock("Block6", new ValidationFlow.PathIsValid());
            builder.AddBlock("Block7", new ValidationFlow.TestingOfMyBranch());

            // Configure transitions between blocks:
            // From Block1: on success -> Block2.
            builder.SetTransition("Block1", "Block2");
            // From Block2: on success -> Block3; on failure -> Block7.
            builder.SetTransition("Block2", "Block3", "Block7");
            // Continue chaining the flow.
            builder.SetTransition("Block3", "Block4");
            builder.SetTransition("Block4", "Block5");
            builder.SetTransition("Block5", "Block6");

            // Return the starting block in a list (your FlowProcessor expects a collection).
            return new List<ValidationFlow.ValidationBlock> { builder.Build() };
        }
    }

}
