using EmprestimosWorkerService;
using EmprestimosWorkerService.Interfaces;
using EmprestimosWorkerService.Services;
using EmprestimosWorkerService.Workers.AgendadorQuartz;
using EmprestimosWorkerService.Workers.BackgroundServiceBase;
using EmprestimosWorkerService.Workers.HostedBase;
using EmprestimosWorkerService.Workers.QueueBasedBackgroundService;
using EmprestimosWorkerService.Workers.ScopedService;
using EmprestimosWorkerService.Workers.TimedServiceBase;
using Quartz;
using Serilog;
using System.Threading.Channels;

#region Configuração do Serilog
// Configuração do Serilog antes da criação do host
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()

    // Log geral
    .WriteTo.Console()
    .WriteTo.File("logs/geral.log", rollingInterval: RollingInterval.Day)

    // Log específico para NotificacaoContratosWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("NotificacaoContratosWorker") == true)
        .WriteTo.File("logs/NotificacaoContratosWorker.log", rollingInterval: RollingInterval.Day))

    // Log específico para RelatorioDiarioWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("RelatorioDiarioWorker") == true)
        .WriteTo.File("logs/RelatorioDiarioWorker.log", rollingInterval: RollingInterval.Day))

    // Log específico para ContratosProcessorWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("ContratosProcessorWorker") == true)
        .WriteTo.File("logs/ContratosProcessorWorker.log", rollingInterval: RollingInterval.Day))

    // Log específico para SincronizacaoStatusContratosWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("SincronizacaoStatusContratosWorker") == true)
        .WriteTo.File("logs/SincronizacaoStatusContratosWorker.log", rollingInterval: RollingInterval.Day))

    // Log específico para CustomizadoWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("WorkerCustomizadoHosted") == true)
        .WriteTo.File("logs/WorkerCustomizadoHosted.log", rollingInterval: RollingInterval.Day))

    // Log específico para ValidacaoWorker
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties["SourceContext"]?.ToString().Contains("ValidacaoWorker") == true)
        .WriteTo.File("logs/ValidacaoWorker.log", rollingInterval: RollingInterval.Day))

    .CreateLogger();

#endregion

var builder = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        #region Worker implementado diremente com IHostedService
        // HostedBase - Serviço customizado que implementa diretamente IHostedService, para controle total do ciclo de vida
        //services.AddHostedService<CustomizadoWorker>();
        #endregion

        #region Worker implementado com BackgroundService (Serviço contínuo)
        // BackgroundService - Serviço contínuo que monitora e envia notificações relacionadas a contratos
        //services.AddHostedService<NotificacaoContratosWorker>();
        #endregion

        #region Worker implementado com TimedServiceBase (Serviço baseado em timer)
        // TimedService - Serviço baseado em timer para geração periódica de relatórios diários de empréstimos
        //services.AddHostedService<RelatorioDiarioWorker>();
        #endregion

        #region Worker implementado com QueueBase (Serviço baseado em fila)
        // QueueBasedBackgroundService - Serviço consumidor responsável por processar contratos provenientes da fila (Channel)
        services.AddHostedService<ContratosProcessorWorker>();
        
        //Serviço para entrada de comandos via Console
        services.AddHostedService<ConsoleCommandWorker>();

        //Criação de uma fila em memória(Channel) para comunicação assíncrona e desacoplada entre produtores e consumidores
        var channel = Channel.CreateUnbounded<string>();
        services.AddSingleton(channel);
        #endregion

        #region Worker implementado com ScopedService (Serviço baseado em escopo)
        //Registro da implementação do serviço de validação de contratos com tempo de vida Scoped(por requisição / escopo)
        //services.AddScoped<IValidacaoEmprestimo, ValidacaoEmprestimoService>();
        //services.AddSingleton<IValidacaoEmprestimoSingleton, ValidacaoEmprestimoSingletonService>();

        // ScopedService - Serviço que consome o serviço de validação Scoped dentro de um BackgroundService, criando escopos manuais para injeção
        //services.AddHostedService<ValidacaoWorker>();
        #endregion

        #region Worker implementado com AgendadorQuartz (Quartz Scheduler)
        // AgendadorQuartz - Configuração do Quartz Scheduler para agendamento avançado de tarefas
        //services.AddQuartz(q =>
        //{
        //    var jobKey = new JobKey("SincronizacaoStatusContratosJob");

        //    // Registro do job que executa a lógica de sincronização com sistemas externos
        //    q.AddJob<SincronizacaoStatusContratosWorker>(opt => opt.WithIdentity(jobKey));

        //    // Configuração do trigger que dispara o job a cada 20 segundos, repetidamente
        //    q.AddTrigger(opt => opt
        //        .ForJob(jobKey)
        //        .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever()));
        //});

        ////Hospeda o Quartz como um serviço gerenciado, aguardando o término dos jobs no shutdown
        //services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
        #endregion

    }).UseWindowsService();

await builder.Build().RunAsync();
