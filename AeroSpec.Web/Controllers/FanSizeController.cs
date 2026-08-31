using AeroSpec.Database;
using AeroSpec.Repositories.Contracts;
using AeroSpec.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AeroSpec.Web.Controllers;

public class FanSizeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FanSizeController> _logger;

    public FanSizeController(IUnitOfWork unitOfWork, ILogger<FanSizeController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sizes = await _unitOfWork.FanSizeRepository.GetAllActiveAsync();

        var viewModel = sizes.Select(s => new FanSizeViewModel
        {
            Id = s.Id,
            SizeId = s.SizeId,
            DiameterIn = s.DiameterIn,
            CfmScale = s.CfmScale,
            SpScale = s.SpScale,
            MaxRpm = s.MaxRpm,
            OutletArea = s.OutletArea,
            WeightBase = s.WeightBase,
            BasePrice = s.BasePrice
        }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new FanSizeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FanSizeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await _unitOfWork.FanSizeRepository.ExistsAsync(model.SizeId))
        {
            ModelState.AddModelError(nameof(model.SizeId), "A fan size with this Size ID already exists.");
            return View(model);
        }

        var entity = new FanSize
        {
            SizeId = model.SizeId,
            DiameterIn = model.DiameterIn,
            CfmScale = model.CfmScale,
            SpScale = model.SpScale,
            MaxRpm = model.MaxRpm,
            OutletArea = model.OutletArea,
            WeightBase = model.WeightBase,
            BasePrice = model.BasePrice
        };

        await _unitOfWork.FanSizeRepository.AddAsync(entity);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _unitOfWork.FanSizeRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var viewModel = new FanSizeViewModel
        {
            Id = entity.Id,
            SizeId = entity.SizeId,
            DiameterIn = entity.DiameterIn,
            CfmScale = entity.CfmScale,
            SpScale = entity.SpScale,
            MaxRpm = entity.MaxRpm,
            OutletArea = entity.OutletArea,
            WeightBase = entity.WeightBase,
            BasePrice = entity.BasePrice
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FanSizeViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entity = await _unitOfWork.FanSizeRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.SizeId = model.SizeId;
        entity.DiameterIn = model.DiameterIn;
        entity.CfmScale = model.CfmScale;
        entity.SpScale = model.SpScale;
        entity.MaxRpm = model.MaxRpm;
        entity.OutletArea = model.OutletArea;
        entity.WeightBase = model.WeightBase;
        entity.BasePrice = model.BasePrice;

        await _unitOfWork.FanSizeRepository.UpdateAsync(entity);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _unitOfWork.FanSizeRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}