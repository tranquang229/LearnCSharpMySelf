using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestDependencyApiLife.Interfaces;
using TestDependencyApiLife.Services;

namespace TestDependencyApiLife.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OperationsController : ControllerBase
    {
        private readonly OperationService _operationService;
        private readonly IOperationTransient _transientOperation;
        private readonly IOperationScoped _scopedOperation;
        private readonly IOperationSingleton _singletonOperation;
        private readonly IOperationSingletonInstance _singletonInstanceOperation;

        public OperationsController(OperationService operationService,
            IOperationTransient transientOperation,
            IOperationScoped scopedOperation,
            IOperationSingleton singletonOperation,
            IOperationSingletonInstance singletonInstanceOperation)
        {
            _operationService = operationService;
            _transientOperation = transientOperation;
            _scopedOperation = scopedOperation;
            _singletonOperation = singletonOperation;
            _singletonInstanceOperation = singletonInstanceOperation;
        }

        public IActionResult Index()
        {
            // ViewBag contains controller-requested services
            var transient = _transientOperation;
            var scoped = _scopedOperation;
            var singleton = _singletonOperation;
            var singletonInstance = _singletonInstanceOperation;

            // Operation service has its own requested services
            var service = _operationService;
            var serviceString = JsonConvert.SerializeObject(service);

            return Ok(new
            {
                TransitionId = transient.OperationId,
                ScopedId = scoped.OperationId,
                SingletonId = singleton.OperationId,
                SingletonOperationId = singletonInstance.OperationId,
                servicesString = serviceString,
            });
        }
    }
}
