using AeroSpec.Database;
using AeroSpec.Models.DTOs;

namespace AeroSpec.Business.Contracts;

public interface IQuoteService
{
    Task<QuoteDto> GenerateQuoteAsync(
        FanEvaluationResultDto selectedFan,
        SpecificationInputDto specification,
        FanType fanType);

    int CalculateUnitPrice(FanSize fanSize, FanType fanType, string arrangement);
    int CalculateFreight(int subtotal);
}