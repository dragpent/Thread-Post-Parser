namespace Thread_Post_Parser
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            lbUrl = new Label();
            txtUrl = new TextBox();
            btnFetch = new Button();
            btnStop = new Button();
            lbCookie = new Label();
            txtCookie = new TextBox();
            chkRemember = new CheckBox();
            lbCookieHelp = new LinkLabel();
            btnSignIn = new Button();
            btnCopyHtml = new Button();
            btnSave = new Button();
            dgData = new DataGridView();
            lbStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)dgData).BeginInit();
            SuspendLayout();
            //
            // lbUrl
            //
            lbUrl.AutoSize = true;
            lbUrl.Location = new Point(12, 15);
            lbUrl.Name = "lbUrl";
            lbUrl.Size = new Size(79, 20);
            lbUrl.TabIndex = 0;
            lbUrl.Text = "Topic URL";
            //
            // txtUrl
            //
            txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrl.Location = new Point(97, 12);
            txtUrl.Name = "txtUrl";
            txtUrl.PlaceholderText = "https://forums.d2jsp.org/topic.php?t=110498378&f=104";
            txtUrl.Size = new Size(485, 27);
            txtUrl.TabIndex = 1;
            //
            // btnFetch
            //
            btnFetch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFetch.Location = new Point(588, 11);
            btnFetch.Name = "btnFetch";
            btnFetch.Size = new Size(100, 29);
            btnFetch.TabIndex = 2;
            btnFetch.Text = "Fetch All";
            btnFetch.UseVisualStyleBackColor = true;
            btnFetch.Click += btnFetch_Click;
            //
            // btnStop
            //
            btnStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnStop.Enabled = false;
            btnStop.Location = new Point(694, 11);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(94, 29);
            btnStop.TabIndex = 3;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            //
            // lbCookie
            //
            lbCookie.AutoSize = true;
            lbCookie.Location = new Point(12, 48);
            lbCookie.Name = "lbCookie";
            lbCookie.Size = new Size(55, 20);
            lbCookie.TabIndex = 4;
            lbCookie.Text = "Cookie";
            //
            // txtCookie
            //
            txtCookie.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtCookie.Location = new Point(97, 45);
            txtCookie.Name = "txtCookie";
            txtCookie.PlaceholderText = "paste the Cookie header from Chrome DevTools (or sign in below instead)";
            txtCookie.Size = new Size(485, 27);
            txtCookie.TabIndex = 5;
            txtCookie.UseSystemPasswordChar = true;
            //
            // chkRemember
            //
            chkRemember.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkRemember.AutoSize = true;
            chkRemember.Checked = true;
            chkRemember.CheckState = CheckState.Checked;
            chkRemember.Location = new Point(588, 47);
            chkRemember.Name = "chkRemember";
            chkRemember.Size = new Size(104, 24);
            chkRemember.TabIndex = 6;
            chkRemember.Text = "Remember";
            chkRemember.UseVisualStyleBackColor = true;
            //
            // lbCookieHelp
            //
            lbCookieHelp.AutoSize = true;
            lbCookieHelp.Location = new Point(97, 75);
            lbCookieHelp.Name = "lbCookieHelp";
            lbCookieHelp.Size = new Size(181, 20);
            lbCookieHelp.TabIndex = 7;
            lbCookieHelp.TabStop = true;
            lbCookieHelp.Text = "How do I get the cookie?";
            lbCookieHelp.LinkClicked += lbCookieHelp_LinkClicked;
            //
            // btnSignIn
            //
            btnSignIn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSignIn.Location = new Point(458, 72);
            btnSignIn.Name = "btnSignIn";
            btnSignIn.Size = new Size(172, 29);
            btnSignIn.TabIndex = 8;
            btnSignIn.Text = "Open browser / sign in";
            btnSignIn.UseVisualStyleBackColor = true;
            btnSignIn.Click += btnSignIn_Click;
            //
            // btnCopyHtml
            //
            btnCopyHtml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopyHtml.Location = new Point(636, 72);
            btnCopyHtml.Name = "btnCopyHtml";
            btnCopyHtml.Size = new Size(152, 29);
            btnCopyHtml.TabIndex = 9;
            btnCopyHtml.Text = "Paste HTML instead";
            btnCopyHtml.UseVisualStyleBackColor = true;
            btnCopyHtml.Click += btnCopyHtml_Click;
            //
            // dgData
            //
            dgData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgData.Location = new Point(12, 110);
            dgData.Name = "dgData";
            dgData.RowHeadersWidth = 51;
            dgData.Size = new Size(776, 328);
            dgData.TabIndex = 10;
            //
            // lbStatus
            //
            lbStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbStatus.AutoEllipsis = true;
            lbStatus.Location = new Point(12, 452);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new Size(570, 25);
            lbStatus.TabIndex = 11;
            lbStatus.Text = "Paste a topic URL and your cookie, then Fetch All.";
            //
            // btnSave
            //
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(588, 448);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(200, 29);
            btnSave.TabIndex = 12;
            btnSave.Text = "Save user lists";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            //
            // Main
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 489);
            Controls.Add(btnSave);
            Controls.Add(lbStatus);
            Controls.Add(dgData);
            Controls.Add(btnCopyHtml);
            Controls.Add(btnSignIn);
            Controls.Add(lbCookieHelp);
            Controls.Add(chkRemember);
            Controls.Add(txtCookie);
            Controls.Add(lbCookie);
            Controls.Add(btnStop);
            Controls.Add(btnFetch);
            Controls.Add(txtUrl);
            Controls.Add(lbUrl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Main";
            Text = "Thread Parser";
            ((System.ComponentModel.ISupportInitialize)dgData).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbUrl;
        private TextBox txtUrl;
        private Button btnFetch;
        private Button btnStop;
        private Label lbCookie;
        private TextBox txtCookie;
        private CheckBox chkRemember;
        private LinkLabel lbCookieHelp;
        private Button btnSignIn;
        private Button btnCopyHtml;
        private Button btnSave;
        private DataGridView dgData;
        private Label lbStatus;
    }
}
