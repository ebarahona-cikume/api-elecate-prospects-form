using ApiElecateProspectsForm.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiElecateProspectsForm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : Controller
    {
        private readonly IServiceReository _repository;
        public ServiceController(IServiceReository repository)
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
