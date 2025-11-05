using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentDriftChecker.Domain.Enums
{
    public enum DriftSeverity
    {
        Low,        // Non-critical settings
        Medium,     // API or database configs
        High        // Security or credentials
    }
}
