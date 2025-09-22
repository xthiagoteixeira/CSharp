using WinFormsApp1.Models;
using WinFormsApp1.Services;

namespace WinFormsApp1
{
    /// <summary>
    /// Formulário principal da aplicação de unificação de PDFs
    /// </summary>
    public partial class Form1 : Form
    {
        private readonly IPdfMergerService _pdfService;
        private readonly List<PdfDocument> _pdfDocuments;

        public Form1()
        {
            InitializeComponent();
            _pdfService = new PdfMergerService();
            _pdfDocuments = new List<PdfDocument>();

            ConfigureListView();
            UpdateButtonStates();
        }

        private void ConfigureListView()
        {
            listViewFiles.AllowDrop = true;
            listViewFiles.DragEnter += ListViewFiles_DragEnter;
            listViewFiles.DragDrop += ListViewFiles_DragDrop;
        }

        private async void btnAddFiles_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Arquivos PDF (*.pdf)|*.pdf",
                Multiselect = true,
                Title = "Selecionar arquivos PDF"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                await AddPdfFilesAsync(openFileDialog.FileNames);
            }
        }

        private void btnRemoveFiles_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                var indicesToRemove = listViewFiles.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();

                foreach (int index in indicesToRemove)
                {
                    _pdfDocuments.RemoveAt(index);
                    listViewFiles.Items.RemoveAt(index);
                }

                UpdateButtonStates();
                UpdateStatus($"Removidos {indicesToRemove.Count} arquivo(s)");
            }
        }

        private async void btnMergePdfs_Click(object sender, EventArgs e)
        {
            if (_pdfDocuments.Count < 2)
            {
                MessageBox.Show("Selecione pelo menos 2 arquivos PDF para unificar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var saveFileDialog = new SaveFileDialog
            {
                Filter = "Arquivo PDF (*.pdf)|*.pdf",
                Title = "Salvar PDF unificado",
                FileName = "PDFs_Unificados.pdf"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                await MergePdfsAsync(saveFileDialog.FileName);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _pdfDocuments.Clear();
            listViewFiles.Items.Clear();
            progressBar.Value = 0;
            UpdateButtonStates();
            UpdateStatus("Lista limpa");
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(-1);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(1);
        }

        private async void ListViewFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Any(f => Path.GetExtension(f).ToLower() == ".pdf"))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private async void ListViewFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var pdfFiles = files.Where(f => Path.GetExtension(f).ToLower() == ".pdf").ToArray();

                if (pdfFiles.Any())
                {
                    await AddPdfFilesAsync(pdfFiles);
                }
            }
        }

        private async Task AddPdfFilesAsync(string[] filePaths)
        {
            btnAddFiles.Enabled = false;
            UpdateStatus("Adicionando arquivos...");

            try
            {
                foreach (var filePath in filePaths)
                {
                    if (_pdfDocuments.Any(d => d.FilePath == filePath))
                        continue; // Arquivo já adicionado

                    var pdfDoc = await _pdfService.GetPdfInfoAsync(filePath);
                    _pdfDocuments.Add(pdfDoc);

                    var item = new ListViewItem(pdfDoc.FileName);
                    item.SubItems.Add(pdfDoc.PageCount.ToString());
                    item.SubItems.Add(pdfDoc.FileSizeFormatted);
                    item.SubItems.Add(pdfDoc.IsValid ? "Válido" : "Inválido");
                    item.Tag = pdfDoc;

                    if (!pdfDoc.IsValid)
                        item.ForeColor = Color.Red;

                    listViewFiles.Items.Add(item);
                }

                UpdateButtonStates();
                UpdateStatus($"Adicionados {filePaths.Length} arquivo(s)");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar arquivos: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAddFiles.Enabled = true;
            }
        }

        private async Task MergePdfsAsync(string outputPath)
        {
            var validPdfs = _pdfDocuments.Where(d => d.IsValid).ToList();

            if (validPdfs.Count < 2)
            {
                MessageBox.Show("Pelo menos 2 PDFs válidos são necessários para unificação.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnMergePdfs.Enabled = false;
            progressBar.Value = 0;

            try
            {
                var progress = new Progress<int>(value =>
                {
                    progressBar.Value = value;
                    UpdateStatus($"Unificando PDFs... {value}%");
                });

                var inputFiles = validPdfs.Select(d => d.FilePath);
                bool success = await _pdfService.MergePdfsAsync(inputFiles, outputPath, progress);

                if (success)
                {
                    progressBar.Value = 100;
                    UpdateStatus("PDFs unificados com sucesso!");

                    var result = MessageBox.Show(
                        "PDFs unificados com sucesso!\n\nDeseja abrir o arquivo criado?",
                        "Sucesso",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = outputPath,
                            UseShellExecute = true
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Erro ao unificar os PDFs. Verifique se os arquivos são válidos.", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus("Erro na unificação");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a unificação: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Erro na unificação");
            }
            finally
            {
                btnMergePdfs.Enabled = true;
                progressBar.Value = 0;
            }
        }

        private void MoveSelectedItem(int direction)
        {
            if (listViewFiles.SelectedItems.Count != 1)
                return;

            int selectedIndex = listViewFiles.SelectedIndices[0];
            int newIndex = selectedIndex + direction;

            if (newIndex < 0 || newIndex >= _pdfDocuments.Count)
                return;

            // Trocar na lista de documentos
            var temp = _pdfDocuments[selectedIndex];
            _pdfDocuments[selectedIndex] = _pdfDocuments[newIndex];
            _pdfDocuments[newIndex] = temp;

            // Recriar ListView
            RefreshListView();

            // Selecionar o item na nova posição
            listViewFiles.Items[newIndex].Selected = true;
            listViewFiles.Items[newIndex].Focused = true;
        }

        private void RefreshListView()
        {
            listViewFiles.Items.Clear();

            foreach (var pdfDoc in _pdfDocuments)
            {
                var item = new ListViewItem(pdfDoc.FileName);
                item.SubItems.Add(pdfDoc.PageCount.ToString());
                item.SubItems.Add(pdfDoc.FileSizeFormatted);
                item.SubItems.Add(pdfDoc.IsValid ? "Válido" : "Inválido");
                item.Tag = pdfDoc;

                if (!pdfDoc.IsValid)
                    item.ForeColor = Color.Red;

                listViewFiles.Items.Add(item);
            }
        }

        private void UpdateButtonStates()
        {
            bool hasFiles = _pdfDocuments.Count > 0;
            bool hasValidFiles = _pdfDocuments.Count(d => d.IsValid) >= 2;

            btnRemoveFiles.Enabled = hasFiles;
            btnClear.Enabled = hasFiles;
            btnMergePdfs.Enabled = hasValidFiles;
            btnMoveUp.Enabled = hasFiles;
            btnMoveDown.Enabled = hasFiles;
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;
            Application.DoEvents();
        }
    }
}