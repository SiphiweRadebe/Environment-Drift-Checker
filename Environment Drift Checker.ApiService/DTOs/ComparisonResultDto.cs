namespace EnvironmentDriftChecker.ApiService.DTOs
{
    public class ComparisonResultDto
    {
        public int RecordId { get; set; }
        public EnvironmentSummaryDto SourceEnvironment { get; set; } = null!;
        public EnvironmentSummaryDto TargetEnvironment { get; set; } = null!;
        public DateTime ComparisonDate { get; set; }
        public int TotalKeys { get; set; }
        public int DriftCount { get; set; }
        public int MatchCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<DriftDetailDto> Drifts { get; set; } = new();
    }

    public class EnvironmentSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class DriftDetailDto
    {
        public string Key { get; set; } = string.Empty;
        public string? SourceValue { get; set; }
        public string? TargetValue { get; set; }
        public string DriftType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}