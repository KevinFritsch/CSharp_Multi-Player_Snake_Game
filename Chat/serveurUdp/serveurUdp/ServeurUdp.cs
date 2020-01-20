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

        static void Main(string[] args)
        {
            traitement(serverPort);
        }

        static void traitement(int port)
        {
            // ************************************************************** Initialisation

            string historique = "";
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
            while (continuer)
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

                switch (chatMsg.commande)
                {
                    case Commande.POST:
                        historique += chatMsg.pseudo + ":" + chatMsg.data + "\n";
                        envoieMessage(chatMsg, serverSocket, clientEP);
                        // Envoie du message à toutes les personnes qui ont SUBSCRIBE
                        foreach (EndPoint ep in subscribedUsers)
                        {
                            if (!ep.Equals(clientEP))
                            {
                                envoieMessage(chatMsg, serverSocket, ep);
                            }
                        }
                        break;
                    case Commande.GET:
                        // Creation du message affichant l'historique
                        if (historique.Length < ChatMessage.bufferSize - 21)
                        {
                            chatMsg.dataSize = historique.Length;
                            chatMsg.data = historique;
                        }
                        else
                        {
                            chatMsg.dataSize = ChatMessage.bufferSize - 21;
                            try
                            {
                                chatMsg.data = "..." + historique.Substring(historique.Length - ChatMessage.bufferSize + 21 + 3, ChatMessage.bufferSize - 21 - 3);
                            } catch(ArgumentOutOfRangeException E)
                            {
                                Console.WriteLine(E.Message);
                                Console.ReadKey();
                            }
                        }
                        // Envoie de l'historique au client
                        envoieMessage(chatMsg, serverSocket, clientEP);
                        break;
                    case Commande.SUBSCRIBE:
                        // Verifie si un client n'est pas déjà SUB
                        if(!subscribedUsers.Contains(clientEP)) {
                            subscribedUsers.Add(clientEP);
                            string reponse = "Abonnement effectue";
                            chatMsg.data = reponse;
                            chatMsg.dataSize = reponse.Length;
                        } else
                        {
                            string reponse = "Vous ne pouvez pas vous abonner";
                            chatMsg.data = reponse;
                            chatMsg.dataSize = reponse.Length;
                        }
                        // Notifie le client
                        envoieMessage(chatMsg, serverSocket, clientEP);
                        break;
                    case Commande.UNSUBSCRIBE:
                        // Vérifie que le client est bien SUB, sinon il ne peut pas se UNSUB
                        if (subscribedUsers.Contains(clientEP))
                        {
                            subscribedUsers.Remove(clientEP);
                            string reponse = "Desabonnement effectue";
                            chatMsg.data = reponse;
                            chatMsg.dataSize = reponse.Length;
                        } else
                        {
                            string reponse = "Vous ne pouvez pas vous desabonner";
                            chatMsg.data = reponse;
                            chatMsg.dataSize = reponse.Length;
                        }
                        // Notifie le client
                        envoieMessage(chatMsg, serverSocket, clientEP);
                        break;
                    case Commande.CREATEROOM:
                        string nomRoom = chatMsg.data;
                        serverPort++;
                        roomList[nomRoom] = new Thread(() => traitement(serverPort));
                        roomList[nomRoom].Start();
                        chatMsg.data = "Room " + nomRoom + " créée. PORT: " + serverPort;
                        chatMsg.dataSize = chatMsg.data.Length;
                        envoieMessage(chatMsg, serverSocket, clientEP);
                        break;
                    case Commande.LISTROOMS:

                        break;
                    case Commande.STOPSERVEUR:
                        continuer = false;
                        envoieMessage(chatMsg, serverSocket, clientEP);
                        break;
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
