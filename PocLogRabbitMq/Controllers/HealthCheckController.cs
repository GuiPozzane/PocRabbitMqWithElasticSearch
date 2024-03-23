using MassTransit;
using PocLogRabbitMq.Rabbitmq.Consumer;
using Serilog;
using System.Web.Http;

namespace PocLogRabbitMq.Controllers
{
    [RoutePrefix("api/healthcheck")]
    public class HealthCheckController : ApiController
    {
        private readonly IBus _bus;
        public HealthCheckController(IBus bus)
        {
            _bus = bus;
        }
        [HttpPost]
        public IHttpActionResult Post()
        {
            try
            {
                _bus.Publish(new CijunMessage { Text = "Hello, World!" });
                Log.Information("Processing a request to {ControllerName}", nameof(HealthCheckController));
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Handling an exception on {ControllerName}", nameof(HealthCheckController));
                return BadRequest();
            }
          
            // Your logic here
            return Ok();
        }
        [HttpGet]
        public IHttpActionResult Get()
        {
            Log.Information("Processing a request to {ControllerName}", nameof(HealthCheckController));
            // Your logic here
            try
            {
                int? teste = null;
                return Ok(teste.Value);
            }
            catch (System.Exception ex)
            {

                Log.Error(ex, "Handling an exception on {ControllerName} with IdUser {IdUser} and Name {Name}", nameof(HealthCheckController),100, "Um teste qualquer");
            }
            return Ok();
        }
    }
}

