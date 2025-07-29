using EmprestimosWorkerService.Interfaces;
namespace EmprestimosWorkerService.Services;

public class ValidacaoEmprestimoSingletonService(ILogger<ValidacaoEmprestimoSingletonService> logger) : IValidacaoEmprestimoSingleton
{
    public Task ValidarAsync(string contratoId)
    {
        logger.LogInformation($"Validando contrato Singleton: {contratoId}");
        if (contratoId is null || contratoId.Length < 5)
            logger.LogError($"Contrato Singleton {contratoId} inválido.");
        logger.LogInformation($"Contrato Singleton {contratoId} validado com sucesso.");
        return Task.CompletedTask;
    }
}
