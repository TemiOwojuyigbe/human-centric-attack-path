using System.Collections.Generic;

namespace HumanCentricAttackPath.Models
{
    public enum SecurityLevel 
    {
        Low,
        Medium,
        High
    }
    public class Location 
    {
        public string location_id { get; set;}
        public string name { get; set;}
        public bool has_badge_reader { get; set;}
        public SecurityLevel security_level { get; set;}
        public List<string> adjacent_to { get; set;}

    }
}