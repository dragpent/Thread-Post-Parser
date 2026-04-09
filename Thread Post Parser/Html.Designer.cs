namespace Thread_Post_Parser
{
    partial class Html
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
            htmlTextBox = new RichTextBox();
            btnSaveAndClose = new Button();
            btnSaveAndAdd = new Button();
            lbCount = new Label();
            SuspendLayout();
            // 
            // htmlTextBox
            // 
            htmlTextBox.Location = new Point(12, 12);
            htmlTextBox.Name = "htmlTextBox";
            htmlTextBox.Size = new Size(776, 385);
            htmlTextBox.TabIndex = 1;
            htmlTextBox.Text = "";
            // 
            // btnSaveAndClose
            // 
            btnSaveAndClose.Location = new Point(670, 409);
            btnSaveAndClose.Name = "btnSaveAndClose";
            btnSaveAndClose.Size = new Size(118, 29);
            btnSaveAndClose.TabIndex = 2;
            btnSaveAndClose.Text = "Save and Close";
            btnSaveAndClose.UseVisualStyleBackColor = true;
            btnSaveAndClose.Click += btnSaveAndClose_Click;
            // 
            // btnSaveAndAdd
            // 
            btnSaveAndAdd.Location = new Point(547, 409);
            btnSaveAndAdd.Name = "btnSaveAndAdd";
            btnSaveAndAdd.Size = new Size(117, 29);
            btnSaveAndAdd.TabIndex = 3;
            btnSaveAndAdd.Text = "Save and Add";
            btnSaveAndAdd.UseVisualStyleBackColor = true;
            btnSaveAndAdd.Click += btnSaveAndAdd_Click;
            // 
            // lbCount
            // 
            lbCount.AutoSize = true;
            lbCount.Location = new Point(12, 413);
            lbCount.Name = "lbCount";
            lbCount.Size = new Size(50, 20);
            lbCount.TabIndex = 4;
            lbCount.Text = "label1";
            lbCount.Visible = false;
            // 
            // Html
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lbCount);
            Controls.Add(btnSaveAndAdd);
            Controls.Add(btnSaveAndClose);
            Controls.Add(htmlTextBox);
            Name = "Html";
            Text = "Html";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox htmlTextBox;
        private Button btnSaveAndClose;
        private Button btnSaveAndAdd;
        private Label lbCount;
    }
}