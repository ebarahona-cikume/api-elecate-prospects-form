using Microsoft.AspNetCore.Mvc;
using ApiElecateProspectsForm.Interfaces;

namespace ApiElecateProspectsForm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaritalStatusController : Controller
    {
        private readonly IMaritalStatusRepository _repository;
        public MaritalStatusController(IMaritalStatusRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetMaritalStatuses()
        {
            var maritalStatuses = _repository.GetAllMaritalStatuses();
            return Ok(maritalStatuses);
        }
    }
}
