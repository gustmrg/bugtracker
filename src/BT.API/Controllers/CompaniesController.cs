using BT.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BT.API.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CompaniesController(ApplicationDbContext context)
    {
        _context = context;
    }
}