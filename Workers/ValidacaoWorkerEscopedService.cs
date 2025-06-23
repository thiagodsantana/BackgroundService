using EmprestimosWorkerService.Interfaces;

namespace EmprestimosWorkerService.Workers;

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
                    string contratoId = Guid.NewGuid().ToString();

                    logger.LogInformation("[ValidacaoWorkerScopedService] - Iniciando validação para o contrato: {ContratoId}", contratoId);

                    await validador.ValidarAsync(contratoId);

                    logger.LogInformation("[ValidacaoWorkerScopedService] - Validação concluída para o contrato: {ContratoId}", contratoId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[ValidacaoWorkerScopedService] - Erro durante a validação de um contrato.");
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("[ValidacaoWorkerScopedService] - Cancelamento solicitado. Encerrando serviço de validação por escopo.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[ValidacaoWorkerScopedService] - Erro inesperado no serviço de validação por escopo.");
        }
        finally
        {
            logger.LogInformation("[ValidacaoWorkerScopedService] - Serviço de validação de contratos finalizado.");
        }
    }
}