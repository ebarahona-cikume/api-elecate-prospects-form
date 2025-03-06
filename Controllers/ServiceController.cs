using ApiElecateProspectsForm.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiElecateProspectsForm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _repository;
        public ServiceController(IServiceRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetServices()
        {
            var services = _repository.GetAllServices();
            return Ok(services);
        }
    }
}
