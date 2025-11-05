using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentDriftChecker.Domain.Entities
{
    public class DriftRecord
    {
        public int Id { get; set; }
        public int SourceEnvironmentId { get; set; }
        public int TargetEnvironmentId { get; set; }
        public DateTime ComparisonDate { get; set; }
        public int TotalKeys { get; set; }
        public int DriftCount { get; set; }
        public string Status { get; set; } = string.Empty; // Clean, Warning, Critical

        // Navigation Properties
        public Environment SourceEnvironment { get; set; } = null!;
        public Environment TargetEnvironment { get; set; } = null!;
        public ICollection<DriftDetail> DriftDetails { get; set; } = new List<DriftDetail>();
    }
}

