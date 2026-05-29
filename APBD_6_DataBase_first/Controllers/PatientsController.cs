using APBD_6_DataBase_first.DTOs;
using APBD_6_DataBase_first.Exceptions;
using APBD_6_DataBase_first.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_6_DataBase_first.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PatientsController : Controller
{
    private readonly IDbService _dbService;

    public PatientsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPatientsWithAdmissions([FromQuery] string? search)
    {
        try
        {
            var result = await _dbService.GetPatientsWithAdmissions(search);
            return Ok(result);
            
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        
        
    }

    [HttpPost]
    [Route("{pesel}/bedassigments")]
    public async Task<IActionResult> AssignPatientBed([FromRoute]string pesel,[FromBody] AssignBedDto assignBedDto )
    {
        try
        {
            
            await _dbService.AssignBedToPatient(pesel, assignBedDto);
            return Created();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
}