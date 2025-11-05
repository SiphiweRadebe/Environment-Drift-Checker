using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentDriftChecker.Domain.Entities
{
    public class DriftDetail
    {
        public int Id { get; set; }
        public int DriftRecordId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string? SourceValue { get; set; }
        public string? TargetValue { get; set; }
        public string DriftType { get; set; } = string.Empty; // Different, MissingInSource, MissingInTarget, Match
        public string Severity { get; set; } = string.Empty; // Low, Medium, High

        // Navigation Property
        public DriftRecord DriftRecord { get; set; } = null!;
    }
}

