using Cocus.Domain.DTOs;
using Cocus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cocus.Mvc.Controllers;

[Authorize]
public class AircraftController : Controller
{
	private readonly IAircraftService _aircraftService;

	public AircraftController(IAircraftService aircraftService)
	{
		_aircraftService = aircraftService;
	}

	// GET: Aircraft
	public async Task<IActionResult> Index()
	{
		var aircraft = await _aircraftService.GetAllAsync();
		return View(aircraft);
	}

	// GET: Aircraft/Details/5
	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var aircraft = await _aircraftService.GetByIdAsync(id.Value);
		if (aircraft == null)
		{
			return NotFound();
		}

		return View(aircraft);
	}

	// GET: Aircraft/Create
	public IActionResult Create()
	{
		return View();
	}

	// POST: Aircraft/Create
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(AircraftDto aircraftDto)
	{
		if (ModelState.IsValid)
		{
			var validationError = await _aircraftService.ValidateAircraftAsync(aircraftDto);
			if (validationError != null)
			{
				ModelState.AddModelError("", validationError);
				return View(aircraftDto);
			}

			try
			{
				await _aircraftService.CreateAsync(aircraftDto);
				TempData["SuccessMessage"] = "Aircraft created successfully.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Error creating aircraft: {ex.Message}");
			}
		}
		return View(aircraftDto);
	}

	// GET: Aircraft/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var aircraft = await _aircraftService.GetByIdAsync(id.Value);
		if (aircraft == null)
		{
			return NotFound();
		}
		return View(aircraft);
	}

	// POST: Aircraft/Edit/5
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, AircraftDto aircraftDto)
	{
		if (id != aircraftDto.Id)
		{
			return NotFound();
		}

		if (ModelState.IsValid)
		{
			var validationError = await _aircraftService.ValidateAircraftAsync(aircraftDto);
			if (validationError != null)
			{
				ModelState.AddModelError("", validationError);
				return View(aircraftDto);
			}

			try
			{
				await _aircraftService.UpdateAsync(aircraftDto);
				TempData["SuccessMessage"] = "Aircraft updated successfully.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Error updating aircraft: {ex.Message}");
			}
		}
		return View(aircraftDto);
	}

	// GET: Aircraft/Delete/5
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var aircraft = await _aircraftService.GetByIdAsync(id.Value);
		if (aircraft == null)
		{
			return NotFound();
		}

		return View(aircraft);
	}

	// POST: Aircraft/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		try
		{
			await _aircraftService.DeleteAsync(id);
			TempData["SuccessMessage"] = "Aircraft deleted successfully.";
		}
		catch (Exception ex)
		{
			TempData["ErrorMessage"] = $"Error deleting aircraft: {ex.Message}";
		}

		return RedirectToAction(nameof(Index));
	}
}
