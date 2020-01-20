using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TronServeur
{
    class Program
    {
        static void Main(string[] args)
        {
            ServeurUdp.ServeurUdp.StartChat();

            Tron.Tron myTron;            // Moteur du jeu

            byte nJoueurs = 2;      // Nombre de joueurs
            byte frequence = 1;    // Temps du tour de jeu (en dixieme de s)
            byte taille = 60;       // Taille du terrain

            // ************************************* Intitialisation partie
            System.Console.WriteLine("Initialisation");

            // TODO Creation de la socket d'écoute TCP
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // TODO Bind et listen
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 23232));
            serverSocket.Listen(nJoueurs);
            while (1 == 1)
            {
                // TODO Creation du tableau des sockets connectées
                List<Socket> clients = new List<Socket>();

                // Creation du moteur de jeu
                myTron = new Tron.Tron(nJoueurs, taille);

                // TODO Acceptation des clients
                for (int i = 0; i < nJoueurs; i++)
                {
                    clients.Add(serverSocket.Accept());
                    Console.WriteLine("Connexion de " + clients[i].RemoteEndPoint);
                }

                // TODO Envoie des paramètres

                byte[] parametres = new byte[4];
                parametres[1] = nJoueurs;
                parametres[2] = frequence;
                parametres[3] = taille;

                for (int i = 0; i < nJoueurs; i++)
                {
                    parametres[0] = (byte)i;
                    clients[i].Send(parametres, parametres.Length, SocketFlags.None);
                }

                // ************************************* Routine à chaque tour
                System.Console.WriteLine("Routine");

                // Tant que la partie n'est pas finie
                while (!myTron.IsFinished())
                {
                    // TODO Réception de la direction de chaque joueur
                    byte[] directions = new byte[clients.Count];
                    byte[] receive = new byte[1];
                    for (int i = 0; i < clients.Count; i++)
                    {
                        try
                        {
                            if (clients[i].Connected)
                            {
                                clients[i].Receive(receive, receive.Length, SocketFlags.None);
                                directions[i] = receive[0];
                            }
                            else
                            {
                                clients[i].Close();
                                // Perdu
                                directions[i] = 5;
                            }
                        }
                        catch (Exception e)
                        {
                            clients[i].Close();
                            // Perdu
                            directions[i] = 5;
                        }

                    }
                    // TODO Calcul collision : myTron.Collision(byte[] <toutes les directions>);

                    myTron.Collision(directions);

                    // TODO Envoie des directions de tous les joueurs à tous les clients

                    foreach (Socket s in clients)
                    {
                        try
                        {
                            if (s.Connected)
                            {
                                s.Send(directions, directions.Length, SocketFlags.None);
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }


                // ************************************* Conclusion
                System.Console.WriteLine("Conclusion");

                // TODO Fermeture des sockets connectées

                foreach (Socket s in clients)
                {
                    if (s.Connected)
                    {
                        s.Close();
                    }
                }
            }
            // TODO Fermeture socket d'écoute
            serverSocket.Close();
            ServeurUdp.ServeurUdp.thread.Abort();
        }
    }
}
