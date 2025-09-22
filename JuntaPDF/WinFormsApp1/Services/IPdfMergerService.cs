using WinFormsApp1.Models;

namespace WinFormsApp1.Services
{
    /// <summary>
    /// Interface para serviços de manipulação de PDF
    /// </summary>
    public interface IPdfMergerService
    {
        /// <summary>
        /// Valida se um arquivo é um PDF válido
        /// </summary>
        Task<bool> ValidatePdfAsync(string filePath);

        /// <summary>
        /// Obtém informações de um documento PDF
        /// </summary>
        Task<PdfDocument> GetPdfInfoAsync(string filePath);

        /// <summary>
        /// Mescla múltiplos PDFs em um único arquivo
        /// </summary>
        Task<bool> MergePdfsAsync(IEnumerable<string> inputFiles, string outputPath, IProgress<int> progress = null);

        /// <summary>
        /// Obtém o número de páginas de um PDF
        /// </summary>
        Task<int> GetPageCountAsync(string filePath);
    }
}