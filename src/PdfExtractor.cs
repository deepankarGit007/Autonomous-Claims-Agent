using System;
using System.IO;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace SynapxAgent
{
    public static class PdfExtractor
    {
        /// <summary>
        /// Reads a PDF file and extracts all text content into a single string.
        /// </summary>
        public static string ExtractText(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file was not found: {filePath}");
            }

            StringBuilder textBuilder = new StringBuilder();

            try
            {
                using (PdfDocument document = PdfDocument.Open(filePath))
                {
                    // Loop through every page in the PDF
                    foreach (Page page in document.GetPages())
                    {
                        // Extract text from the page
                        string pageText = page.Text;

                        // Append it to our builder with a separator for clarity
                        textBuilder.AppendLine($"--- PAGE {page.Number} ---");
                        textBuilder.AppendLine(pageText);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading PDF: {ex.Message}");
                return string.Empty;
            }

            return textBuilder.ToString();
        }
    }
}