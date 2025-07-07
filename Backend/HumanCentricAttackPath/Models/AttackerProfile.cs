using System.Collections.Generic;

namespace HumanCentricAttackPath.Models
{
    public class AttackerProfile
    {
        public string Name { get; set;}
        public double PathLengthWeight { get; set;}
        public double CostWeight { get; set;}
        public double VulnerabilityWeight { get; set;}
    }

    public static class AttackerProfiles
    {
        // Stealthy: cares a lot about path length and cost, less about vulnerability
        public static AttackerProfile Stealthy = new AttackerProfile { Name = "Stealthy", PathLengthWeight = 5.0, CostWeight = 5.0, VulnerabilityWeight = 0.2 };
        // Aggressive: cares almost only about vulnerability
        public static AttackerProfile Aggressive = new AttackerProfile { Name = "Aggressive", PathLengthWeight = 0.05, CostWeight = 0.05, VulnerabilityWeight = 2.0 };
        // Opportunistic: balances all factors, but still more sensitive to vulnerability
        public static AttackerProfile Opportunistic = new AttackerProfile { Name = "Opportunistic", PathLengthWeight = 1.0, CostWeight = 1.0, VulnerabilityWeight = 1.0 };
    }
}
