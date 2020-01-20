using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TronClient
{
    public class Client
    {
        private Tron.Tron myTron;        // Moteur du jeu

        public byte frequence;      // Temps du tour de jeu (en dixieme de s)

        IPEndPoint serverEP;
        Socket clientSocket;

        // constructeur : IP/Port du serveur
        public Client(String myServerIP)
        {
            // TODO : Creation de la socket d'écoute TCP
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            serverEP = new IPEndPoint(IPAddress.Parse(myServerIP), 23232);
        }


        // Appelé au début de la partie
        public Tron.Tron Init()
        {
            System.Console.WriteLine("Init");

            // TODO Connexion au serveur 
            clientSocket.Connect(serverEP);
            // TODO Réception des paramètres
            byte[] receive = new byte[4];
            clientSocket.Receive(receive, receive.Length, SocketFlags.None);
            
            // TODO Initialisation de la fréquence : frequence = <frequence>
            frequence = receive[2];

            // TODO Initialisation du moteur : myTron = new Tron(byte <taille terrain>, byte <nombre de joueurs>, byte <numéro du joueur>);
            myTron = new Tron.Tron(receive[3], receive[1], receive[0]);

            // Retourne le moteur
            return myTron;
        }

        // Appelé régulièrement à chaque tour de jeu
        public void Routine()
        {
            System.Console.WriteLine("Routine");

            // TODO Envoie de sa direction : myTron.getDirection()
            byte[] directionSend = new byte[1];
            directionSend[0] = myTron.getDirection();
            try
            {
                if (clientSocket.Connected)
                {
                    clientSocket.Send(directionSend, directionSend.Length, SocketFlags.None);
                }
            } catch
            {
                byte[] directions = myTron.getDirections();
                directions[myTron.getMonNum()] = 5;
                myTron.setDirections(directions);
                clientSocket.Close();
            }
            // TOSO Reception de toutes les directions : myTron.setDirections(byte[] < toutes les directions>);

            byte[] directionsReceive = new byte[myTron.getNJoueurs()];
            try
            {
                if (clientSocket.Connected)
                {
                    clientSocket.Receive(directionsReceive, directionsReceive.Length, SocketFlags.None);
                }
            } catch
            {
                byte[] directions = myTron.getDirections();
                directions[myTron.getMonNum()] = 5;
                myTron.setDirections(directions);
                clientSocket.Close();
            }
            myTron.setDirections(directionsReceive);
        }

        // Appelé à la fin de la partie
        public void Conclusion()
        {
            System.Console.WriteLine("Conclusion");

            // fermeture socket
            clientSocket.Close();
        }


        // propriété frequence (Ne pas toucher)
        public int freq
        {
            get { return frequence * 100; }
            set { }
        }
    }
}
