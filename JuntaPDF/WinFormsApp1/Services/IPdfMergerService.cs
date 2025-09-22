using WinFormsApp1.Models;

namespace WinFormsApp1.Services
{
    /// <summary>
    /// Interface para servi�os de manipula��o de PDF
    /// </summary>
    public interface IPdfMergerService
    {
        /// <summary>
        /// Valida se um arquivo � um PDF v�lido
        /// </summary>
        Task<bool> ValidatePdfAsync(string filePath);

        /// <summary>
        /// Obt�m informa��es de um documento PDF
        /// </summary>
        Task<PdfDocument> GetPdfInfoAsync(string filePath);

        /// <summary>
        /// Mescla m�ltiplos PDFs em um �nico arquivo
        /// </summary>
        Task<bool> MergePdfsAsync(IEnumerable<string> inputFiles, string outputPath, IProgress<int> progress = null);

        /// <summary>
        /// Obt�m o n�mero de p�ginas de um PDF
        /// </summary>
        Task<int> GetPageCountAsync(string filePath);
    }
}