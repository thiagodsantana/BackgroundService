using EmprestimosWorkerService.Interfaces;
using EmprestimosWorkerService.Services;
using EmprestimosWorkerService.Workers;
using Quartz;
using System.Threading.Channels;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Servi�o cont�nuo: Respons�vel por enviar notifica��es de contratos
        services.AddHostedService<NotificacaoContratosWorkerBackgroundService>();

        // Servi�o baseado em Timer: Gera��o de relat�rios di�rios de empr�stimos
        services.AddHostedService<RelatorioDiarioWorkerTimedService>();

        // Channel (fila na mem�ria): Usado para enfileirar contratos para processamento ass�ncrono
        services.AddSingleton(Channel.CreateUnbounded<string>());

        // Servi�o consumidor da fila de contratos
        services.AddHostedService<ContratosProcessorWorkerQueue>();

        // Registro do servi�o de valida��o de contratos (inje��o por escopo)
        services.AddScoped<IValidacaoEmprestimo, ValidacaoEmprestimoService>();

        // Servi�o que consome o scoped service de valida��o dentro de um BackgroundService
        services.AddHostedService<ValidacaoWorkerScopedService>();

        // Servi�o customizado implementado diretamente com IHostedService
        services.AddHostedService<WorkerCustomizadoHosted>();

        // Configura��o do Quartz Scheduler para tarefas agendadas
        services.AddQuartz(q =>
        {
            // Define a chave �nica para o Job de sincroniza��o de status de contratos
            var jobKey = new JobKey("SincronizacaoStatusContratosJob");

            // Registro do Job: L�gica de sincroniza��o de status dos contratos com fontes externas
            q.AddJob<SincronizacaoStatusContratosWorkerQuartz>(opt => opt.WithIdentity(jobKey));

            // Configura��o do Trigger: Executa o Job a cada 20 segundos (simula��o)
            q.AddTrigger(opt => opt
                .ForJob(jobKey)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever()));
        });

        // Servi�o que hospeda o Quartz no ciclo de vida da aplica��o
        services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
    });

// Inicia o Host (aplica��o Worker Service)
await builder.Build().RunAsync();
