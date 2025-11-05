using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentDriftChecker.Domain.Entities
{
    public class DriftAlert
    {
        public int Id { get; set; }
        public string AlertName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty; // e.g., "DriftCount > 5"
        public bool IsActive { get; set; }
        public string NotificationChannel { get; set; } = string.Empty; // Email, Teams, Slack
        public DateTime CreatedAt { get; set; }

        // Navigation Property (Many-to-Many)
        public ICollection<EnvironmentAlert> EnvironmentAlerts { get; set; } = new List<EnvironmentAlert>();
    }
}