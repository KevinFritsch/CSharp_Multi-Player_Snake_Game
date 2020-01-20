using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ClientUdp
{
    class ClientUdp
    {

        private static Thread thread;
        private static bool continuer = true;
        private static bool portHasChange = false;
        private static int serverPort = 12345;

        static void Main(string[] args)
        {
            //************************************************************** Initialisation
            //adresse ip du serveur
            string serverIP = "169.254.227.20";
            // serverIP = "127.0.0.1";
            //port du serveur
            


            // Création de la socket d'écoute UDP
            Socket clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);


            // Liaison de la socket au point de communication
            clientSocket.Bind(new IPEndPoint(IPAddress.Any, 0));


            // Création du EndPoint 
            EndPoint serverEP = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

            while (continuer)
            {

                thread = new Thread(() => subscribeHandler(clientSocket, serverEP));
                thread.Start();
                portHasChange = false;

                while (!portHasChange && continuer)
                {
                    String pseudo = "";
                    String msg = "";
                    bool typeValide = false;
                    while (!typeValide)
                    {
                        // Lecture message au clavier selon le type de commande choisit
                        Console.Write("POST : 1 \n" +
                            "GET : 2 \n" +
                            "SUBSCRIBE : 3 \n" +
                            "UNSUBSCRIBE : 4 \n" +
                            "CREATEROOM : 5 \n" +
                            "LISTROOMS : 6 \n" +
                            "CHANGER DE PORT : 7 \n" +
                            "ARRET : 8\n");
                        Console.Write("Type de commande ? : \n");
                        string commande = Console.ReadLine();
                        typeValide = true;
                        ChatMessage chatMsg;

                        // Lecture de la commande choisit par le client
                        switch (commande)
                        {
                            //post
                            case "1":
                                Console.Write("Pseudo ? ");
                                pseudo = Console.ReadLine();
                                Console.Write("Message ? ");
                                msg = Console.ReadLine();

                                chatMsg = new ChatMessage(Commande.POST, CommandeType.REQUETE, msg, pseudo);

                                // Envoie du message
                                envoieMessage(chatMsg, clientSocket, serverEP);

                                // Reception de la réponse
                                //receptionMessage(clientSocket, serverEP);
                                break;
                            //get
                            case "2":

                                chatMsg = new ChatMessage(Commande.GET, CommandeType.REQUETE, msg, pseudo);

                                // Envoie du message
                                envoieMessage(chatMsg, clientSocket, serverEP);

                                // Reception de la réponse
                                //receptionMessage(clientSocket, serverEP);
                                break;
                            //subscribe
                            case "3":
                                chatMsg = new ChatMessage(Commande.SUBSCRIBE, CommandeType.REQUETE, msg, pseudo);

                                // Envoie du message
                                envoieMessage(chatMsg, clientSocket, serverEP);
                                break;
                            //unsubscribe
                            case "4":
                                chatMsg = new ChatMessage(Commande.UNSUBSCRIBE, CommandeType.REQUETE, msg, pseudo);

                                envoieMessage(chatMsg, clientSocket, serverEP);
                                break;
                            //createroom
                            case "5":
                                Console.Write("Pseudo ? ");
                                pseudo = Console.ReadLine();
                                Console.Write("Nom de la room ? ");
                                msg = Console.ReadLine();

                                chatMsg = new ChatMessage(Commande.CREATEROOM, CommandeType.REQUETE, msg, pseudo);

                                envoieMessage(chatMsg, clientSocket, serverEP);
                                break;
                            //listrooms
                            case "6":
                                chatMsg = new ChatMessage(Commande.LISTROOMS, CommandeType.REQUETE, msg, pseudo);

                                envoieMessage(chatMsg, clientSocket, serverEP);
                                break;
                            //changer de port
                            case "7":
                                Console.Write("Port ? ");
                                serverPort = int.Parse(Console.ReadLine());
                                serverEP = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
                                portHasChange = true;
                                break;
                            //quitter le serveur
                            case "8":
                                continuer = false;

                                chatMsg = new ChatMessage(Commande.STOPSERVEUR, CommandeType.REQUETE, msg, pseudo);

                                // Envoie du message
                                envoieMessage(chatMsg, clientSocket, serverEP);

                                // Reception de la réponse
                                receptionMessage(clientSocket, serverEP);

                                if (thread.ThreadState == ThreadState.Running)
                                {
                                    thread.Abort();
                                }
                                break;

                            default:
                                typeValide = false;
                                break;
                        }
                    }
                }
            }
            //************************************************************** Conclusion
            // Fermeture socket
            Console.WriteLine("Fermeture Socket...");
            clientSocket.Close();
            Console.ReadKey();
        }

        private static void subscribeHandler(Socket clientSocket, EndPoint serverEP)
        {
            while (continuer)
            {
                receptionMessage(clientSocket, serverEP);
            }
        }

        private static void receptionMessage(Socket clientSocket, EndPoint serverEP)
        {
            //************************************************************** Reception du message

            byte[] buffer = new byte[ChatMessage.bufferSize];
            int nBytes = 0;
            try
            {
                // Reception du message au serveur
                nBytes = clientSocket.ReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref serverEP);
                ChatMessage chatMsgRecu = new ChatMessage(buffer);
                //  Affichage dans la console du message recu
                Console.WriteLine("\nNouveau message recu de "
                + serverEP
                + " (" + nBytes + " octets)"
                + ": \"" + chatMsgRecu + "\"");
            }
            catch (SocketException E)
            {
                // Affichage de l'exception dans la console
                Console.WriteLine(E.Message);
                Console.ReadKey();
            }
        }

        private static void envoieMessage(ChatMessage chatMsg, Socket clientSocket, EndPoint serverEP)
        {
            //************************************************************** Envoie du message

            // Encodage du string dans un buffer de bytes en ASCII
            byte[] buffer = chatMsg.GetBytes();


            int nBytes = 0;
            try
            {
                // Envoie du message au serveur
                nBytes = clientSocket.SendTo(buffer, 0, buffer.Length, SocketFlags.None, serverEP);
            }
            catch (SocketException E)
            {
                // Affichage de l'exception dans la console
                Console.WriteLine(E.Message);
                Console.ReadKey();
            }

            // Affichage du message envoyé dans la console
            Console.WriteLine("Nouveau message envoye vers "
                + serverEP
                + " (" + nBytes + " octets)"
                + ": \"" + chatMsg + "\"");
        }
    }
}
