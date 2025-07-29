using Quartz;
namespace EmprestimosWorkerService.Workers.AgendadorQuartz;
public class SincronizacaoStatusContratosWorker(ILogger<SincronizacaoStatusContratosWorker> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogCritical("====== SincronizacaoStatusContratosWorkerQuartz ======");
        logger.LogInformation("");
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
        await Task.Delay(500); // Simula tempo de execução
    }
}