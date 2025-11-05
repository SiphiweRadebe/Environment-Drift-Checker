using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentDriftChecker.Domain.Entities
{
    public class ConfigurationItem
    {
        public int Id { get; set; }
        public int EnvironmentId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // API, Database, Feature, Security
        public DateTime LastUpdated { get; set; }
        public bool IsSensitive { get; set; } // Hide in logs/UI

        // Navigation Property
        public Environment Environment { get; set; } = null!;
    }
}
