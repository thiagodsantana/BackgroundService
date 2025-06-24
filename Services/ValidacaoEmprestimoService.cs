using EmprestimosWorkerService.Interfaces;

namespace EmprestimosWorkerService.Services;

public class ValidacaoEmprestimoService(ILogger<ValidacaoEmprestimoService> logger) : IValidacaoEmprestimo
{
    public Task ValidarAsync(string contratoId)
    {
        logger.LogInformation($"Validando contrato: {contratoId}");
        return Task.CompletedTask;
    }
}
