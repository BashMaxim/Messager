namespace Client
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.SearchLabel = new System.Windows.Forms.Label();
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.SearchPanel = new System.Windows.Forms.Panel();
            this.SearchResultFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ChatPanel = new System.Windows.Forms.Panel();
            this.MessagesZonePanel = new System.Windows.Forms.Panel();
            this.MessagesFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.EnterMessageZonePanel = new System.Windows.Forms.Panel();
            this.MessageTextBox = new System.Windows.Forms.RichTextBox();
            this.UsernamePanel = new System.Windows.Forms.Panel();
            this.ChatUserNameLabel = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SearchPanel.SuspendLayout();
            this.ChatPanel.SuspendLayout();
            this.MessagesZonePanel.SuspendLayout();
            this.EnterMessageZonePanel.SuspendLayout();
            this.UsernamePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SearchLabel
            // 
            this.SearchLabel.AutoSize = true;
            this.SearchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchLabel.Location = new System.Drawing.Point(3, 9);
            this.SearchLabel.Name = "SearchLabel";
            this.SearchLabel.Size = new System.Drawing.Size(47, 16);
            this.SearchLabel.TabIndex = 0;
            this.SearchLabel.Text = "Поиск";
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.Location = new System.Drawing.Point(56, 9);
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(250, 20);
            this.SearchTextBox.TabIndex = 1;
            this.SearchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchTextBox_KeyDown);
            // 
            // SearchPanel
            // 
            this.SearchPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SearchPanel.Controls.Add(this.SearchResultFlowPanel);
            this.SearchPanel.Controls.Add(this.SearchTextBox);
            this.SearchPanel.Controls.Add(this.SearchLabel);
            this.SearchPanel.Location = new System.Drawing.Point(12, 12);
            this.SearchPanel.Name = "SearchPanel";
            this.SearchPanel.Size = new System.Drawing.Size(327, 617);
            this.SearchPanel.TabIndex = 2;
            // 
            // SearchResultFlowPanel
            // 
            this.SearchResultFlowPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SearchResultFlowPanel.Location = new System.Drawing.Point(6, 35);
            this.SearchResultFlowPanel.Name = "SearchResultFlowPanel";
            this.SearchResultFlowPanel.Size = new System.Drawing.Size(314, 577);
            this.SearchResultFlowPanel.TabIndex = 2;
            // 
            // ChatPanel
            // 
            this.ChatPanel.Controls.Add(this.MessagesZonePanel);
            this.ChatPanel.Controls.Add(this.EnterMessageZonePanel);
            this.ChatPanel.Controls.Add(this.UsernamePanel);
            this.ChatPanel.Location = new System.Drawing.Point(345, 12);
            this.ChatPanel.Name = "ChatPanel";
            this.ChatPanel.Size = new System.Drawing.Size(647, 614);
            this.ChatPanel.TabIndex = 3;
            // 
            // MessagesZonePanel
            // 
            this.MessagesZonePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MessagesZonePanel.Controls.Add(this.MessagesFlowPanel);
            this.MessagesZonePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessagesZonePanel.Location = new System.Drawing.Point(0, 38);
            this.MessagesZonePanel.Name = "MessagesZonePanel";
            this.MessagesZonePanel.Size = new System.Drawing.Size(647, 476);
            this.MessagesZonePanel.TabIndex = 2;
            // 
            // MessagesFlowPanel
            // 
            this.MessagesFlowPanel.AutoScroll = true;
            this.MessagesFlowPanel.Location = new System.Drawing.Point(156, 6);
            this.MessagesFlowPanel.Name = "MessagesFlowPanel";
            this.MessagesFlowPanel.Size = new System.Drawing.Size(488, 464);
            this.MessagesFlowPanel.TabIndex = 0;
            // 
            // EnterMessageZonePanel
            // 
            this.EnterMessageZonePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EnterMessageZonePanel.Controls.Add(this.MessageTextBox);
            this.EnterMessageZonePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.EnterMessageZonePanel.Location = new System.Drawing.Point(0, 514);
            this.EnterMessageZonePanel.Name = "EnterMessageZonePanel";
            this.EnterMessageZonePanel.Size = new System.Drawing.Size(647, 100);
            this.EnterMessageZonePanel.TabIndex = 1;
            // 
            // MessageTextBox
            // 
            this.MessageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MessageTextBox.Location = new System.Drawing.Point(23, 15);
            this.MessageTextBox.Multiline = false;
            this.MessageTextBox.Name = "MessageTextBox";
            this.MessageTextBox.Size = new System.Drawing.Size(600, 69);
            this.MessageTextBox.TabIndex = 0;
            this.MessageTextBox.Text = "";
            this.MessageTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MessageTextBox_KeyDown);
            // 
            // UsernamePanel
            // 
            this.UsernamePanel.Controls.Add(this.ChatUserNameLabel);
            this.UsernamePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.UsernamePanel.Location = new System.Drawing.Point(0, 0);
            this.UsernamePanel.Name = "UsernamePanel";
            this.UsernamePanel.Size = new System.Drawing.Size(647, 38);
            this.UsernamePanel.TabIndex = 0;
            // 
            // ChatUserNameLabel
            // 
            this.ChatUserNameLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ChatUserNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChatUserNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ChatUserNameLabel.Location = new System.Drawing.Point(0, 0);
            this.ChatUserNameLabel.Name = "ChatUserNameLabel";
            this.ChatUserNameLabel.Size = new System.Drawing.Size(647, 38);
            this.ChatUserNameLabel.TabIndex = 0;
            this.ChatUserNameLabel.Text = "label1";
            this.ChatUserNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 641);
            this.Controls.Add(this.ChatPanel);
            this.Controls.Add(this.SearchPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.SearchPanel.ResumeLayout(false);
            this.SearchPanel.PerformLayout();
            this.ChatPanel.ResumeLayout(false);
            this.MessagesZonePanel.ResumeLayout(false);
            this.EnterMessageZonePanel.ResumeLayout(false);
            this.UsernamePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label SearchLabel;
        private System.Windows.Forms.TextBox SearchTextBox;
        private System.Windows.Forms.Panel SearchPanel;
        public System.Windows.Forms.FlowLayoutPanel SearchResultFlowPanel;
        private System.Windows.Forms.Panel ChatPanel;
        private System.Windows.Forms.Panel UsernamePanel;
        private System.Windows.Forms.Label ChatUserNameLabel;
        private System.Windows.Forms.Panel MessagesZonePanel;
        private System.Windows.Forms.FlowLayoutPanel MessagesFlowPanel;
        private System.Windows.Forms.Panel EnterMessageZonePanel;
        private System.Windows.Forms.RichTextBox MessageTextBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}