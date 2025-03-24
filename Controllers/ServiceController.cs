using ApiElecateProspectsForm.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiElecateProspectsForm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController(IServiceRepository repository) : Controller
    {
        private readonly IServiceRepository _repository = repository;

        [HttpGet]
        public IActionResult GetServices()
        {
            IQueryable<Models.ServiceModel> services = _repository.GetAllServices();
            return Ok(services);
        }
    }
}
