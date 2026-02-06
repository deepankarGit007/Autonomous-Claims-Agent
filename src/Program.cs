using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SynapxAgent
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("   Synapx Autonomous Insurance Claims Agent");
            Console.WriteLine("=================================================");

            // 1. Configuration Setup
            // In a real app, use appsettings.json or Environment Variables.
            Console.Write("Enter your Google Gemini API Key: ");
            string? apiKey = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Error: API Key is required to proceed.");
                return;
            }

            var agent = new ClaimsAgent(apiKey);

            // 2. Locate Directories
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // Navigate up from bin/Debug/net8.0 to the project root
            string inputFolder = Path.GetFullPath(Path.Combine(baseDir, "../../../InputDocs"));
            string outputFolder = Path.GetFullPath(Path.Combine(baseDir, "../../../Output"));

            // Ensure folders exist
            Directory.CreateDirectory(inputFolder);
            Directory.CreateDirectory(outputFolder);

            // 3. Process Files
            string[] pdfFiles = Directory.GetFiles(inputFolder, "*.pdf");

            if (pdfFiles.Length == 0)
            {
                Console.WriteLine($"\nNo PDF files found in: {inputFolder}");
                Console.WriteLine("Please place your FNOL documents (e.g., ACORD forms) there and run again.");
                return;
            }

            Console.WriteLine($"\nFound {pdfFiles.Length} document(s) to process...");

            foreach (var filePath in pdfFiles)
            {
                string fileName = Path.GetFileName(filePath);
                Console.WriteLine($"\n[Processing] {fileName}...");

                try
                {
                    // Step A: Ingestion (OCR/Text Extraction)
                    Console.Write("  - Extracting text... ");
                    string rawText = PdfExtractor.ExtractText(filePath);
                    if (string.IsNullOrWhiteSpace(rawText))
                    {
                        Console.WriteLine("Failed (Empty text). Skipping.");
                        continue;
                    }
                    Console.WriteLine("Done.");

                    // Step B: AI Processing (Extraction)
                    Console.Write("  - AI Agent extracting fields... ");
                    ExtractedFields extractedData = await agent.ExtractDataAsync(rawText);
                    Console.WriteLine("Done.");

                    // Step C: Business Logic (Routing)
                    Console.Write("  - Applying routing rules... ");
                    ClaimAssessment finalResult = RuleEngine.EvaluateClaim(extractedData);
                    Console.WriteLine($"Done. (Route: {finalResult.RecommendedRoute})");

                    // Step D: Save Output
                    string jsonOutput = JsonSerializer.Serialize(finalResult, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    string outputFilePath = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(fileName)}_Result.json");
                    await File.WriteAllTextAsync(outputFilePath, jsonOutput);

                    Console.WriteLine($"  - Saved to: Output/{Path.GetFileName(outputFilePath)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  - ERROR: {ex.Message}");
                }
            }

            Console.WriteLine("\n=================================================");
            Console.WriteLine("   Batch Processing Complete.");
            Console.WriteLine("=================================================");
        }
    }
}