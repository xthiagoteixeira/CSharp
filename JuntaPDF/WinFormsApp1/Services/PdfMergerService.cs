using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using WinFormsApp1.Models;
using WinFormsApp1.Services;

namespace WinFormsApp1.Services
{
    /// <summary>
    /// Implementação do serviço de mesclagem de PDFs usando iText7
    /// </summary>
    public class PdfMergerService : IPdfMergerService
    {
        public async Task<bool> ValidatePdfAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                await Task.Run(() =>
                {
                    using var reader = new PdfReader(filePath);
                    using var document = new iText.Kernel.Pdf.PdfDocument(reader);
                    // Se chegou até aqui, o PDF é válido
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Models.PdfDocument> GetPdfInfoAsync(string filePath)
        {
            var pdfDoc = new Models.PdfDocument
            {
                FilePath = filePath,
                FileName = Path.GetFileName(filePath)
            };

            try
            {
                var fileInfo = new FileInfo(filePath);
                pdfDoc.FileSize = fileInfo.Length;

                var pageCount = await GetPageCountAsync(filePath);
                pdfDoc.PageCount = pageCount;
                pdfDoc.IsValid = pageCount > 0;
            }
            catch
            {
                pdfDoc.IsValid = false;
                pdfDoc.PageCount = 0;
            }

            return pdfDoc;
        }

        public async Task<bool> MergePdfsAsync(IEnumerable<string> inputFiles, string outputPath, IProgress<int> progress = null)
        {
            try
            {
                var fileList = inputFiles.ToList();
                if (!fileList.Any())
                    return false;

                await Task.Run(() =>
                {
                    using var writer = new PdfWriter(outputPath);
                    using var mergedDocument = new iText.Kernel.Pdf.PdfDocument(writer);
                    var merger = new PdfMerger(mergedDocument);

                    int processedFiles = 0;
                    int totalFiles = fileList.Count;

                    foreach (var inputFile in fileList)
                    {
                        using var reader = new PdfReader(inputFile);
                        using var sourceDocument = new iText.Kernel.Pdf.PdfDocument(reader);

                        merger.Merge(sourceDocument, 1, sourceDocument.GetNumberOfPages());

                        processedFiles++;
                        var progressPercentage = (int)((double)processedFiles / totalFiles * 100);
                        progress?.Report(progressPercentage);
                    }
                });

                return true;
            }
            catch (Exception)
            {
                // Log da exceção poderia ser adicionado aqui
                return false;
            }
        }

        public async Task<int> GetPageCountAsync(string filePath)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using var reader = new PdfReader(filePath);
                    using var document = new iText.Kernel.Pdf.PdfDocument(reader);
                    return document.GetNumberOfPages();
                });
            }
            catch
            {
                return 0;
            }
        }
    }
}