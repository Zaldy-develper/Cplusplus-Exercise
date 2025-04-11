using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipv4SocketCommunication
{
    public partial class ValidationFlow
    {
        public enum FocusPosition
        {
            IP,
            PORT,
            PATH,
            DEFAULT
        }
        public abstract class InputHandler
        {
            public abstract string serverName { get; }
        }

        // Parameter Object Pattern
        public class CustomInputParameters
        {
            public string Ip { get; set; }
            public string Port { get; set; }
            public string FilePath { get; set; }
            public MaskedTextBox IpMaskedTextBox { get; set; }
        }
        // Concrete implementation for a specific input validation flow.
        public class CustomInput : InputHandler
        {
            private readonly string m_ipAddress;
            private readonly string m_portStr;
            private readonly string m_filePath;
            private FocusPosition m_lastPosFocus;

            // Property to hold the MaskedTextBox reference.
            public MaskedTextBox IpMaskedTextBox { get; }

            // Constructor like in C++
            // Constructor accepting a parameter object.
            public CustomInput(CustomInputParameters parameters)
            {
                m_ipAddress = parameters.Ip;
                m_portStr = parameters.Port;
                m_filePath = parameters.FilePath;
                IpMaskedTextBox = parameters.IpMaskedTextBox;
                m_lastPosFocus = FocusPosition.DEFAULT;

            }

            // Gets the server custom name.
            public override string serverName => "zaldy chatroom";

            // Gets the IP address.
            public string IpAddress => m_ipAddress;
            // This is the same with:
            /*
            public string IpAddress
            {
                get { return m_ipAddress; }
            }
            */

            // Gets the port string.
            public string PortStr => m_portStr;

            // Gets the file path.
            public string FilePath => m_filePath;

            // Gets or sets the last focus position.
            public FocusPosition LastPosFocus
            {
                get => m_lastPosFocus;
                set => m_lastPosFocus = value;
            }
        }
    }
}
