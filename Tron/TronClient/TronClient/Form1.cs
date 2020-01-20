using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using ClientUdp;

namespace TronClient
{
    public partial class formChat : Form
    {
        string ip = "";

        public formChat(string ip)
        {
            InitializeComponent();
            // On récupère l'ip choisi
            this.ip = ip;
        }

        private void formChat_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //adresse ip du serveur
            string serverIP = this.ip;

            // Création de la socket d'écoute UDP
            clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);


            // Liaison de la socket au point de communication
            clientSocket.Bind(new IPEndPoint(IPAddress.Any, 0));


            // Création du EndPoint 
            serverEP = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            ChatMessage chatMsg = new ChatMessage(Commande.SUB, CommandeType.REQUETE, "", "");
            envoieMessage(chatMsg, clientSocket, serverEP);
            thread = new Thread(() => subscribeHandler(clientSocket, serverEP));
            thread.Start();

        }

        private void formChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread.Abort();
        }

        public void recevoirMessage(string pseudo, string message)
        {
            lblChat.Text += pseudo + ": " + message + "\n";
        }

        public void envoyerMessage()
        {
            textBoxPseudo.ReadOnly = true;
            ChatMessage chatMsg = new ChatMessage(Commande.POST, CommandeType.REQUETE, textBoxMessage.Text, textBoxPseudo.Text);
            recevoirMessage(textBoxPseudo.Text, textBoxMessage.Text);
            textBoxMessage.Text = "";
            // Envoie du message
            envoieMessage(chatMsg, clientSocket, serverEP);
        }

        private void buttonEnvoyer_Click(object sender, EventArgs e)
        {
            envoyerMessage();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar.Equals((char) Keys.Return))
            {
                envoyerMessage();
            }
        }

        private void receptionMessage(Socket clientSocket, EndPoint serverEP)
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

                recevoirMessage(chatMsgRecu.pseudo, chatMsgRecu.data);
            }
            catch (SocketException E)
            {
                // Affichage de l'exception dans la console
                Console.WriteLine(E.Message);
            }
        }

        private void envoieMessage(ChatMessage chatMsg, Socket clientSocket, EndPoint serverEP)
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
            }

            // Affichage du message envoyé dans la console
            Console.WriteLine("Nouveau message envoye vers "
                + serverEP
                + " (" + nBytes + " octets)"
                + ": \"" + chatMsg + "\"");
        }

        private Thread thread;
        private bool continuer = true;
        private int serverPort = 12345;
        private Socket clientSocket;
        private EndPoint serverEP;

        private void subscribeHandler(Socket clientSocket, EndPoint serverEP)
        {
            while (continuer)
            {
                receptionMessage(clientSocket, serverEP);
            }
        }
    }
}
