using EmprestimosWorkerService.Interfaces;

namespace EmprestimosWorkerService.Services;

public class ServicoValidacaoEmprestimo(ILogger<ServicoValidacaoEmprestimo> logger) : IValidacaoEmprestimo
{
    public Task ValidarAsync(string contratoId)
    {
        logger.LogInformation($"Validando contrato: {contratoId}");
        return Task.CompletedTask;
    }
}
