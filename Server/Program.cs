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
            _ = HandleClientAsync(clientSocket);
        }
    }

    private static async Task HandleClientAsync(Socket clientSocket)
    {
        IPEndPoint clientEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
        string clientIP = clientEndPoint.Address.ToString();

        byte[] buffer = new byte[1024];
        int receivedLength = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
        string clientMessage = Encoding.ASCII.GetString(buffer, 0, receivedLength);

        string serverResponse = "Привіт, клієнте!";
        await clientSocket.SendAsync(Encoding.ASCII.GetBytes(serverResponse), SocketFlags.None);

        Console.WriteLine($"о {DateTime.Now.ToString("HH:mm")} від [{clientIP}] отримано рядок: {clientMessage}");

        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }
}

