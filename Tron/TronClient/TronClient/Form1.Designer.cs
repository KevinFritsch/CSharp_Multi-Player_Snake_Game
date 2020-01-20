namespace TronClient
{
    partial class formChat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonEnvoyer = new System.Windows.Forms.Button();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.lblChat = new System.Windows.Forms.Label();
            this.textBoxPseudo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonEnvoyer
            // 
            this.buttonEnvoyer.Location = new System.Drawing.Point(316, 416);
            this.buttonEnvoyer.Name = "buttonEnvoyer";
            this.buttonEnvoyer.Size = new System.Drawing.Size(75, 23);
            this.buttonEnvoyer.TabIndex = 3;
            this.buttonEnvoyer.Text = "Envoyer";
            this.buttonEnvoyer.UseVisualStyleBackColor = true;
            this.buttonEnvoyer.Click += new System.EventHandler(this.buttonEnvoyer_Click);
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(102, 418);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(208, 20);
            this.textBoxMessage.TabIndex = 2;
            this.textBoxMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // lblChat
            // 
            this.lblChat.AutoSize = true;
            this.lblChat.Location = new System.Drawing.Point(12, 9);
            this.lblChat.Name = "lblChat";
            this.lblChat.Size = new System.Drawing.Size(0, 13);
            this.lblChat.TabIndex = 2;
            // 
            // textBoxPseudo
            // 
            this.textBoxPseudo.Location = new System.Drawing.Point(12, 418);
            this.textBoxPseudo.Name = "textBoxPseudo";
            this.textBoxPseudo.Size = new System.Drawing.Size(84, 20);
            this.textBoxPseudo.TabIndex = 1;
            // 
            // formChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 450);
            this.Controls.Add(this.textBoxPseudo);
            this.Controls.Add(this.lblChat);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.buttonEnvoyer);
            this.Name = "formChat";
            this.Text = "Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formChat_FormClosing);
            this.Load += new System.EventHandler(this.formChat_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonEnvoyer;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Label lblChat;
        private System.Windows.Forms.TextBox textBoxPseudo;
    }
}