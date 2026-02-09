using Xunit;
using SynapxAgent;
using System.Collections.Generic;

namespace SynapxAgent.Tests
{
    public class RuleEngineTests
    {
        // Helper method to create a "perfect" claim, then we break it in specific tests
        private ExtractedFields CreateBaseClaim()
        {
            return new ExtractedFields
            {
                Policy = new PolicyDetails { PolicyNumber = "P-12345", PolicyHolderName = "John Doe" },
                Incident = new IncidentDetails { Date = "2025-01-01", Description = "Fender bender in parking lot." },
                Asset = new AssetDetails { EstimatedDamage = 5000 },
                Mandatory = new MandatoryData { ClaimType = "Property Damage" }
            };
        }

        [Fact]
        public void EvaluateClaim_ShouldRouteToFastTrack_WhenDamageIsLowAndDataComplete()
        {
            // Arrange
            var claim = CreateBaseClaim();
            claim.Asset.EstimatedDamage = 10000; // Less than $25,000

            // Act
            var result = RuleEngine.EvaluateClaim(claim);

            // Assert
            Assert.Equal("Fast-track", result.RecommendedRoute);
        }

        [Fact]
        public void EvaluateClaim_ShouldRouteToManualReview_WhenDamageIsHigh()
        {
            // Arrange
            var claim = CreateBaseClaim();
            claim.Asset.EstimatedDamage = 30000; // More than $25,000

            // Act
            var result = RuleEngine.EvaluateClaim(claim);

            // Assert
            Assert.Equal("Manual review", result.RecommendedRoute);
        }

        [Fact]
        public void EvaluateClaim_ShouldRouteToSpecialist_WhenClaimTypeIsInjury()
        {
            // Arrange
            var claim = CreateBaseClaim();
            claim.Mandatory.ClaimType = "Bodily Injury";

            // Act
            var result = RuleEngine.EvaluateClaim(claim);

            // Assert
            Assert.Equal("Specialist Queue", result.RecommendedRoute);
        }

        [Fact]
        public void EvaluateClaim_ShouldFlagInvestigation_WhenDescriptionContainsFraudKeywords()
        {
            // Arrange
            var claim = CreateBaseClaim();
            claim.Incident.Description = "The driver gave inconsistent details about the crash.";

            // Act
            var result = RuleEngine.EvaluateClaim(claim);

            // Assert
            Assert.Equal("Investigation Flag", result.RecommendedRoute);
            Assert.Contains("fraud", result.Reasoning.ToLower()); // Verify reasoning mentions fraud
        }

        [Fact]
        public void EvaluateClaim_ShouldRouteToManualReview_WhenMandatoryFieldIsMissing()
        {
            // Arrange
            var claim = CreateBaseClaim();
            claim.Policy.PolicyNumber = null; // Missing Policy Number

            // Act
            var result = RuleEngine.EvaluateClaim(claim);

            // Assert
            Assert.Equal("Manual review", result.RecommendedRoute);
            Assert.Contains("Policy Number", result.Reasoning); // Verify it specifically mentions the missing field
        }
    }
}