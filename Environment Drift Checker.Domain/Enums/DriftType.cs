using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentDriftChecker.Domain.Enums
{
    public enum DriftType
    {
        Match,              // Values are the same
        Different,          // Values differ
        MissingInSource,    // Key exists in target but not source
        MissingInTarget     // Key exists in source but not target
    }
}
