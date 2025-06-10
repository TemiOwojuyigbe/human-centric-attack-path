using System.Collections.Generic;

namespace HumanCentricAttackPath.Models
{
    public class Vulnerability
    {
        public string cve { get; set;}
        public string severity { get; set;}
    }

    public class Asset
    {
        public string asset_id { get; set;}
        public string name { get; set;}
        public string type { get; set;}
        public string location_id { get; set;}
        public List<Vulnerability> vulnerabilities { get; set;}
        public bool is_critical { get; set;}

    }
}