// Backend/Models/Person.cs
using System.Collections.Generic;
using System;


namespace HumanCentricAttackPath.Models
{
    public enum AuthorityLevel 
    {
        Low,
        Medium,
        High
    }
    public class Person
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string risk_profile { get; set; }
        public bool has_phish_training { get; set; }
        public List<string> access_assets { get; set;}
        public List<string> access_locations { get; set; }
        
        public DateTime? last_training_date { get; set;}
        public string training_type { get; set;}
        public int social_engineering_resistance { get; set;}
        public string authority_level { get; set;}
        public string work_schedule { get; set;}
        public List<string> communication_preferences { get; set;}
    }
}