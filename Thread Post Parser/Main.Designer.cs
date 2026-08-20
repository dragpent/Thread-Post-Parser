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
            btnCopyHtml = new Button();
            btnSave = new Button();
            dgData = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgData).BeginInit();
            SuspendLayout();
            // 
            // btnCopyHtml
            // 
            btnCopyHtml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopyHtml.Location = new Point(694, 12);
            btnCopyHtml.Name = "btnCopyHtml";
            btnCopyHtml.Size = new Size(94, 29);
            btnCopyHtml.TabIndex = 0;
            btnCopyHtml.Text = "Copy HTML";
            btnCopyHtml.UseVisualStyleBackColor = true;
            btnCopyHtml.Click += btnCopyHtml_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(694, 409);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(94, 29);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // dgData
            // 
            dgData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgData.Location = new Point(12, 12);
            dgData.Name = "dgData";
            dgData.RowHeadersWidth = 51;
            dgData.Size = new Size(676, 426);
            dgData.TabIndex = 2;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgData);
            Controls.Add(btnSave);
            Controls.Add(btnCopyHtml);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Main";
            Text = "Thread Parser";
            ((System.ComponentModel.ISupportInitialize)dgData).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnCopyHtml;
        private Button btnSave;
        private DataGridView dgData;
    }
}
