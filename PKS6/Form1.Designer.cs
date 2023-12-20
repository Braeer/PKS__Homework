namespace ПКС6
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
            dataGridView1 = new DataGridView();
            файлToolStripMenuItem = new ToolStripMenuItem();
            btnOpenFile = new ToolStripMenuItem();
            btnExport = new ToolStripMenuItem();
            menuStrip1 = new MenuStrip();
            txtSearchLastName = new TextBox();
            btnSearch = new Button();
            Task3 = new TextBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(0, 31);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(828, 425);
            dataGridView1.TabIndex = 1;
            // 
            // файлToolStripMenuItem
            // 
            файлToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { btnOpenFile, btnExport });
            файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            файлToolStripMenuItem.Size = new Size(59, 24);
            файлToolStripMenuItem.Text = "Файл";
            // 
            // btnOpenFile
            // 
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(193, 26);
            btnOpenFile.Text = "Открыть";
            btnOpenFile.Click += btnOpenFile_Click;
            // 
            // btnExport
            // 
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(193, 26);
            btnExport.Text = "Экспорт в XML";
            btnExport.Click += btnExport_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { файлToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1436, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // txtSearchLastName
            // 
            txtSearchLastName.Location = new Point(1028, 28);
            txtSearchLastName.Name = "txtSearchLastName";
            txtSearchLastName.Size = new Size(296, 27);
            txtSearchLastName.TabIndex = 2;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(1330, 28);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(94, 29);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "Поиск";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click_1;
            // 
            // Task3
            // 
            Task3.Location = new Point(834, 61);
            Task3.Multiline = true;
            Task3.Name = "Task3";
            Task3.ScrollBars = ScrollBars.Vertical;
            Task3.Size = new Size(590, 395);
            Task3.TabIndex = 4;
            Task3.TextChanged += Task3_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(834, 31);
            label1.Name = "label1";
            label1.Size = new Size(188, 20);
            label1.TabIndex = 5;
            label1.Text = "Статистика по сотруднику";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1436, 468);
            Controls.Add(label1);
            Controls.Add(Task3);
            Controls.Add(btnSearch);
            Controls.Add(txtSearchLastName);
            Controls.Add(dataGridView1);
            Controls.Add(menuStrip1);
            Name = "Form1";
            Text = "XML Editor";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DataGridView dataGridView1;
        private ToolStripMenuItem файлToolStripMenuItem;
        private ToolStripMenuItem btnOpenFile;
        private ToolStripMenuItem btnExport;
        private MenuStrip menuStrip1;
        private TextBox txtSearchLastName;
        private Button btnSearch;
        private TextBox Task3;
        private Label label1;
    }
}