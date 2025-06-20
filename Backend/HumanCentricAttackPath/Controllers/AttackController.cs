using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;
using HumanCentricAttackPath.Models;
using HumanCentricAttackPath.Services;

namespace HumanCentricAttackPath.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttackController : ControllerBase
    {
        private readonly AttackScoringService _scoringService;

        public AttackController()
        {
            _scoringService = new AttackScoringService();
        }

        [HttpGet("test_vulnerability")]
        public ActionResult<object> TestVulnerabilityScoring()
        {
            var dataPath = Path.Combine("data", "demo_data.json");
            var json = System.IO.File.ReadAllText(dataPath);
            var demoData = JsonSerializer.Deserialize<DemoData>(json);

            var results = new List<object>();

            foreach (var person in demoData.persons)
            {
                double vulnerability = _scoringService.CalculateUserVulnerability(person);
                
                results.Add(new
                {
                    user_id = person.user_id,
                    name = person.name,
                    role = person.role,
                    risk_profile = person.risk_profile,
                    last_training_date = person.last_training_date,
                    social_engineering_resistance = person.social_engineering_resistance,
                    authority_level = person.authority_level,
                    vulnerability_score = vulnerability,
                    vulnerability_percentage = Math.Round(vulnerability * 100, 2)
                });
            }

            return Ok(results);
        }

        [HttpGet("top_paths")]
        public ActionResult<List<AttackPath>> GetTopPaths()
        {
            var dataPath = Path.Combine("data", "demo_data.json");
            var json = System.IO.File.ReadAllText(dataPath);
            var demoData = JsonSerializer.Deserialize<DemoData>(json);

            var alice = demoData.persons.FirstOrDefault(p => p.user_id == "U1001");
            double probability = (alice != null && alice.has_phish_training) ? 0.01 : 0.05;

            var topPaths = new List<AttackPath>
            {
                new AttackPath { Path = "Attacker -> Alice Smith -> HR Database", Probability = probability }
            };
            return Ok(topPaths);
        }

        [HttpPost("toggle_training")]
        public ActionResult<List<AttackPath>> ToggleTraining([FromBody] ToggleTrainingRequest request)
        {
            var dataPath = Path.Combine("data", "demo_data.json");
            var json = System.IO.File.ReadAllText(dataPath);
            var demoData = JsonSerializer.Deserialize<DemoData>(json);

            var alice = demoData.persons.FirstOrDefault(p => p.user_id == "U1001");
            if (alice != null)
            {
                alice.has_phish_training = !alice.has_phish_training;
            }

            var updatedJson = JsonSerializer.Serialize(demoData, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(dataPath, updatedJson);

            double probability = (alice != null && alice.has_phish_training) ? 0.01 : 0.05;

            var topPaths = new List<AttackPath>
            {
                new AttackPath { Path = "Attacker -> Alice Smith -> HR Database", Probability = probability }
            };
            return Ok(topPaths);
        }

        public class ToggleTrainingRequest
        {
            public string user_id { get; set; } = string.Empty;
        }
    }
}