using AeroSpec.Business.Contracts;
using AeroSpec.Database;
using AeroSpec.Models.DTOs;

namespace AeroSpec.Business.Services;

public class QuoteService : IQuoteService
{
    public async Task<QuoteDto> GenerateQuoteAsync(
        FanEvaluationResultDto selectedFan,
        SpecificationInputDto specification,
        FanType fanType)
    {
        var unitPrice = CalculateUnitPrice(selectedFan.Size, fanType, specification.Arrangement);
        var qty = specification.Qty;
        var subtotal = unitPrice * qty;
        var freight = CalculateFreight(subtotal);
        var total = subtotal + freight;

        return new QuoteDto
        {
            SelectedFan = selectedFan,
            Specification = specification,
            FanTypeInfo = fanType,
            UnitPrice = unitPrice,
            Qty = qty,
            Subtotal = subtotal,
            Freight = freight,
            Total = total,
            QuoteDate = DateTime.Now
        };
    }

    public int CalculateUnitPrice(FanSize fanSize, FanType fanType, string arrangement)
    {
        var basePrice = fanSize.BasePrice;
        var arrangementMultiplier = arrangement.Contains("Belt") ? 1.08 : 1.0;
        var motorCost = fanSize.WeightBase * 95 / 10;
        var typeSurcharge = fanType.TypeId == "housed_direct" ? 400 : 0;

        return (int)Math.Round(basePrice * arrangementMultiplier + motorCost + typeSurcharge);
    }

    public int CalculateFreight(int subtotal)
    {
        return (int)Math.Round(subtotal * 0.06);
    }
}