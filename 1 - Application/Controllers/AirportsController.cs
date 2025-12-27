using Cocus.Domain.DTOs;
using Cocus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cocus.Mvc.Controllers;

[Authorize]
public class AirportsController : Controller
{
	private readonly IAirportService _airportService;

	public AirportsController(IAirportService airportService)
	{
		_airportService = airportService;
	}

	public async Task<IActionResult> Index()
	{
		var airports = await _airportService.GetAllAsync();
		return View(airports);
	}

	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var airport = await _airportService.GetByIdAsync(id.Value);
		if (airport == null)
		{
			return NotFound();
		}

		return View(airport);
	}

	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(AirportDto airportDto)
	{
		if (ModelState.IsValid)
		{
			var validationError = await _airportService.ValidateAirportAsync(airportDto);
			if (validationError != null)
			{
				ModelState.AddModelError("Code", validationError);
				return View(airportDto);
			}

			try
			{
				await _airportService.CreateAsync(airportDto);
				TempData["SuccessMessage"] = "Airport created successfully.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Error creating airport: {ex.Message}");
			}
		}
		return View(airportDto);
	}

	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var airport = await _airportService.GetByIdAsync(id.Value);
		if (airport == null)
		{
			return NotFound();
		}
		return View(airport);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, AirportDto airportDto)
	{
		if (id != airportDto.Id)
		{
			return NotFound();
		}

		if (ModelState.IsValid)
		{
			var validationError = await _airportService.ValidateAirportAsync(airportDto);
			if (validationError != null)
			{
				ModelState.AddModelError("Code", validationError);
				return View(airportDto);
			}

			try
			{
				await _airportService.UpdateAsync(airportDto);
				TempData["SuccessMessage"] = "Airport updated successfully.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Error updating airport: {ex.Message}");
			}
		}
		return View(airportDto);
	}

	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var airport = await _airportService.GetByIdAsync(id.Value);
		if (airport == null)
		{
			return NotFound();
		}

		return View(airport);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		try
		{
			await _airportService.DeleteAsync(id);
			TempData["SuccessMessage"] = "Airport deleted successfully.";
		}
		catch (Exception ex)
		{
			TempData["ErrorMessage"] = $"Error deleting airport: {ex.Message}";
		}

		return RedirectToAction(nameof(Index));
	}
}
