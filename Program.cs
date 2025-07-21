using EmprestimosWorkerService.Interfaces;
using EmprestimosWorkerService.Services;
using EmprestimosWorkerService.Workers;
using EmprestimosWorkerService.Workers.AgendadorQuartz;
using EmprestimosWorkerService.Workers.BackgroundServiceBase;
using EmprestimosWorkerService.Workers.HostedBase;
using EmprestimosWorkerService.Workers.ScopedService;
using EmprestimosWorkerService.Workers.TimedServiceBase;
using Quartz;
using System.Threading.Channels;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // BackgroundService - Servi�o cont�nuo que monitora e envia notifica��es relacionadas a contratos
        services.AddHostedService<NotificacaoContratosWorker>();

        // TimedService - Servi�o baseado em timer para gera��o peri�dica de relat�rios di�rios de empr�stimos
        services.AddHostedService<RelatorioDiarioWorker>();

        // Cria��o de uma fila em mem�ria (Channel) para comunica��o ass�ncrona e desacoplada entre produtores e consumidores
        var channel = Channel.CreateUnbounded<string>();
        services.AddSingleton(channel);

        // QueueBasedBackgroundService - Servi�o consumidor respons�vel por processar contratos provenientes da fila (Channel)
        services.AddHostedService<ContratosProcessorWorker>();

        // Simula��o de produ��o inicial de dados na fila para testes ou inicializa��o
        Task.Run(async () =>
        {
            var writer = channel.Writer;

            await writer.WriteAsync("Contrato-001");
            await writer.WriteAsync("Contrato-002");
            await writer.WriteAsync("Contrato-003");

            writer.Complete();  // Indica que n�o haver� mais itens produzidos
        });

        // Registro da implementa��o do servi�o de valida��o de contratos com tempo de vida Scoped (por requisi��o/escopo)
        services.AddScoped<IValidacaoEmprestimo, ValidacaoEmprestimoService>();

        // ScopedService - Servi�o que consome o servi�o de valida��o Scoped dentro de um BackgroundService, criando escopos manuais para inje��o
        services.AddHostedService<ValidacaoWorkerScopedService>();

        // HostedBase - Servi�o customizado que implementa diretamente IHostedService, para controle total do ciclo de vida
        services.AddHostedService<CustomizadoWorker>();

        // AgendadorQuartz - Configura��o do Quartz Scheduler para agendamento avan�ado de tarefas
        services.AddQuartz(q =>
        {
            // Defini��o de chave �nica para o job de sincroniza��o de status dos contratos
            var jobKey = new JobKey("SincronizacaoStatusContratosJob");

            // Registro do job que executa a l�gica de sincroniza��o com sistemas externos
            q.AddJob<SincronizacaoStatusContratosWorker>(opt => opt.WithIdentity(jobKey));

            // Configura��o do trigger que dispara o job a cada 20 segundos, repetidamente
            q.AddTrigger(opt => opt
                .ForJob(jobKey)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever()));
        });

        // Hospeda o Quartz como um servi�o gerenciado, aguardando o t�rmino dos jobs no shutdown
        services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
    })
    // Configura o host para rodar como servi�o do Windows, se implantado nessa plataforma
    .UseWindowsService();

// Inicializa e executa o Host
await builder.Build().RunAsync();
