using System;
using System.Collections.Generic;

namespace SynapxAgent
{
    public static class RuleEngine
    {
        public static ClaimAssessment EvaluateClaim(ExtractedFields data)
        {
            var assessment = new ClaimAssessment
            {
                ExtractedFields = data
            };

            // Rule 1: Identify Missing Fields
            var missing = new List<string>();
            if (string.IsNullOrEmpty(data.Policy.PolicyNumber)) missing.Add("Policy Number");
            if (string.IsNullOrEmpty(data.Policy.PolicyHolderName)) missing.Add("Policy Holder Name");
            if (string.IsNullOrEmpty(data.Incident.Date)) missing.Add("Incident Date");
            if (string.IsNullOrEmpty(data.Incident.Description)) missing.Add("Incident Description");

            assessment.MissingFields = missing;

            // Rule 2: Determine Route based on priority logic

            // Check for Fraud Keywords (Highest Priority)
            var description = data.Incident.Description?.ToLower() ?? "";
            if (description.Contains("fraud") || description.Contains("staged") || description.Contains("inconsistent"))
            {
                assessment.RecommendedRoute = "Investigation Flag";
                assessment.Reasoning = "Potential fraud indicators detected in claim description.";
                return assessment;
            }

            // Check for Injury (Specialist)
            if (data.Mandatory.ClaimType?.ToLower().Contains("injury") == true)
            {
                assessment.RecommendedRoute = "Specialist Queue";
                assessment.Reasoning = "Claim involves personal injury.";
                return assessment;
            }

            // Check for Missing Data (Manual Review)
            if (missing.Count > 0)
            {
                assessment.RecommendedRoute = "Manual review";
                assessment.Reasoning = $"Mandatory fields are missing: {string.Join(", ", missing)}";
                return assessment;
            }

            // Check for Low Value (Fast-track)
            // If damage is known AND less than $25,000
            if (data.Asset.EstimatedDamage.HasValue && data.Asset.EstimatedDamage < 25000)
            {
                assessment.RecommendedRoute = "Fast-track";
                assessment.Reasoning = "Estimated damage is under $25,000 and all data is present.";
                return assessment;
            }

            // Default fallback
            assessment.RecommendedRoute = "Manual review";
            assessment.Reasoning = "Standard processing required.";

            return assessment;
        }
    }
}