using EnvironmentDriftChecker.ApiService.DTOs;
using EnvironmentDriftChecker.ApiService.Services.Interfaces;
using EnvironmentDriftChecker.Data.Repositories.Interfaces;
using EnvironmentDriftChecker.Domain.Entities;

namespace EnvironmentDriftChecker.ApiService.Services
{
    public class DriftCheckerService : IDriftCheckerService
    {
        private readonly IConfigurationItemRepository _configRepo;
        private readonly IDriftRecordRepository _driftRepo;
        private readonly IEnvironmentRepository _environmentRepo;
        private readonly ILogger<DriftCheckerService> _logger;

        public DriftCheckerService(
            IConfigurationItemRepository configRepo,
            IDriftRecordRepository driftRepo,
            IEnvironmentRepository environmentRepo,
            ILogger<DriftCheckerService> logger)
        {
            _configRepo = configRepo;
            _driftRepo = driftRepo;
            _environmentRepo = environmentRepo;
            _logger = logger;
        }

        public async Task<ComparisonResultDto> CompareEnvironmentsAsync(int sourceEnvId, int targetEnvId)
        {
            // Validate environments exist
            var sourceEnv = await _environmentRepo.GetByIdAsync(sourceEnvId);
            var targetEnv = await _environmentRepo.GetByIdAsync(targetEnvId);

            if (sourceEnv == null || targetEnv == null)
            {
                throw new KeyNotFoundException("One or both environments not found.");
            }

            // Get configurations
            var sourceConfigs = await _configRepo.GetByEnvironmentIdAsync(sourceEnvId);
            var targetConfigs = await _configRepo.GetByEnvironmentIdAsync(targetEnvId);

            var sourceDict = sourceConfigs.ToDictionary(c => c.Key, c => c);
            var targetDict = targetConfigs.ToDictionary(c => c.Key, c => c);

            var allKeys = sourceDict.Keys.Union(targetDict.Keys).Distinct();
            var driftDetails = new List<DriftDetail>();

            foreach (var key in allKeys)
            {
                var hasSource = sourceDict.TryGetValue(key, out var sourceItem);
                var hasTarget = targetDict.TryGetValue(key, out var targetItem);

                string driftType;
                string severity;
                string? sourceValue = null;
                string? targetValue = null;

                if (!hasSource)
                {
                    driftType = "MissingInSource";
                    severity = DetermineSeverity(targetItem!.Category);
                    targetValue = targetItem.Value;
                }
                else if (!hasTarget)
                {
                    driftType = "MissingInTarget";
                    severity = DetermineSeverity(sourceItem!.Category);
                    sourceValue = sourceItem.Value;
                }
                else if (sourceItem!.Value != targetItem!.Value)
                {
                    driftType = "Different";
                    severity = DetermineSeverity(sourceItem.Category);
                    sourceValue = sourceItem.Value;
                    targetValue = targetItem.Value;
                }
                else
                {
                    driftType = "Match";
                    severity = "Low";
                    sourceValue = sourceItem.Value;
                    targetValue = targetItem.Value;
                }

                driftDetails.Add(new DriftDetail
                {
                    Key = key,
                    SourceValue = sourceValue,
                    TargetValue = targetValue,
                    DriftType = driftType,
                    Severity = severity
                });
            }

            var driftCount = driftDetails.Count(d => d.DriftType != "Match");
            var status = DetermineStatus(driftCount);

            // Save drift record
            var driftRecord = new DriftRecord
            {
                SourceEnvironmentId = sourceEnvId,
                TargetEnvironmentId = targetEnvId,
                TotalKeys = allKeys.Count(),
                DriftCount = driftCount,
                Status = status,
                DriftDetails = driftDetails
            };

            var savedRecord = await _driftRepo.AddAsync(driftRecord);
            _logger.LogInformation(
                "Comparison complete: {Source} vs {Target}, Drifts: {Count}",
                sourceEnv.Name,
                targetEnv.Name,
                driftCount);

            return MapToDto(savedRecord, sourceEnv, targetEnv);
        }

        public async Task<IEnumerable<ComparisonResultDto>> GetComparisonHistoryAsync()
        {
            var records = await _driftRepo.GetRecentAsync(20);
            return records.Select(r => MapToDto(r, r.SourceEnvironment, r.TargetEnvironment));
        }

        public async Task<IEnumerable<ComparisonResultDto>> GetEnvironmentHistoryAsync(int environmentId)
        {
            var records = await _driftRepo.GetByEnvironmentAsync(environmentId);
            return records.Select(r => MapToDto(r, r.SourceEnvironment, r.TargetEnvironment));
        }

        public async Task<ComparisonResultDto?> GetComparisonByIdAsync(int id)
        {
            var record = await _driftRepo.GetByIdAsync(id);
            if (record == null) return null;

            return MapToDto(record, record.SourceEnvironment, record.TargetEnvironment);
        }

        private string DetermineSeverity(string category)
        {
            return category.ToLower() switch
            {
                "security" => "High",
                "database" => "High",
                "api" => "Medium",
                "integration" => "Medium",
                _ => "Low"
            };
        }

        private string DetermineStatus(int driftCount)
        {
            if (driftCount == 0) return "Clean";
            if (driftCount <= 5) return "Warning";
            return "Critical";
        }

        private ComparisonResultDto MapToDto(DriftRecord record, Domain.Entities.Environment sourceEnv, Domain.Entities.Environment targetEnv)
        {
            return new ComparisonResultDto
            {
                RecordId = record.Id,
                SourceEnvironment = new EnvironmentSummaryDto { Id = sourceEnv.Id, Name = sourceEnv.Name },
                TargetEnvironment = new EnvironmentSummaryDto { Id = targetEnv.Id, Name = targetEnv.Name },
                ComparisonDate = record.ComparisonDate,
                TotalKeys = record.TotalKeys,
                DriftCount = record.DriftCount,
                MatchCount = record.TotalKeys - record.DriftCount,
                Status = record.Status,
                Drifts = record.DriftDetails.Select(d => new DriftDetailDto
                {
                    Key = d.Key,
                    SourceValue = d.SourceValue,
                    TargetValue = d.TargetValue,
                    DriftType = d.DriftType,
                    Severity = d.Severity,
                    Category = "" // Would need to join with ConfigurationItem to get this
                }).ToList()
            };
        }
    }
}