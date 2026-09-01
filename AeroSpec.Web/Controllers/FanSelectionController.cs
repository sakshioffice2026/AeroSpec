using AeroSpec.Business.Contracts;
using AeroSpec.Models.DTOs;
using AeroSpec.Repositories.Contracts;
using AeroSpec.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AeroSpec.Web.Controllers;

public class FanSelectionController : Controller
{
    private readonly IFanSelectionService _fanSelectionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FanSelectionController> _logger;

    public FanSelectionController(
        IFanSelectionService fanSelectionService,
        IUnitOfWork unitOfWork,
        ILogger<FanSelectionController> logger)
    {
        _fanSelectionService = fanSelectionService ?? throw new ArgumentNullException(nameof(fanSelectionService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> Specification()
    {
        var fanTypes = await _unitOfWork.FanTypeRepository.GetAllActiveAsync();

        var viewModel = new SpecificationInputViewModel
        {
            AvailableFanTypes = fanTypes.Select(f => new FanTypeViewModel
            {
                Id = f.Id,
                TypeId = f.TypeId,
                Label = f.Label,
                Prefix = f.Prefix,
                SpMod = f.SpMod,
                EffMod = f.EffMod
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Specification(SpecificationInputViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var fanTypes = await _unitOfWork.FanTypeRepository.GetAllActiveAsync();
            model.AvailableFanTypes = fanTypes.Select(f => new FanTypeViewModel
            {
                Id = f.Id,
                TypeId = f.TypeId,
                Label = f.Label,
                Prefix = f.Prefix,
                SpMod = f.SpMod,
                EffMod = f.EffMod
            }).ToList();

            return View(model);
        }

        var input = new SpecificationInputDto
        {
            ProjectName = model.ProjectName,
            Tag = model.Tag,
            Cfm = model.Cfm,
            Sp = model.Sp,
            Elevation = model.Elevation,
            Temperature = model.Temperature,
            FanType = model.FanType,
            Arrangement = model.Arrangement,
            MaxTipSpeed = model.MaxTipSpeed,
            SoundLimit = model.SoundLimit,
            Qty = model.Qty
        };

        var result = await _fanSelectionService.ProcessSelectionAsync(input);

        if (!result.HasFeasibleFan)
        {
            TempData["SelectionMessage"] = result.Message ?? "No feasible fan size was found for the given specification.";
            return RedirectToAction(nameof(Specification));
        }

        return RedirectToAction(nameof(Results), new { id = result.SavedSelectionId });
    }

    [HttpGet]
    public async Task<IActionResult> Results(int id)
    {
        var saved = await _fanSelectionService.GetByIdAsync(id);
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

        var reprocessed = await _fanSelectionService.ProcessSelectionAsync(input);

        var viewModel = new FanSelectionResultViewModel
        {
            SavedSelectionId = id,
            HasFeasibleFan = reprocessed.HasFeasibleFan,
            Message = reprocessed.Message,
            DensityRatio = reprocessed.DensityRatio,
            Specification = new SpecificationInputViewModel
            {
                ProjectName = input.ProjectName,
                Tag = input.Tag,
                Cfm = input.Cfm,
                Sp = input.Sp,
                Elevation = input.Elevation,
                Temperature = input.Temperature,
                FanType = input.FanType,
                Arrangement = input.Arrangement,
                MaxTipSpeed = input.MaxTipSpeed,
                SoundLimit = input.SoundLimit,
                Qty = input.Qty
            },
            AllResults = reprocessed.AllResults.Select(r => new FanEvaluationResultViewModel
            {
                SizeId = r.Size.SizeId,
                DiameterIn = r.Size.DiameterIn,
                Feasible = r.Feasible,
                Reason = r.Reason,
                Rpm = r.N,
                EffPct = r.EffPct,
                Bhp = r.Bhp,
                MotorHp = r.MotorHp,
                TipSpeed = r.TipSpeed,
                OutletVelocity = r.OutletVelocity,
                SoundDba = r.SoundDba,
                IsSelected = r.Size.SizeId == saved.SelectedFanId
            }).ToList()
        };

        viewModel.SelectedFan = viewModel.AllResults.FirstOrDefault(r => r.IsSelected);

        viewModel.Curve = new PerformanceCurveViewModel
        {
            SelectedSizeId = saved.SelectedFanId,
            DutyCfm = saved.RequiredCfm,
            DutySp = saved.RequiredSp,
            Points = reprocessed.Curve.Points.Select(p => new CurvePointViewModel
            {
                Cfm = p.Cfm,
                FanSp = p.FanSp,
                Eff = p.Eff,
                SysSp = p.SysSp
            }).ToList(),
            GhostLow = reprocessed.Curve.GhostLow.Select(g => new GhostPointViewModel { Cfm = g.Cfm, Sp = g.Sp }).ToList(),
            GhostHigh = reprocessed.Curve.GhostHigh.Select(g => new GhostPointViewModel { Cfm = g.Cfm, Sp = g.Sp }).ToList()
        };

        viewModel.Quote = new QuoteViewModel
        {
            ProjectName = input.ProjectName,
            Tag = input.Tag,
            SelectedSizeId = saved.SelectedFanId,
            FanTypeLabel = fanType.Label,
            Arrangement = input.Arrangement,
            UnitPrice = reprocessed.Quote.UnitPrice,
            Qty = reprocessed.Quote.Qty,
            Subtotal = reprocessed.Quote.Subtotal,
            Freight = reprocessed.Quote.Freight,
            Total = reprocessed.Quote.Total,
            QuoteDate = reprocessed.Quote.QuoteDate
        };

        return View("CentriSelect", viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> History()
    {
        var selections = await _fanSelectionService.GetHistoryAsync();

        var viewModel = selections.Select(s => new FanSelectionHistoryViewModel
        {
            Id = s.Id,
            ProjectName = s.ProjectName,
            Tag = s.Tag,
            RequiredCfm = s.RequiredCfm,
            RequiredSp = s.RequiredSp,
            FanType = s.FanType,
            SelectedFanId = s.SelectedFanId,
            SelectedRpm = s.SelectedRpm,
            SelectedMotorHp = s.SelectedMotorHp,
            SelectedEfficiency = s.SelectedEfficiency,
            Quantity = s.Quantity,
            CreatedDate = s.CreatedDate
        }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var saved = await _fanSelectionService.GetByIdAsync(id);
        if (saved == null)
        {
            return NotFound();
        }

        var viewModel = new FanSelectionHistoryViewModel
        {
            Id = saved.Id,
            ProjectName = saved.ProjectName,
            Tag = saved.Tag,
            RequiredCfm = saved.RequiredCfm,
            RequiredSp = saved.RequiredSp,
            FanType = saved.FanType,
            SelectedFanId = saved.SelectedFanId,
            SelectedRpm = saved.SelectedRpm,
            SelectedMotorHp = saved.SelectedMotorHp,
            SelectedEfficiency = saved.SelectedEfficiency,
            Quantity = saved.Quantity,
            CreatedDate = saved.CreatedDate
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _fanSelectionService.DeleteAsync(id);
        return RedirectToAction(nameof(History));
    }
}