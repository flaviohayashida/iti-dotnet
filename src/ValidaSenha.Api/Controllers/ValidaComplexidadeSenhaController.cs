using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ValidaSenha.Api.Services;
using ValidaSenha.Api.ViewModel;

namespace ValidaSenha.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ValidaComplexidadeSenhaController : ControllerBase
    {
        private readonly ILogger<ValidaComplexidadeSenhaController> _logger;
        private readonly ValidaSenhaService _service;

        public ValidaComplexidadeSenhaController(
            ILogger<ValidaComplexidadeSenhaController> logger,
            ValidaSenhaService service
            )
        {
            this._logger = logger;
            this._service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ValidaSenhaRequest request)
        {
            try
            {
                if (request is null) return BadRequest();
                var success = _service.Valida(request.Senha);
                return await Task.FromResult<IActionResult>(Ok(new { success = success }));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "OOoops!");
                return Problem(
                    type: "http://itivalidacomplexidadesenha/internalerror",
                    title: $"One or more model errors occurred. traceId: {ControllerContext.HttpContext.TraceIdentifier}",
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: ex.Message,
                    instance: Request.Path
                    );
            }
        }
    }
}