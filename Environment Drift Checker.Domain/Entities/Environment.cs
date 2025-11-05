using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentDriftChecker.Domain.Entities
{
    public class Environment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public ICollection<ConfigurationItem> ConfigurationItems { get; set; } = new List<ConfigurationItem>();
        public ICollection<DriftRecord> DriftRecordsAsSource { get; set; } = new List<DriftRecord>();
        public ICollection<DriftRecord> DriftRecordsAsTarget { get; set; } = new List<DriftRecord>();
        public ICollection<EnvironmentAlert> EnvironmentAlerts { get; set; } = new List<EnvironmentAlert>();
    }
}
