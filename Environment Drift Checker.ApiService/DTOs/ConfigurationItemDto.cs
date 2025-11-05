namespace EnvironmentDriftChecker.ApiService.DTOs
{
    public class ConfigurationItemDto
    {
        public int Id { get; set; }
        public int EnvironmentId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public bool IsSensitive { get; set; }
    }

    public class CreateConfigurationItemDto
    {
        public int EnvironmentId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsSensitive { get; set; }
    }

    public class UpdateConfigurationItemDto
    {
        public string Value { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsSensitive { get; set; }
    }
}