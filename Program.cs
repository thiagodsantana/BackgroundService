using EmprestimosWorkerService.Interfaces;
using EmprestimosWorkerService.Services;
using EmprestimosWorkerService.Workers;
using Quartz;
using System.Threading.Channels;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Serviço contínuo: Responsável por enviar notificações de contratos
        services.AddHostedService<NotificacaoContratosWorkerBackgroundService>();

        // Serviço baseado em Timer: Geração de relatórios diários de empréstimos
        services.AddHostedService<RelatorioDiarioWorkerTimedService>();

        // Channel (fila na memória): Usado para enfileirar contratos para processamento assíncrono
        services.AddSingleton(Channel.CreateUnbounded<string>());

        // Serviço consumidor da fila de contratos
        services.AddHostedService<ContratosProcessorWorkerQueue>();

        // Registro do serviço de validação de contratos (injeção por escopo)
        services.AddScoped<IValidacaoEmprestimo, ValidacaoEmprestimoService>();

        // Serviço que consome o scoped service de validação dentro de um BackgroundService
        services.AddHostedService<ValidacaoWorkerScopedService>();

        // Serviço customizado implementado diretamente com IHostedService
        services.AddHostedService<WorkerCustomizadoHosted>();

        // Configuração do Quartz Scheduler para tarefas agendadas
        services.AddQuartz(q =>
        {
            // Define a chave única para o Job de sincronização de status de contratos
            var jobKey = new JobKey("SincronizacaoStatusContratosJob");

            // Registro do Job: Lógica de sincronização de status dos contratos com fontes externas
            q.AddJob<SincronizacaoStatusContratosWorkerQuartz>(opt => opt.WithIdentity(jobKey));

            // Configuração do Trigger: Executa o Job a cada 20 segundos (simulação)
            q.AddTrigger(opt => opt
                .ForJob(jobKey)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever()));
        });

        // Serviço que hospeda o Quartz no ciclo de vida da aplicação
        services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
    });

// Inicia o Host (aplicação Worker Service)
await builder.Build().RunAsync();
