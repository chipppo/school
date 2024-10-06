using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//importi       
class Client
{
    private const int Port = 8888;//port za connect na server s klient
    private const string ServerIp = "127.0.0.1";//konstanta za Ipv4 na server   

    static void Main()
    {
        TcpClient client = new TcpClient(ServerIp, Port);//inicializirane promenliva klas TcpClient s atributi port i ip
        Console.WriteLine("Connected to server. Start chatting!");

        NetworkStream stream = client.GetStream();//vzimane na potoka ot servera

        Thread receiveThread = new Thread(ReceiveMessages);// izvikvane funkcia za chetene na saobshtenia
        receiveThread.Start(stream);//inicializirane na potoka
        while (true)//cikul (dokato ne zatvorish programata)koito izprashta saobshtenia
        {
            string message = Console.ReadLine();//chetene na saobshtenie ot consola
            byte[] buffer = Encoding.ASCII.GetBytes(message);//buffer na potoka
            stream.Write(buffer, 0, buffer.Length);//izprashtane na saobshtenie
        }
    }

    static void ReceiveMessages(object obj)//funkcia poluchavane msg
    {
        NetworkStream stream = (NetworkStream)obj;//promenliva tip potok
        byte[] buffer = new byte[1024];
        int bytesRead;

        while (true)
        {
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);//chetene na stringa v bufera
                if (bytesRead == 0)//ako nqma string izliza ot funkciata
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead); // printirane na saobshtenieto
                Console.WriteLine(message);
            }
            catch (Exception)//preventva crash na programata pri exception
            {
                break;
            }
        }
    }
}