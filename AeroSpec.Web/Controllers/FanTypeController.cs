
using AeroSpec.Database;
using AeroSpec.Repositories.Contracts;
using AeroSpec.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AeroSpec.Web.Controllers;

public class FanTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FanTypeController> _logger;

    public FanTypeController(IUnitOfWork unitOfWork, ILogger<FanTypeController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var types = await _unitOfWork.FanTypeRepository.GetAllActiveAsync();

        var viewModel = types.Select(t => new FanTypeViewModel
        {
            Id = t.Id,
            TypeId = t.TypeId,
            Label = t.Label,
            Prefix = t.Prefix,
            SpMod = t.SpMod,
            EffMod = t.EffMod
        }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new FanTypeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FanTypeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await _unitOfWork.FanTypeRepository.ExistsAsync(model.TypeId))
        {
            ModelState.AddModelError(nameof(model.TypeId), "A fan type with this Type ID already exists.");
            return View(model);
        }

        var entity = new FanType
        {
            TypeId = model.TypeId,
            Label = model.Label,
            Prefix = model.Prefix,
            SpMod = model.SpMod,
            EffMod = model.EffMod
        };

        await _unitOfWork.FanTypeRepository.AddAsync(entity);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _unitOfWork.FanTypeRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var viewModel = new FanTypeViewModel
        {
            Id = entity.Id,
            TypeId = entity.TypeId,
            Label = entity.Label,
            Prefix = entity.Prefix,
            SpMod = entity.SpMod,
            EffMod = entity.EffMod
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FanTypeViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entity = await _unitOfWork.FanTypeRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.TypeId = model.TypeId;
        entity.Label = model.Label;
        entity.Prefix = model.Prefix;
        entity.SpMod = model.SpMod;
        entity.EffMod = model.EffMod;

        await _unitOfWork.FanTypeRepository.UpdateAsync(entity);

        return RedirectToAction(nameof(Index));
    }
}