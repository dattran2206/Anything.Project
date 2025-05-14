using Base.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Shell.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IDataBaseService _db;

    public WeatherForecastController(IDataBaseService db)
    {
        _db = db;
    }

    [HttpGet("param1/param2")]
    public async Task<IActionResult> Demo(string Param1, string Param2)
    {
        var parameters = new
        {
            param1 = Param1,
            param2 = Param2
        };
        var result = await _db.ExcuteStoreAsync("sp_Getbasic", parameters);
        return Ok(result);
    }

}
