using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.Interfaces;
using MiApp.Application.DTOs.Students;

namespace MiApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClassmateController(
    IClassmateService classmateService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClassmatesResponseDto>>> GetAll()
    {
        var response = await classmateService.GetAllAsync();

        return Ok(response);
    }
}
