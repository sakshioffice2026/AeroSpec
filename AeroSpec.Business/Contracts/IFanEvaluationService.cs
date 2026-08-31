using AeroSpec.Database;
using AeroSpec.Models.DTOs;

namespace AeroSpec.Business.Contracts;

public interface IFanEvaluationService
{
    Task<List<FanEvaluationResultDto>> EvaluateAllFanSizesAsync(
        SpecificationInputDto input,
        FanType fanType,
        double densityRatio);

    Task<PerformanceCurveDto> BuildCurveDataAsync(
        FanEvaluationResultDto evalResult,
        SpecificationInputDto input,
        double densityRatio,
        FanType fanType);
}