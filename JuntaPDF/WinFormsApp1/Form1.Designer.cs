namespace WinFormsApp1
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
            btnAddFiles = new Button();
            btnRemoveFiles = new Button();
            btnMergePdfs = new Button();
            btnClear = new Button();
            listViewFiles = new ListView();
            colFileName = new ColumnHeader();
            colPages = new ColumnHeader();
            colSize = new ColumnHeader();
            colStatus = new ColumnHeader();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            btnMoveUp = new Button();
            btnMoveDown = new Button();
            groupBoxFiles = new GroupBox();
            groupBoxActions = new GroupBox();
            groupBoxFiles.SuspendLayout();
            groupBoxActions.SuspendLayout();
            SuspendLayout();
            // 
            // btnAddFiles
            // 
            btnAddFiles.Location = new Point(37, 64);
            btnAddFiles.Margin = new Padding(6, 6, 6, 6);
            btnAddFiles.Name = "btnAddFiles";
            btnAddFiles.Size = new Size(223, 75);
            btnAddFiles.TabIndex = 0;
            btnAddFiles.Text = "Adicionar PDFs";
            btnAddFiles.UseVisualStyleBackColor = true;
            btnAddFiles.Click += btnAddFiles_Click;
            // 
            // btnRemoveFiles
            // 
            btnRemoveFiles.Location = new Point(279, 64);
            btnRemoveFiles.Margin = new Padding(6, 6, 6, 6);
            btnRemoveFiles.Name = "btnRemoveFiles";
            btnRemoveFiles.Size = new Size(223, 75);
            btnRemoveFiles.TabIndex = 1;
            btnRemoveFiles.Text = "Remover";
            btnRemoveFiles.UseVisualStyleBackColor = true;
            btnRemoveFiles.Click += btnRemoveFiles_Click;
            // 
            // btnMergePdfs
            // 
            btnMergePdfs.BackColor = Color.FromArgb(0, 122, 204);
            btnMergePdfs.ForeColor = Color.White;
            btnMergePdfs.Location = new Point(37, 171);
            btnMergePdfs.Margin = new Padding(6, 6, 6, 6);
            btnMergePdfs.Name = "btnMergePdfs";
            btnMergePdfs.Size = new Size(279, 85);
            btnMergePdfs.TabIndex = 2;
            btnMergePdfs.Text = "Unificar PDFs";
            btnMergePdfs.UseVisualStyleBackColor = false;
            btnMergePdfs.Click += btnMergePdfs_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(520, 64);
            btnClear.Margin = new Padding(6, 6, 6, 6);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(223, 75);
            btnClear.TabIndex = 3;
            btnClear.Text = "Limpar Lista";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // listViewFiles
            // 
            listViewFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listViewFiles.Columns.AddRange(new ColumnHeader[] { colFileName, colPages, colSize, colStatus });
            listViewFiles.FullRowSelect = true;
            listViewFiles.GridLines = true;
            listViewFiles.Location = new Point(37, 64);
            listViewFiles.Margin = new Padding(6, 6, 6, 6);
            listViewFiles.Name = "listViewFiles";
            listViewFiles.Size = new Size(1111, 529);
            listViewFiles.TabIndex = 4;
            listViewFiles.UseCompatibleStateImageBehavior = false;
            listViewFiles.View = View.Details;
            // 
            // colFileName
            // 
            colFileName.Text = "Nome do Arquivo";
            colFileName.Width = 300;
            // 
            // colPages
            // 
            colPages.Text = "Páginas";
            colPages.Width = 80;
            // 
            // colSize
            // 
            colSize.Text = "Tamanho";
            colSize.Width = 100;
            // 
            // colStatus
            // 
            colStatus.Text = "Status";
            colStatus.Width = 100;
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(37, 1109);
            progressBar.Margin = new Padding(6, 6, 6, 6);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1245, 49);
            progressBar.TabIndex = 5;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(37, 1067);
            lblStatus.Margin = new Padding(6, 0, 6, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(85, 32);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "Pronto";
            // 
            // btnMoveUp
            // 
            btnMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveUp.Location = new Point(1189, 64);
            btnMoveUp.Margin = new Padding(6, 6, 6, 6);
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Size = new Size(56, 64);
            btnMoveUp.TabIndex = 7;
            btnMoveUp.Text = "↑";
            btnMoveUp.UseVisualStyleBackColor = true;
            btnMoveUp.Click += btnMoveUp_Click;
            // 
            // btnMoveDown
            // 
            btnMoveDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoveDown.Location = new Point(1189, 149);
            btnMoveDown.Margin = new Padding(6, 6, 6, 6);
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.Size = new Size(56, 64);
            btnMoveDown.TabIndex = 8;
            btnMoveDown.Text = "↓";
            btnMoveDown.UseVisualStyleBackColor = true;
            btnMoveDown.Click += btnMoveDown_Click;
            // 
            // groupBoxFiles
            // 
            groupBoxFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxFiles.Controls.Add(listViewFiles);
            groupBoxFiles.Controls.Add(btnMoveUp);
            groupBoxFiles.Controls.Add(btnMoveDown);
            groupBoxFiles.Location = new Point(37, 43);
            groupBoxFiles.Margin = new Padding(6, 6, 6, 6);
            groupBoxFiles.Name = "groupBoxFiles";
            groupBoxFiles.Padding = new Padding(6, 6, 6, 6);
            groupBoxFiles.Size = new Size(1263, 640);
            groupBoxFiles.TabIndex = 9;
            groupBoxFiles.TabStop = false;
            groupBoxFiles.Text = "Arquivos PDF";
            // 
            // groupBoxActions
            // 
            groupBoxActions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxActions.Controls.Add(btnAddFiles);
            groupBoxActions.Controls.Add(btnRemoveFiles);
            groupBoxActions.Controls.Add(btnClear);
            groupBoxActions.Controls.Add(btnMergePdfs);
            groupBoxActions.Location = new Point(37, 725);
            groupBoxActions.Margin = new Padding(6, 6, 6, 6);
            groupBoxActions.Name = "groupBoxActions";
            groupBoxActions.Padding = new Padding(6, 6, 6, 6);
            groupBoxActions.Size = new Size(1263, 299);
            groupBoxActions.TabIndex = 10;
            groupBoxActions.TabStop = false;
            groupBoxActions.Text = "Ações";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1337, 1195);
            Controls.Add(groupBoxActions);
            Controls.Add(groupBoxFiles);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            Margin = new Padding(6, 6, 6, 6);
            MinimumSize = new Size(1092, 986);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Unificador de PDFs - Seguro e Local";
            groupBoxFiles.ResumeLayout(false);
            groupBoxActions.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnAddFiles;
        private Button btnRemoveFiles;
        private Button btnMergePdfs;
        private Button btnClear;
        private ListView listViewFiles;
        private ColumnHeader colFileName;
        private ColumnHeader colPages;
        private ColumnHeader colSize;
        private ColumnHeader colStatus;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Button btnMoveUp;
        private Button btnMoveDown;
        private GroupBox groupBoxFiles;
        private GroupBox groupBoxActions;
    }
}