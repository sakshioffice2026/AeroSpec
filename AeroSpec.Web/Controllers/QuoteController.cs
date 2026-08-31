
using AeroSpec.Business.Contracts;
using AeroSpec.Models.DTOs;
using AeroSpec.Repositories.Contracts;
using AeroSpec.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AeroSpec.Web.Controllers;

public class QuoteController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFanEvaluationService _evaluationService;
    private readonly IFanCalculationService _calculationService;
    private readonly IQuoteService _quoteService;
    private readonly ILogger<QuoteController> _logger;

    public QuoteController(
        IUnitOfWork unitOfWork,
        IFanEvaluationService evaluationService,
        IFanCalculationService calculationService,
        IQuoteService quoteService,
        ILogger<QuoteController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _evaluationService = evaluationService ?? throw new ArgumentNullException(nameof(evaluationService));
        _calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));
        _quoteService = quoteService ?? throw new ArgumentNullException(nameof(quoteService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> Quote(int id)
    {
        var saved = await _unitOfWork.FanSelectionRepository.GetByIdAsync(id);
        if (saved == null)
        {
            return NotFound();
        }

        var fanType = await _unitOfWork.FanTypeRepository.GetByTypeIdAsync(saved.FanType);
        var fanSize = await _unitOfWork.FanSizeRepository.GetBySizeIdAsync(saved.SelectedFanId);

        if (fanType == null || fanSize == null)
        {
            return NotFound();
        }

        var input = new SpecificationInputDto
        {
            ProjectName = saved.ProjectName,
            Tag = saved.Tag,
            Cfm = saved.RequiredCfm,
            Sp = saved.RequiredSp,
            Elevation = saved.Elevation,
            Temperature = saved.Temperature,
            FanType = saved.FanType,
            Arrangement = saved.Arrangement,
            MaxTipSpeed = saved.MaxTipSpeed,
            SoundLimit = saved.SoundLimit,
            Qty = saved.Quantity
        };

        var densityRatio = _calculationService.ComputeDensityRatio(saved.Elevation, saved.Temperature);
        var allResults = await _evaluationService.EvaluateAllFanSizesAsync(input, fanType, densityRatio);
        var selectedResult = allResults.FirstOrDefault(r => r.Size.SizeId == saved.SelectedFanId);

        if (selectedResult == null)
        {
            return NotFound();
        }

        var quote = await _quoteService.GenerateQuoteAsync(selectedResult, input, fanType);

        var viewModel = new QuoteViewModel
        {
            ProjectName = saved.ProjectName,
            Tag = saved.Tag,
            SelectedSizeId = saved.SelectedFanId,
            FanTypeLabel = fanType.Label,
            Arrangement = saved.Arrangement,
            UnitPrice = quote.UnitPrice,
            Qty = quote.Qty,
            Subtotal = quote.Subtotal,
            Freight = quote.Freight,
            Total = quote.Total,
            QuoteDate = quote.QuoteDate
        };

        return View(viewModel);
    }
}