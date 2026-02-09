Autonomous Insurance Claims Processing Agent
An intelligent, lightweight agent built with .NET Core (C#) and Google Gemini AI. This tool automates the ingestion of First Notice of Loss (FNOL) documents, extracts critical data, validates completeness, and routes claims to the appropriate workflow based on business logic.

🚀 Features
Automated Ingestion: Reads raw text from PDF FNOL documents (e.g., ACORD forms) using PdfPig.

AI-Powered Extraction: Utilizes Google Gemini 1.5 Flash to intelligently parse unstructured text into structured data (JSON).

Smart Validation: Automatically detects missing mandatory fields (Policy Number, Date, Description).

Intelligent Routing: Applies business rules to classify claims into workflows:

Fast-track: Claims with < $25,000 damage and complete data.

Specialist Queue: Claims involving injuries.

Investigation Flag: Claims with suspicious keywords (e.g., "fraud", "inconsistent").

Manual Review: Incomplete or ambiguous claims.

📂 Project Structure
Plaintext
Synapx-Assessment-Agent
 ┣ 📂 src
 ┃ ┣ 📂 InputDocs           <-- Place your PDF files here
 ┃ ┣ 📂 Output              <-- JSON results appear here
 ┃ ┣ 📄 Program.cs          <-- Main application entry
 ┃ ┣ 📄 ClaimsAgent.cs      <-- AI Integration (Gemini)
 ┃ ┣ 📄 RuleEngine.cs       <-- Routing Logic
 ┃ ┣ 📄 PdfExtractor.cs     <-- OCR/Text Extraction
 ┃ ┗ 📄 DomainModels.cs     <-- Data Structures
 ┗ 📄 README.md
🛠️ Prerequisites
.NET 8.0 SDK

A Google Gemini API Key (Free tier is sufficient).

⚙️ Setup & Installation
Clone the repository (or unzip the project):

Bash
git clone <repository-url>
cd Synapx-Assessment-Agent
Navigate to the source folder:

Bash
cd src
Restore dependencies:

Bash
dotnet restore
🏃‍♂️ How to Run
Prepare your Input:
Place your PDF files (e.g., ACORD-Automobile-Loss-Notice.pdf) into the src/InputDocs folder.

Running Tests
To verify the business logic (routing rules), run the automated test suite:
```bash
dotnet test

Run the Application:

Bash
dotnet run
Enter API Key:
When prompted, paste your Google Gemini API Key and press Enter.
(Note: The key is not stored and is only used for the current session.)

View Results:
The agent will process all files in the input folder. Check the src/Output directory for the generated JSON files (e.g., Claim_Result.json).

🧠 Routing Logic
The agent applies the following rules hierarchy to determine the recommendedRoute :

Investigation Flag: Triggered if description contains keywords like "fraud", "staged", or "inconsistent".

Specialist Queue: Triggered if the Claim Type indicates "Injury".

Manual Review: Triggered if mandatory fields (Policy #, Name, Date) are missing.

Fast-track: Triggered if Estimated Damage is less than $25,000 and all data is valid.

📦 Dependencies
UglyToad.PdfPig (PDF Text Extraction)

System.Text.Json (JSON Serialization)

Microsoft.NET.Sdk

## 🎥 Demo Video
Watch the agent process the claims in real-time:

[**▶️ Watch the Demo Video Here**](https://drive.google.com/file/d/1QSn1CL5bvcxUYTtGOtW806V6ZAY6_e4R/view?usp=sharing)