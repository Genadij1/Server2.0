using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

public class ServerForm : Form
{
    private TextBox textBox;
    private Button startButton;

    public ServerForm()
    {
        textBox = new TextBox { Multiline = true, Dock = DockStyle.Fill, ReadOnly = true };
        startButton = new Button { Text = "Start Server", Dock = DockStyle.Top };
        startButton.Click += StartButton_Click;

        Controls.Add(textBox);
        Controls.Add(startButton);
    }

    private async void StartButton_Click(object sender, EventArgs e)
    {
        startButton.Enabled = false;
        await StartServerAsync();
    }

    private async Task StartServerAsync()
    {
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        IPEndPoint ep = new IPEndPoint(ip, 8888);
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(ep);
        serverSocket.Listen(10);

        AppendText("Server started. Waiting for a connection...");

        while (true)
        {
            Socket clientSocket = await serverSocket.AcceptAsync();
            _ = HandleClientAsync(clientSocket);
        }
    }

    private async Task HandleClientAsync(Socket clientSocket)
    {
        IPEndPoint clientEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
        string clientIP = clientEndPoint.Address.ToString();

        byte[] buffer = new byte[1024];
        int receivedLength = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
        string clientMessage = Encoding.ASCII.GetString(buffer, 0, receivedLength);

        string serverResponse = "Привіт, клієнте!";
        await clientSocket.SendAsync(Encoding.ASCII.GetBytes(serverResponse), SocketFlags.None);

        AppendText($"о {DateTime.Now.ToString("HH:mm")} від [{clientIP}] отримано рядок: {clientMessage}");

        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }

    private void AppendText(string text)
    {
        if (textBox.InvokeRequired)
        {
            textBox.Invoke(new Action<string>(AppendText), text);
        }
        else
        {
            textBox.AppendText(text + Environment.NewLine);
        }
    }
}

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ServerForm());
    }
}

