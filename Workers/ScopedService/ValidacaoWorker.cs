using EmprestimosWorkerService.Interfaces;

namespace EmprestimosWorkerService.Workers.ScopedService;

/* Scoped Background Service
    - Dentro de um BackgroundService, você pode criar escopos manuais para injetar dependências do tipo Scoped (como um DbContext).
Quando usar?
    - Quando o seu serviço precisa de uma dependência que não pode ser Singleton.
 */

public class ValidacaoWorker(IServiceProvider serviceProvider, IValidacaoEmprestimoSingleton validacaoEmprestimoSingleton, ILogger<ValidacaoWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogCritical("====== ValidacaoWorkerScopedService ======");
        logger.LogInformation("");
        logger.LogInformation("[ValidacaoWorkerScopedService] - Iniciando serviço de validação de contratos por escopo.");
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
        try
        {
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var escopo = serviceProvider.CreateScope();
                try
                {
                    string contratoId = string.Empty;
                    contratoId = Guid.NewGuid().ToString();

                    logger.LogInformation("[ValidacaoWorkerSingletonService] - Iniciando validação para o contrato Singleton: {ContratoId}", contratoId);

                    await validacaoEmprestimoSingleton.ValidarAsync(contratoId);

                    logger.LogInformation("[ValidacaoWorkerSingletonService] - Validação concluída para o contrato: {ContratoId}", contratoId);


                    var validador = escopo.ServiceProvider.GetRequiredService<IValidacaoEmprestimo>();
                    contratoId = Guid.NewGuid().ToString();

                    logger.LogInformation("[ValidacaoWorkerScopedService] - Iniciando validação para o contrato: {ContratoId}", contratoId);

                    await validador.ValidarAsync(contratoId);

                    logger.LogInformation("[ValidacaoWorkerScopedService] - Validação concluída para o contrato: {ContratoId}", contratoId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[ValidacaoWorkerScopedService] - Erro ao validar contrato dentro do escopo.");
                }            
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
