using Microsoft.AspNetCore.Mvc;
using ApiElecateProspectsForm.Interfaces.Repositories;

namespace ApiElecateProspectsForm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaritalStatusController(IMaritalStatusRepository repository) : Controller
    {
        private readonly IMaritalStatusRepository _repository = repository;

        [HttpGet]
        public async Task<IActionResult> GetMaritalStatuses()
        {
            var maritalStatuses = await _repository.GetAllMaritalStatusesAsync();
            return Ok(maritalStatuses);
        }
    }
}
