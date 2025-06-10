using System.IO; //for file
using System.Text.Json; // for JsonSerializer
using HumanCentricAttackPath.Models; // to use DemoData, Person, etc

namespace HumanCentricAttackPath.Services
{
    public class AttackGraphService
    {
        // Path to the JSON from disk and convert to DemoData object
        private readonly string _dataPath = Path.Combine("data", "demo_data.json");
        
        //Method to load JSON from disk and convert to a DemoData object
        public DemoData LoadData()
        {
            //Read entire file as a single string
            string json = File.ReadAllText(_dataPath);

            //Deserialize JSON string into DemoData
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            DemoData data = JsonSerializer.Deserialize<DemoData>(json, options);

            return data;
        }
    }
}
