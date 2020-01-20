using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientUdp
{
    public enum Commande
    {
        POST, GET, HELP, QUIT, STOPSERVEUR, SUBSCRIBE, SUBSCRIBEv2, UNSUBSCRIBE, CREATEROOM, LISTROOMS, SIGNALER, LISTESERVEURS
    };

    public enum CommandeType { REQUETE, REPONSE };

    class ChatMessage
    {
        public const int bufferSize = 150;

        // Taille des données : 21

        public Commande commande;               // commande
        public CommandeType commandeType;       // type (Requête/Réponse)
        public int dataSize;                    // taille de la donnée
        public String data;                     // données de la commande
        public String pseudo;                   // pseudo de taille max 15

        //Methode qui permet de créer un nouveau message à partir des informations
        public ChatMessage(Commande commande, CommandeType type, String data, String pseudo)
        {
            this.commande = commande;
            this.commandeType = type;

            // Si le message est trop long, on le découpe
            if (data.Length < bufferSize - 21)
            {
                this.dataSize = data.Length;
                this.data = data;
            }
            else
            {
                this.dataSize = bufferSize - 21;
                this.data = data.Substring(0, bufferSize - 21);
            }

            // Si le pseudo est trop long, on le découpe
            if (pseudo.Length < 15)
            {
                this.pseudo = pseudo;
            }
            else
            {
                this.pseudo = pseudo.Substring(0, 15);
            }
        }

        public ChatMessage(byte[] buffer)
        {
            // Encodage
            this.commande = (Commande)buffer[0];
            this.commandeType = (CommandeType)buffer[1];
            this.pseudo = Encoding.ASCII.GetString(buffer, 2, 15).TrimEnd('\0');
            this.dataSize = BitConverter.ToInt32(buffer, 17);
            this.data = Encoding.ASCII.GetString(buffer, 21, this.dataSize);
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[this.dataSize + 21]; //Création d'un buffer
            buffer[0] = (byte)this.commande; //commande
            buffer[1] = (byte)this.commandeType; //type de la commande
            Encoding.ASCII.GetBytes(this.pseudo, 0, this.pseudo.Length, buffer, 2);
            byte[] intbytes = BitConverter.GetBytes(this.dataSize); //on récupère la taille du message
            buffer[17] = intbytes[0];
            buffer[18] = intbytes[1];
            buffer[19] = intbytes[2];
            buffer[20] = intbytes[3];

            Encoding.ASCII.GetBytes(data, 0, dataSize, buffer, 21); //Buffer

            return buffer;
        }

        public override string ToString()
        {
            return "[" + commande + "," + commandeType + ",\"" + pseudo + "\"," + dataSize + ",\"" + data + "\"]";
        }

    }
}
