using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientUdp;
using System.Threading;

namespace ServeurUdp
{
    class ServeurUdp
    {
        private static bool continuer = true;
        private static int serverPort = 12345;
        public static Thread thread;

        public static void StartChat()
        {
            thread = new Thread(() => traitement(serverPort));
            thread.Start();
        }

        static void traitement(int port)
        {
            // ************************************************************** Initialisation

            List<EndPoint> subscribedUsers = new List<EndPoint>();
            IDictionary<string, Thread> roomList = new Dictionary<string, Thread>();

        // Création de la socket d'écoute UDP
        Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);


            // Liaison de la socket au point de communication
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));

            //************************************************************** Communications
            while (true)
            {
                Console.WriteLine("Attente d'une nouveau message...");

                // Reception message client
                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);

                byte[] buffer = new byte[ChatMessage.bufferSize];
                int nBytes = 0;
                try
                {
                    nBytes = serverSocket.ReceiveFrom(buffer, buffer.Length, SocketFlags.None, ref clientEP);
                }
                catch (SocketException E)
                {
                    Console.WriteLine(E.Message);
                    Console.ReadKey();
                }
                // Decodage du buffer de bytes en ASCII vers un string
                ChatMessage chatMsg = new ChatMessage(buffer);

                Console.WriteLine("Nouveau message recu de "
                    + clientEP
                    + " (" + nBytes + " octets)"
                    + ": \"" + chatMsg + "\"");

                chatMsg.commandeType = CommandeType.REPONSE;
                if (!subscribedUsers.Contains(clientEP))
                {
                    subscribedUsers.Add(clientEP);
                }
                if (chatMsg.commande != Commande.SUB)
                {
                    // Envoie du message à toutes les personnes qui ont SUBSCRIBE
                    foreach (EndPoint ep in subscribedUsers)
                    {
                        if (!ep.Equals(clientEP))
                        {
                            envoieMessage(chatMsg, serverSocket, ep);
                        }
                    }
                }
            }
            // Fermeture socket
            Console.WriteLine("Fermeture Socket...");
            serverSocket.Close();
            Console.ReadKey();
        }

        private static void envoieMessage(ChatMessage chatMsg, Socket serverSocket, EndPoint clientEP)
        {
            //************************************************************** Envoie du message

            // Encodage du string dans un buffer de bytes en ASCII
            int nBytes = 0;
            try
            {
                // Envoie du message au serveur
                nBytes = serverSocket.SendTo(chatMsg.GetBytes(), 0, chatMsg.GetBytes().Length, SocketFlags.None, clientEP);
                Console.WriteLine("Nouveau message envoye vers "
                    + clientEP
                    + " (" + nBytes + " octets)"
                    + ": \"" + chatMsg + "\"");
            }
            catch (SocketException E)
            {
                Console.WriteLine("erreur" + E.Message);
                Console.ReadKey();
            }
        }
    }
}
