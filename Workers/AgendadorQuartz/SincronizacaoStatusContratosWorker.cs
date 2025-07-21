using Quartz;

namespace EmprestimosWorkerService.Workers.AgendadorQuartz;

/*Serviços com agendamento
 - Frameworks como Quartz.NET, Hangfire, ou Coravel oferecem recursos robustos para tarefas agendadas e cron jobs.
Quando usar?
    - Quando você precisa de controle fino, agendamentos complexos, ou persistência dos jobs.
 */

public class SincronizacaoStatusContratosWorker(ILogger<SincronizacaoStatusContratosWorker> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("[SincronizacaoStatusContratosWorkerQuartz] - Iniciando job de sincronização de status de contratos às {Hora}.", DateTime.Now);

        try
        {
            await SincronizarStatusContratosAsync();

            logger.LogInformation("[SincronizacaoStatusContratosWorkerQuartz] - Job de sincronização concluído com sucesso às {Hora}.", DateTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[SincronizacaoStatusContratosWorkerQuartz] - Erro durante a execução do job.");
        }
    }

    private static async Task SincronizarStatusContratosAsync()
    {
        // Lógica real de sincronização: consulta em APIs, banco de dados, etc.        
        await Task.Delay(500); // Simula tempo de execução
    }
}
