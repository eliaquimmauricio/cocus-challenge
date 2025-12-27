using Cocus.Domain.DTOs;
using Cocus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cocus.Mvc.Controllers;

[Authorize]
public class FlightsController : Controller
{
	private readonly IFlightService _flightService;

	public FlightsController(IFlightService flightService)
	{
		_flightService = flightService;
	}

	public async Task<IActionResult> Index()
	{
		var flights = await _flightService.GetAllAsync();
		return View(flights);
	}

	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var flight = await _flightService.GetByIdAsync(id.Value);
		if (flight == null)
		{
			return NotFound();
		}

		return View(flight);
	}

	public async Task<IActionResult> Create()
	{
		var formData = await _flightService.GetFlightFormDataAsync();
		PopulateViewBag(formData);
		return View(new FlightDto { ScheduledDeparture = DateTime.Now.AddDays(1) });
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(FlightDto flightDto)
	{
		var validationError = await _flightService.ValidateFlightAsync(flightDto);
		if (validationError != null)
		{
			ModelState.AddModelError("DestinationAirportId", validationError);
		}

		if (ModelState.IsValid)
		{
			try
			{
				await _flightService.CreateAsync(flightDto);
				TempData["SuccessMessage"] = "Flight created successfully with calculated distance and fuel requirements.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Error creating flight: {ex.Message}");
			}
		}

		var formData = await _flightService.GetFlightFormDataAsync();
		PopulateViewBag(formData, flightDto.DepartureAirportId, flightDto.DestinationAirportId, flightDto.AircraftId);
		return View(flightDto);
	}

	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var flight = await _flightService.GetByIdAsync(id.Value);
		if (flight == null)
		{
			return NotFound();
		}

		var formData = await _flightService.GetFlightFormDataAsync();
		PopulateViewBag(formData, flight.DepartureAirportId, flight.DestinationAirportId, flight.AircraftId);
		return View(flight);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, FlightDto flightDto)
	{
		if (id != flightDto.Id)
		{
			return NotFound();
		}

		var validationError = await _flightService.ValidateFlightAsync(flightDto);
		if (validationError != null)
		{
			ModelState.AddModelError("DestinationAirportId", validationError);
		}

		if (ModelState.IsValid)
		{
			try
			{
				await _flightService.UpdateAsync(flightDto);
				TempData["SuccessMessage"] = "Flight updated successfully. Distance and fuel requirements recalculated.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Error updating flight: {ex.Message}");
			}
		}

		var formData = await _flightService.GetFlightFormDataAsync();
		PopulateViewBag(formData, flightDto.DepartureAirportId, flightDto.DestinationAirportId, flightDto.AircraftId);
		return View(flightDto);
	}

	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var flight = await _flightService.GetByIdAsync(id.Value);
		if (flight == null)
		{
			return NotFound();
		}

		return View(flight);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		try
		{
			await _flightService.DeleteAsync(id);
			TempData["SuccessMessage"] = "Flight deleted successfully.";
			return RedirectToAction(nameof(Index));
		}
		catch (Exception ex)
		{
			TempData["ErrorMessage"] = $"Error deleting flight: {ex.Message}";
			return RedirectToAction(nameof(Index));
		}
	}

	private void PopulateViewBag(FlightFormDataDto formData, int? selectedDeparture = null, int? selectedDestination = null, int? selectedAircraft = null)
	{
		ViewBag.Airports = new SelectList(formData.DepartureAirports, "Id", "Display", selectedDeparture);
		ViewBag.DestinationAirports = new SelectList(formData.DestinationAirports, "Id", "Display", selectedDestination);
		ViewBag.Aircraft = new SelectList(formData.Aircraft, "Id", "Display", selectedAircraft);
	}
}
