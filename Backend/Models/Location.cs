using System.Collections.Generic;

namespace HumanCentricAttackPath.Models
{
    public class Location 
    {
        public string location_id { get; set;}
        public string name { get; set;}
        public bool has_badge_number { get; set;}
        public List<string> adjacent_to { get; set;}

    }
}

