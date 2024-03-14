namespace GBGR.Skill.Editor
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            SkillListComboBox = new ComboBox();
            DataGridView1 = new DataGridView();
            SaveButton = new Button();
            menuStrip1 = new MenuStrip();
            FileToolStripMenuItem = new ToolStripMenuItem();
            OpenToolStripMenuItem = new ToolStripMenuItem();
            SaveAsToolStripMenuItem = new ToolStripMenuItem();
            SkillLabel = new Label();
            LocaleList = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)DataGridView1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            //
            // SkillListComboBox
            //
            SkillListComboBox.FormattingEnabled = true;
            SkillListComboBox.Location = new Point(81, 28);
            SkillListComboBox.Name = "SkillListComboBox";
            SkillListComboBox.Size = new Size(208, 23);
            SkillListComboBox.TabIndex = 0;
            SkillListComboBox.SelectedValueChanged += SkillList_SelectedValueChanged;
            //
            // DataGridView1
            //
            DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridView1.Location = new Point(12, 56);
            DataGridView1.Name = "DataGridView1";
            DataGridView1.Size = new Size(776, 397);
            DataGridView1.TabIndex = 1;
            //
            // SaveButton
            //
            SaveButton.Location = new Point(295, 27);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 2;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            //
            // menuStrip1
            //
            menuStrip1.Items.AddRange(new ToolStripItem[] { FileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            //
            // FileToolStripMenuItem
            //
            FileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { OpenToolStripMenuItem, SaveAsToolStripMenuItem });
            FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            FileToolStripMenuItem.Size = new Size(37, 20);
            FileToolStripMenuItem.Text = "File";
            //
            // OpenToolStripMenuItem
            //
            OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            OpenToolStripMenuItem.Size = new Size(114, 22);
            OpenToolStripMenuItem.Text = "Open";
            OpenToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            //
            // SaveAsToolStripMenuItem
            //
            SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            SaveAsToolStripMenuItem.Size = new Size(114, 22);
            SaveAsToolStripMenuItem.Text = "Save As";
            SaveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
            //
            // SkillLabel
            //
            SkillLabel.AutoSize = true;
            SkillLabel.Location = new Point(12, 31);
            SkillLabel.Name = "SkillLabel";
            SkillLabel.Size = new Size(28, 15);
            SkillLabel.TabIndex = 4;
            SkillLabel.Text = "Skill";
            //
            // LocaleList
            //
            LocaleList.FormattingEnabled = true;
            LocaleList.Location = new Point(667, 28);
            LocaleList.Name = "LocaleList";
            LocaleList.Size = new Size(121, 23);
            LocaleList.TabIndex = 5;
            LocaleList.Text = "en-US";
            LocaleList.SelectedValueChanged += LocaleList_SelectedValueChanged;
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 464);
            Controls.Add(LocaleList);
            Controls.Add(SkillLabel);
            Controls.Add(SaveButton);
            Controls.Add(DataGridView1);
            Controls.Add(SkillListComboBox);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "GBFR Skill Editor";
            ((System.ComponentModel.ISupportInitialize)DataGridView1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox SkillListComboBox;
        private DataGridView DataGridView1;
        private Button SaveButton;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem FileToolStripMenuItem;
        private ToolStripMenuItem OpenToolStripMenuItem;
        private Label SkillLabel;
        private ComboBox LocaleList;
        private ToolStripMenuItem SaveAsToolStripMenuItem;
    }
}
