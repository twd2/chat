namespace ChatClient
{
    partial class ChatForm
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
            this.components = new System.ComponentModel.Container();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.txtSession = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnFile = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnCanvas = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtSend
            // 
            this.txtSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSend.Location = new System.Drawing.Point(12, 285);
            this.txtSend.Multiline = true;
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(297, 51);
            this.txtSend.TabIndex = 0;
            this.txtSend.TextChanged += new System.EventHandler(this.txtSend_TextChanged);
            // 
            // txtSession
            // 
            this.txtSession.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSession.BackColor = System.Drawing.Color.White;
            this.txtSession.Location = new System.Drawing.Point(12, 44);
            this.txtSession.Multiline = true;
            this.txtSession.Name = "txtSession";
            this.txtSession.ReadOnly = true;
            this.txtSession.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSession.Size = new System.Drawing.Size(371, 235);
            this.txtSession.TabIndex = 1;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(315, 285);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(68, 51);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "发送(&S)";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(12, 12);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(127, 26);
            this.btnFile.TabIndex = 3;
            this.btnFile.Text = "发送文件(&F)";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnCanvas
            // 
            this.btnCanvas.Location = new System.Drawing.Point(145, 12);
            this.btnCanvas.Name = "btnCanvas";
            this.btnCanvas.Size = new System.Drawing.Size(127, 26);
            this.btnCanvas.TabIndex = 4;
            this.btnCanvas.Text = "共享画板(&C)";
            this.btnCanvas.UseVisualStyleBackColor = true;
            this.btnCanvas.Click += new System.EventHandler(this.btnCanvas_Click);
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 348);
            this.Controls.Add(this.btnCanvas);
            this.Controls.Add(this.btnFile);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtSession);
            this.Controls.Add(this.txtSend);
            this.Name = "ChatForm";
            this.Text = "与xxxx的聊天 - 正在输入...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChatForm_FormClosing);
            this.Load += new System.EventHandler(this.ChatForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.TextBox txtSession;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnCanvas;
    }
}