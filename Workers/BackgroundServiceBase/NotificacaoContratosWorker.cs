namespace EmprestimosWorkerService.Workers.BackgroundServiceBase;

/*BackgroundService (Base com ExecuteAsync)
  - Derivado diretamente de BackgroundService
Quando usar?
  - Para tarefas contínuas ou com polling (checagens regulares).
 */

public class NotificacaoContratosWorker(ILogger<NotificacaoContratosWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[NotificacaoContratosWorkerBackgroundService] - Serviço de Notificação de Contratos iniciado.");
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("[NotificacaoContratosWorkerBackgroundService] - Iniciando envio de notificações de contratos...");

                await EnviarNotificacoesAsync(stoppingToken);

                logger.LogInformation("[NotificacaoContratosWorkerBackgroundService] - Ciclo de envio de notificações concluído. Aguardando próximo ciclo...");
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("[NotificacaoContratosWorkerBackgroundService] - Cancelamento solicitado. Encerrando o serviço de notificações.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[NotificacaoContratosWorkerBackgroundService] - Erro inesperado durante o envio de notificações de contratos.");
        }
        finally
        {
            logger.LogInformation("[NotificacaoContratosWorkerBackgroundService] - Serviço de Notificação de Contratos finalizado.");
        }
    }

    private async Task EnviarNotificacoesAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Simulação de envio (Ex: envio de e-mails, SMS, etc.)
            logger.LogInformation("[NotificacaoContratosWorkerBackgroundService] - Notificações enviadas com sucesso.");

            await Task.Delay(5000, cancellationToken); // Simulação de tempo de envio
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[NotificacaoContratosWorkerBackgroundService] - Falha ao tentar enviar as notificações. Tentará novamente no próximo ciclo.");
        }
    }
}
