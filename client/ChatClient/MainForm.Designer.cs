namespace ChatClient
{
    partial class MainForm
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
            this.btnListUser = new System.Windows.Forms.Button();
            this.lstBuddies = new System.Windows.Forms.ListBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.labHello = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnListUser
            // 
            this.btnListUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnListUser.Location = new System.Drawing.Point(146, 10);
            this.btnListUser.Margin = new System.Windows.Forms.Padding(2);
            this.btnListUser.Name = "btnListUser";
            this.btnListUser.Size = new System.Drawing.Size(64, 26);
            this.btnListUser.TabIndex = 0;
            this.btnListUser.Text = "添加好友";
            this.btnListUser.UseVisualStyleBackColor = true;
            this.btnListUser.Click += new System.EventHandler(this.btnListUser_Click);
            // 
            // lstBuddies
            // 
            this.lstBuddies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBuddies.FormattingEnabled = true;
            this.lstBuddies.ItemHeight = 12;
            this.lstBuddies.Location = new System.Drawing.Point(11, 40);
            this.lstBuddies.Margin = new System.Windows.Forms.Padding(2);
            this.lstBuddies.Name = "lstBuddies";
            this.lstBuddies.Size = new System.Drawing.Size(267, 196);
            this.lstBuddies.TabIndex = 3;
            this.lstBuddies.DoubleClick += new System.EventHandler(this.lstBuddies_DoubleClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(214, 11);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(2);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(64, 25);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // labHello
            // 
            this.labHello.AutoSize = true;
            this.labHello.Location = new System.Drawing.Point(12, 17);
            this.labHello.Name = "labHello";
            this.labHello.Size = new System.Drawing.Size(41, 12);
            this.labHello.TabIndex = 5;
            this.labHello.Text = "label1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 252);
            this.Controls.Add(this.labHello);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lstBuddies);
            this.Controls.Add(this.btnListUser);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnListUser;
        private System.Windows.Forms.ListBox lstBuddies;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label labHello;
    }
}