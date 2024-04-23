using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private TcpClient client;
    private NetworkStream stream;

    public Client(string serverIp, int serverPort)
    {
        client = new TcpClient(serverIp, serverPort);
        stream = client.GetStream();

        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        SendMessages();
    }

    private void SendMessages()
    {
        try
        {
            Console.WriteLine("Type a message and press Enter to send:");
            string message;
            while ((message = Console.ReadLine()) != null)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            client.Close();
        }
    }

    private void ReceiveMessages()
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received from server: " + message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            client.Close();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string serverIp = "127.0.0.1"; // آی‌پی سرور را وارد کنید
        int serverPort = 12345; // پورت سرور را وارد کنید
        Client client = new Client(serverIp, serverPort);
    }
}
