using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace Ipv4SocketCommunication.Interfaces
{
    // Methods for status updates and logging.
    // Abstracts the operations that previously depended on WinForms controls.
    public interface IUpdateNotifier
    {
        void UpdateStatus(string message);
        void LogMessage(string message);
        // Expose additional properties if needed (e.g., save file path or server message).
        string SaveFilePath { get; }
        string ServerMessage { get; }
    }

    // Derives from IUpdateNotifier to provide specific UI update methods.
    // Encapsulates parameters specific to a client connection.
    // Addition to providing the client's Socket and number.
    public interface IClientParameters : IUpdateNotifier
    {
        Socket ClientSocket { get; }
        int ClientNumber { get; }
    }

    // Declares the operations of the socket handler.
    public interface ISocketHandler
    {
        bool StartServer(IUpdateNotifier notifier);
        void StopServer();
        void HandleClient(IClientParameters clientParams);
        bool TestMessageSendToClient(Socket socket, IUpdateNotifier notifier);
        bool ReceiveFile(IClientParameters clientParams);
        bool MessageSendToClient(IClientParameters clientParams);
    }
}
