using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static readonly List<TcpClient> clients = new List<TcpClient>();//spisuk s klientite vurzani kum survura
    private const int Port = 8888;//port za svurzvane

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, Port);//servera e v izchakvane na klienti s port 8888
        server.Start();//otvaryane server
        Console.WriteLine($"Server started on port {Port}");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();//priema klienta v servera
            clients.Add(client);//dobavya v spisuka
            Thread clientThread = new Thread(HandleClient);//puska funkcia handle
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj;//izbira segashnia klient
        NetworkStream stream = tcpClient.GetStream();//vzima potoka na klienta

        byte[] buffer = new byte[1024];
        int bytesRead;

        while (true)
        {
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);//zapisva duljina na procheteni bytove
                if (bytesRead == 0)//ako nqma nishto izliza ot funkciata
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);//chete saobshtenie
                Console.WriteLine($"Received: {message}");

                BroadcastMessage(tcpClient, message);//spodelya s drugite
            }
            catch (Exception)//prevents crashes 
            {
                break;
            }
        }

        clients.Remove(tcpClient);//sled handle na klienta go maha ot spisuka
        tcpClient.Close();
    }

    static void BroadcastMessage(TcpClient sender, string message)//funkcia za spodelyane na msg
    {
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message);//bufer sus msg

        foreach (TcpClient client in clients)//cikul otnasyasht se za vseki klient
        {
            if (client != sender)//za vseki osven izprashtashtia
            {
                NetworkStream stream = client.GetStream();
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length);//printirane na prochetenia string
            }
        }
    }
}