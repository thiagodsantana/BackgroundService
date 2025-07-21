using EmprestimosWorkerService.Interfaces;
namespace EmprestimosWorkerService.Services;

public class ValidacaoEmprestimoService(ILogger<ValidacaoEmprestimoService> logger) : IValidacaoEmprestimo
{
    public Task ValidarAsync(string contratoId)
    {
        logger.LogInformation($"Validando contrato: {contratoId}");
        if (contratoId is null || contratoId.Length < 5)
            logger.LogError($"Contrato {contratoId} inválido.");
        logger.LogInformation($"Contrato {contratoId} validado com sucesso.");
        return Task.CompletedTask;
    }
}
