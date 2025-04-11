using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ipv4SocketCommunication
{
    public partial class ValidationFlow
    {
        // The abstract base class for validation blocks.
        public abstract class ValidationBlock
        {
            // Lists for storing the next validation blocks for success or failure.
            protected readonly List<ValidationBlock> onSuccess = new List<ValidationBlock>();
            protected readonly List<ValidationBlock> onFailure = new List<ValidationBlock>();

            // Executes the validation check on the provided input.
            public abstract bool Execute(InputHandler input, out string errorMessage);

            // Gets the next validation blocks based on the outcome
            // <returns>A list of subsequent validation blocks.</returns>
            public virtual List<ValidationBlock> NextBlocks(bool success)
            {
                return success ? onSuccess : onFailure;
            }

            // Adds a validation block to run on success.
            public void AddOnSuccess(ValidationBlock block)
            {
                onSuccess.Add(block);
            }

            // Adds a validation block to run on failure.
            public void AddOnFailure(ValidationBlock block)
            {
                onFailure.Add(block);
            }
        }

        // Utility function to check for empty or whitespace strings.
        public static class Utility
        {
            public static bool IsEmptyOrWhitespace(string str)
            {
                return string.IsNullOrWhiteSpace(str);
            }
        }

        // Block 1: Checks that the IP address field is not empty.
        public class IpNotEmptyBlock : ValidationBlock
        {
            public override bool Execute(InputHandler input, out string errorMessage)
            {
                // Cast to our specialized input.
                if (!(input is CustomInput inputHandler))
                {
                    errorMessage = "Block 1: Input is not of type CustomInput.";
                    return false;
                }

                if (!inputHandler.IpMaskedTextBox.MaskCompleted)
                {
                    // Handle the case when the IP address is not completely entered.
                    errorMessage = "Please fill the IP address field.";
                    inputHandler.IpMaskedTextBox.Focus();
                    return false;
                }

                errorMessage = string.Empty;
                return true;
            }
        }

        // Block 2: Validates the IP address format using IPAddress.TryParse.
        public class ValidIpFormatBlock : ValidationBlock
        {
            private ValidationBlock nextSuccess;
            private ValidationBlock nextFailure;

            // Set the block to go to when validation succeeds.
            public void SetNextSuccess(ValidationBlock block)
            {
                nextSuccess = block;
            }

            // Set the block to go to when validation fails.
            public void SetNextFailure(ValidationBlock block)
            {
                nextFailure = block;
            }

            public override bool Execute(InputHandler input, out string errorMessage)
            {
                if (!(input is CustomInput inputHandler))
                {
                    errorMessage = "Block 2: Input is not of type CustomInput.";
                    return false;
                }

                string ip = inputHandler.IpAddress;
                if (!IPAddress.TryParse(ip, out _))
                {
                    errorMessage = "Incorrect IP address has been entered.";
                    inputHandler.LastPosFocus = FocusPosition.IP;
                    return false;
                }

                errorMessage = string.Empty;
                return true;
            }

            // Choose next block(s) based on validation result.
            public override List<ValidationBlock> NextBlocks(bool success)
            {
                var result = new List<ValidationBlock>();
                if (success && nextSuccess != null)
                {
                    result.Add(nextSuccess);
                }
                else if (!success && nextFailure != null)
                {
                    result.Add(nextFailure);
                }
                return result;
            }
        }

        // Block 3: Checks that the Port field is not empty.
        public class PortNotEmptyBlock : ValidationBlock
        {
            public override bool Execute(InputHandler input, out string errorMessage)
            {
                if (!(input is CustomInput inputHandler))
                {
                    errorMessage = "Block 3: Input is not of type CustomInput.";
                    return false;
                }

                if (Utility.IsEmptyOrWhitespace(inputHandler.PortStr))
                {
                    errorMessage = "Please fill the Port number field.";
                    inputHandler.LastPosFocus = FocusPosition.PORT;
                    return false;
                }

                errorMessage = string.Empty;
                return true;
            }
        }

        // Block 4: Checks that the Save Path field is not empty.
        public class PathNotEmptyBlock : ValidationBlock
        {
            public override bool Execute(InputHandler input, out string errorMessage)
            {
                if (!(input is CustomInput inputHandler))
                {
                    errorMessage = "Block 4: Input is not of type CustomInput.";
                    return false;
                }

                if (Utility.IsEmptyOrWhitespace(inputHandler.FilePath))
                {
                    // Note: The original C++ error message referred to the port field;
                    // adjust the message if needed.
                    errorMessage = "Please fill the file path field.";
                    inputHandler.LastPosFocus = FocusPosition.PATH;
                    return false;
                }

                errorMessage = string.Empty;
                return true;
            }
        }

        // Block 5: Checks that the Port is a valid entry.
        public class PortIsValid : ValidationBlock
        {
            public override bool Execute(InputHandler input, out string errorMessage)
            {
                if (!(input is CustomInput inputHandler))
                {
                    errorMessage = "Block 5: Input is not of type CustomInput.";
                    return false;
                }

                if (!int.TryParse(inputHandler.PortStr, out int port))
                {
                    errorMessage = "Invalid port number. Not a valid integer.";
                    inputHandler.LastPosFocus = FocusPosition.PORT;
                    return false;
                }
                if (port < 1024 || port > 65535)
                {
                    errorMessage = "Invalid port number. Allowable port (1024-65535)";
                    inputHandler.LastPosFocus = FocusPosition.PORT;
                    return false;
                }

                errorMessage = string.Empty;
                return true;
            }
        }

        // Block 6: Checks that the Path is valid.
        public class PathIsValid : ValidationBlock
        {
            public override bool Execute(InputHandler input, out string errorMessage)
            {
                if (!(input is CustomInput inputHandler))
                {
                    errorMessage = "Block 6: Input is not of type CustomInput.";
                    return false;
                }

                string filePath = inputHandler.FilePath;

                if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                {
                    errorMessage = "Invalid characters in file path.";
                    inputHandler.LastPosFocus = FocusPosition.PATH;
                    return false;
                }

                if (!Path.IsPathRooted(filePath))
                {
                    errorMessage = "File path is not absolute.";
                    inputHandler.LastPosFocus = FocusPosition.PATH;
                    return false;
                }

                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    errorMessage = "Directory does not exist.";
                    inputHandler.LastPosFocus = FocusPosition.PATH;
                    return false;
                }

                try
                {
                    // Check write access by trying to get files.
                    Directory.GetFiles(directory);
                }
                catch (UnauthorizedAccessException)
                {
                    errorMessage = "No write permission to the directory.";
                    inputHandler.LastPosFocus = FocusPosition.PATH;
                    return false;
                }
                catch (Exception)
                {
                    errorMessage = "Invalid directory.";
                    inputHandler.LastPosFocus = FocusPosition.PATH;
                    return false;
                }

                errorMessage = string.Empty;
                return true;
            }
        }

        // Block 7: Testing custom branching.
        public class TestingOfMyBranch : ValidationBlock
        {
            public override bool Execute(InputHandler input, out string errorMessage)
            {
                if (!(input is CustomInput inputHandler))
                {
                    errorMessage = "Block 7: Input is not of type CustomInput.";
                    return false;
                }

                errorMessage = "Yeah, my custom branching works but sadly, IP format is not Valid.";
                inputHandler.LastPosFocus = FocusPosition.IP;
                return true;
            }
        }

        // Flow processor that iteratively runs validation blocks.
        public static class FlowProcessor
        {
            public static void RunFlow(ValidationBlock start, InputHandler input, ref string errorMessage, int maxIterations = 1000)
            {
                var workQueue = new Queue<ValidationBlock>();
                workQueue.Enqueue(start);
                int iterations = 0;

                while (workQueue.Count > 0 && iterations < maxIterations)
                {
                    ValidationBlock block = workQueue.Dequeue();
                    bool result = block.Execute(input, out errorMessage);

                    // Enqueue next blocks based on the result.
                    List<ValidationBlock> nextBlocks = block.NextBlocks(result);
                    foreach (var nextBlock in nextBlocks)
                    {
                        workQueue.Enqueue(nextBlock);
                    }
                    iterations++;
                }

                if (iterations >= maxIterations)
                {
                    Console.WriteLine("Max iterations reached, potential infinite loop detected.");
                }
            }
        }





    }
}
