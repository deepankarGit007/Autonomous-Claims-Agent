using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SynapxAgent
{
    // This is the main object that represents the final output for one document.
    public class ClaimAssessment
    {
        [JsonPropertyName("extractedFields")]
        public ExtractedFields ExtractedFields { get; set; } = new ExtractedFields();

        [JsonPropertyName("missingFields")]
        public List<string> MissingFields { get; set; } = new List<string>();

        [JsonPropertyName("recommendedRoute")]
        public string RecommendedRoute { get; set; } = "Manual review"; // Default

        [JsonPropertyName("reasoning")]
        public string Reasoning { get; set; } = string.Empty;
    }

    // This class holds all the raw data we find in the PDF.
    public class ExtractedFields
    {
        [JsonPropertyName("policyDetails")]
        public PolicyDetails Policy { get; set; } = new PolicyDetails();

        [JsonPropertyName("incidentDetails")]
        public IncidentDetails Incident { get; set; } = new IncidentDetails();

        [JsonPropertyName("involvedParties")]
        public PartiesDetails Parties { get; set; } = new PartiesDetails();

        [JsonPropertyName("assetDetails")]
        public AssetDetails Asset { get; set; } = new AssetDetails();

        [JsonPropertyName("mandatoryData")]
        public MandatoryData Mandatory { get; set; } = new MandatoryData();
    }

    public class PolicyDetails
    {
        public string? PolicyNumber { get; set; }
        public string? PolicyHolderName { get; set; }
        public string? EffectiveDate { get; set; }
        public string? ExpirationDate { get; set; }
    }

    public class IncidentDetails
    {
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
    }

    public class PartiesDetails
    {
        public string? ClaimantName { get; set; }
        public string? ThirdPartyName { get; set; } // Optional, can be null
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
    }

    public class AssetDetails
    {
        public string? AssetType { get; set; } // e.g., "Vehicle", "Property"
        public string? AssetId { get; set; }   // VIN or similar
        public decimal? EstimatedDamage { get; set; } // Nullable in case it's missing
    }

    public class MandatoryData
    {
        public string? ClaimType { get; set; } // e.g., "Injury", "Property Damage"
        public bool HasAttachments { get; set; }
        public bool HasInitialEstimate { get; set; }
    }
}

