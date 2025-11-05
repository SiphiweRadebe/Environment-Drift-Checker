using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentDriftChecker.Domain.Enums
{
    public enum DriftStatus
    {
        Clean,      // No drifts detected
        Warning,    // 1-5 drifts
        Critical    // More than 5 drifts
    }
}
