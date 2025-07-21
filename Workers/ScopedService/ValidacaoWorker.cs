using EmprestimosWorkerService.Interfaces;

namespace EmprestimosWorkerService.Workers.ScopedService;

/* Scoped Background Service
    - Dentro de um BackgroundService, você pode criar escopos manuais para injetar dependências do tipo Scoped (como um DbContext).
Quando usar?
    - Quando o seu serviço precisa de uma dependência que não pode ser Singleton.
 */

public class ValidacaoWorkerScopedService(IServiceProvider serviceProvider, ILogger<ValidacaoWorkerScopedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[ValidacaoWorkerScopedService] - Iniciando serviço de validação de contratos por escopo.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var escopo = serviceProvider.CreateScope();

                try
                {
                    var validador = escopo.ServiceProvider.GetRequiredService<IValidacaoEmprestimo>();
                    var contratoId = Guid.NewGuid().ToString();

                    logger.LogInformation("[ValidacaoWorkerScopedService] - Iniciando validação para o contrato: {ContratoId}", contratoId);

                    await validador.ValidarAsync(contratoId);

                    logger.LogInformation("[ValidacaoWorkerScopedService] - Validação concluída para o contrato: {ContratoId}", contratoId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[ValidacaoWorkerScopedService] - Erro ao validar contrato dentro do escopo.");
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("[ValidacaoWorkerScopedService] - Cancelamento solicitado. Encerrando serviço.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[ValidacaoWorkerScopedService] - Erro inesperado no serviço de validação.");
        }
        finally
        {
            logger.LogInformation("[ValidacaoWorkerScopedService] - Serviço de validação finalizado.");
        }
    }
}
