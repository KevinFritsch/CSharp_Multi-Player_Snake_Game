using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TronClient
{
    // Form du lobby
    public partial class FormLobby : Form
    {
        Client myClient;
        formChat myFormChat;

        public FormLobby()
        {
            InitializeComponent();
        }

        // Lance la partie
        private void StartTron()
        {
            // Création du client avec IP et Port
                myClient = new Client(textBoxIP.Text);

                // Création et affichage de la Form de jeu
                FormTron myFormTron = new FormTron(myClient);
                myFormTron.Show();
        }

        // Lane le chat
        private void StartChat()
        {
            myFormChat = new formChat(textBoxIP.Text);
            myFormChat.Show();
        }

        // Appelé au click sur le bouton start
        private void button1_Click(object sender, EventArgs e)
        {
            // Lance la partie
            StartTron();
        }

        private void buttonChat_Click(object sender, EventArgs e)
        {
            // Lance le chat
            StartChat();
        }

        private void FormLobby_Load(object sender, EventArgs e)
        {

        }

        private void FormLobby_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(myFormChat != null)
            myFormChat.Close();
        }
    }
}
