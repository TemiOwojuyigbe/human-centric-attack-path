using System;
using System.Collections.Generic;
using System.IO; //for file
using System.Text.Json; // for JsonSerializer
using HumanCentricAttackPath.Models;

namespace HumanCentricAttackPath.Services 
{   
    public class AttackScoringService
    {
        // Main user vulnerability scoring method
        public double CalculateUserVulnerability(Person person)
        {
            // Start with base vulnerability (0.0 = not vulnerable, 1.0 = very vulnerable)
            double baseVulnerability = 0.5; // 50% base vulnerability
                
            // Factor 1: Training impact
            double trainingImpact = CalculateTrainingImpact(person);
                
            // Factor 2: Social engineering resistance
            double resistanceImpact = CalculateResistanceImpact(person);
                
            // Factor 3: Risk profile impact
            double riskImpact = CalculateRiskImpact(person);
                
            // Factor 4: Authority level (higher authority = more valuable target)
            double authorityImpact = CalculateAuthorityImpact(person);
                
            // Combine all factors
            double finalVulnerability = baseVulnerability + trainingImpact + resistanceImpact + riskImpact;
                
            // Ensure result is between 0.0 and 1.0
            return Math.Max(0.0, Math.Min(1.0, finalVulnerability));
        }
        private double CalculateTrainingImpact(Person person)
        {
            if (person.last_training_date == null)
                return 0.3; // No training = 30% more vulnerable
                
            var daysSinceTraining = (DateTime.Now - person.last_training_date.Value).Days;
                
            if (daysSinceTraining <= 90) // 3 months
                return -0.2; // Recent training = 20% less vulnerable
            else if (daysSinceTraining <= 180) // 6 months
                return -0.1; // Older training = 10% less vulnerable
            else if (daysSinceTraining <= 365) // 1 year
                return -0.05; // Very old training = 5% less vulnerable
            else
                return 0.1; // Very old training = 10% more vulnerable
        }
        private double CalculateResistanceImpact(Person person)
        {
            // Convert resistance score (0-100) to vulnerability impact
            // Higher resistance = lower vulnerability
            double resistanceFactor = (100 - person.social_engineering_resistance) / 100.0;
            return resistanceFactor * 0.4; // Max 40% impact
        }
        private double CalculateRiskImpact(Person person)
        {
            switch (person.risk_profile.ToLower())
            {
                case "high": return 0.3; // High risk = 30% more vulnerable
                case "medium": return 0.15; // Medium risk = 15% more vulnerable
                case "low": return 0.0; // Low risk = no additional vulnerability
                default: return 0.0;
            }
        }
        private double CalculateAuthorityImpact(Person person)
        {
            switch (person.authority_level.ToLower())
            {
                case "high": return 0.2; // High authority = 20% more valuable target
                case "medium": return 0.1; // Medium authority = 10% more valuable
                case "low": return 0.0; // Low authority = no additional value
                default: return 0.0;
            }
        }
            
        // Asset value scoring
        public double CalculateAssetValue(Asset asset)
        {
            double score = 0.0;

            if (asset.type == "Database"){
                score += 0.2;
            }
            if (asset.type == "Directory Service"){
                score += 0.3;
            }
            if (asset.is_critical == true) {
                score += 0.4;
            }

            foreach (var vulnerability in asset.vulnerabilities){
                if (vulnerability.severity != null){
                    switch (vulnerability.severity.ToLower()){
                    case "high": score += 0.2; break;
                    case "medium": score += 0.1; break;
                    case "low": score += 0.05; break;
                    }
                } else {
                    return 0.0;
                }
            }
            if (score > 1.0){
                score = 1.0;
            }
            return score;

        }

        // Path finding 
        public List<AttackPath> FindAttackPaths(List<Person> users, List<Asset> assets, List<Location> locations, AttackerProfile profile)
        {
            var paths = new List<AttackPath>();

            foreach (var user in users)
            {
                double userVuln = CalculateUserVulnerability(user);
                string userLabel = $"{user.name} (Vuln: {userVuln:F2})";

                foreach (var locId in user.access_locations)
                {
                    DfsLocation(
                        userLabel,
                        userVuln,
                        locId,
                        "", // pathSoFar
                        new HashSet<string>(),
                        locations,
                        assets,
                        paths,
                        profile,
                        1, 
                        0.0 
                    );
                }

                // For direct asset access
                foreach (var assetId in user.access_assets)
                {
                    var asset = assets.FirstOrDefault(a => a.asset_id == assetId);
                    if (asset != null)
                    {
                        double assetValue = CalculateAssetValue(asset);
                        double pathScore = (profile.VulnerabilityWeight * userVuln * assetValue) /
                        (1 + profile.PathLengthWeight * 1 + profile.CostWeight * 0.0);
                        var path = new AttackPath
                        {
                            Path = $"{userLabel} -> {asset.name}",
                            Probability = pathScore
                        };
                        paths.Add(path);
                    }
                }
            }

            paths = paths.OrderByDescending(p => p.Probability).ToList();
            return paths;
        }

        private void DfsLocation(
            string userLabel,
            double userVuln,
            string currentLocId,
            string pathSoFar,
            HashSet<string> visitedLocs,
            List<Location> locations,
            List<Asset> assets,
            List<AttackPath> paths,
            AttackerProfile profile,
            int pathLength = 1,
            double totalCost = 0.0)
        {
            visitedLocs.Add(currentLocId);

            // Find all assets in this location
            var assetsInLoc = assets.Where(a => a.location_id == currentLocId).ToList();
            foreach (var asset in assetsInLoc)
            {
                double assetValue = CalculateAssetValue(asset);
                double pathScore = (profile.VulnerabilityWeight * userVuln * assetValue) /
                (1 + profile.PathLengthWeight * pathLength + profile.CostWeight * totalCost); 
                var pathObj = new AttackPath
                {
                    Path = $"{userLabel} -> {pathSoFar}{locations.First(l => l.location_id == currentLocId).name} -> {asset.name}",
                    Probability = pathScore
                };
                paths.Add(pathObj);
            }

            // Find adjacent locations and recurse
            var currentLoc = locations.First(l => l.location_id == currentLocId);
            double stepCost = 0.0;
            if (currentLoc.has_badge_reader) stepCost += 1.0;
            foreach (var adjLocId in currentLoc.adjacent_to)
            {
                if (!visitedLocs.Contains(adjLocId))
                {
                    DfsLocation(
                        userLabel,
                        userVuln,
                        adjLocId,
                        pathSoFar + currentLoc.name + " -> ",
                        visitedLocs,
                        locations,
                        assets,
                        paths,
                        profile,
                        pathLength + 1,
                        totalCost + stepCost
                    );
                }
            }

            visitedLocs.Remove(currentLocId); // Backtrack
        }

        // Combined scoring 
        public List<AttackPath> GetOptimalAttackPaths(DemoData data, AttackerProfile profile, int topN = 1)
        {
            var allPaths = FindAttackPaths(data.persons, data.assets, data.locations, profile );
            return allPaths.Take(topN).ToList();
        }
    }
           
}