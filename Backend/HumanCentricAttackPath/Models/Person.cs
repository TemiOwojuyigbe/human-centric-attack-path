// Backend/Models/Person.cs
using System.Collections.Generic;

namespace HumanCentricAttackPath.Models
{
    public class Person
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string risk_profile { get; set; }
        public bool has_phish_training { get; set; }
        public List<string> access_assets { get; set; }
        public List<string> access_locations { get; set; }
    }
}