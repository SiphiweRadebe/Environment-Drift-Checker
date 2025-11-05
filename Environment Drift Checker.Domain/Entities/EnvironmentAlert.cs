using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentDriftChecker.Domain.Entities
{
    public class EnvironmentAlert
    {
        public int EnvironmentId { get; set; }
        public int DriftAlertId { get; set; }

        // Navigation Properties
        public Environment Environment { get; set; } = null!;
        public DriftAlert DriftAlert { get; set; } = null!;
    }
}

