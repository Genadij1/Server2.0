using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Server
{
    static async Task Main()
    {
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        IPEndPoint ep = new IPEndPoint(ip, 8888);
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(ep);
        serverSocket.Listen(10);

        Console.WriteLine("Server started. Waiting for a connection...");

        while (true)
        {
            Socket clientSocket = await serverSocket.AcceptAsync();
            _ = HandleClientAsync(clientSocket); // Fire-and-forget to handle multiple clients concurrently
        }
    }

    static async Task HandleClientAsync(Socket clientSocket)
    {
        IPEndPoint clientEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
        string clientIP = clientEndPoint.Address.ToString();

        byte[] buffer = new byte[1024];
        int receivedLength = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
        string clientRequest = Encoding.ASCII.GetString(buffer, 0, receivedLength).Trim();

        string response = string.Empty;
        if (clientRequest.Equals("date", StringComparison.OrdinalIgnoreCase))
        {
            response = DateTime.Now.ToString("yyyy-MM-dd");
        }
        else if (clientRequest.Equals("time", StringComparison.OrdinalIgnoreCase))
        {
            response = DateTime.Now.ToString("HH:mm:ss");
        }
        else
        {
            response = "Invalid request";
        }

        await clientSocket.SendAsync(Encoding.ASCII.GetBytes(response), SocketFlags.None);
        Console.WriteLine($"о {DateTime.Now.ToString("HH:mm")} від [{clientIP}] отримано рядок: {clientRequest}");

        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }
}

