using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private TcpListener listener;
    private List<TcpClient> clients = new List<TcpClient>();

    public Server(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("Server started on port " + port);

        AcceptClients();
    }

    private void AcceptClients()
    {
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            clients.Add(client);
            Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);

            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received from client " + client.Client.RemoteEndPoint + ": " + message);
                BroadcastMessage(message, client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            clients.Remove(client);
            client.Close();
        }
    }

    private void BroadcastMessage(string message, TcpClient sender)
    {
        foreach (TcpClient client in clients)
        {
            if (client != sender)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        int port = 12345;
        Server server = new Server(port);
    }
}
