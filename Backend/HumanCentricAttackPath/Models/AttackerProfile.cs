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
        public static AttackerProfile Stealthy = new AttackerProfile { Name = "Stealthy", PathLengthWeight = 1.0, CostWeight = 1.0, VulnerabilityWeight = 0.5 };
        public static AttackerProfile Aggressive = new AttackerProfile { Name = "Aggressive", PathLengthWeight = 0.2, CostWeight = 0.1, VulnerabilityWeight = 1.0 };
        public static AttackerProfile Opportunistic = new AttackerProfile { Name = "Opportunistic", PathLengthWeight = 0.5, CostWeight = 0.5, VulnerabilityWeight = 0.5 };
    }
}
