````markdown
# 💼 Serviço de Processamento de Empréstimos Consignados

Este projeto é um exemplo completo de **Worker Service em .NET** voltado para o domínio de **empréstimos consignados**. Ele demonstra diferentes abordagens para tarefas em segundo plano, incluindo serviços contínuos, temporizadores, filas assíncronas, injeção de dependência escopada, agendamento com Quartz e serviços customizados.

---

## 🚀 Arquitetura e Serviços Implementados

| Tipo de Serviço                         | Classe / Serviço                              | Descrição |
|----------------------------------------|-----------------------------------------------|-----------|
| **BackgroundService Contínuo**         | `NotificacaoContratosWorkerBackgroundService` | Envia notificações periódicas aos clientes sobre seus contratos. |
| **Timed Service (Timer)**              | `RelatorioDiarioWorkerTimedService`           | Gera relatórios em intervalos fixos. |
| **Fila em Memória (Channel)**          | `ContratosProcessorWorkerQueue`               | Processa contratos enfileirados de forma assíncrona. |
| **Scoped Service para Validação**      | `ValidacaoWorkerScopedService` + `ServicoValidacaoEmprestimo` | Valida contratos com escopo isolado via DI. |
| **Serviço Customizado (IHostedService)**| `WorkerCustomizadoHosted`                     | Exemplo de serviço com ciclo de vida controlado. |
| **Agendamento com Quartz**             | `SincronizacaoStatusContratosWorkerQuartz`    | Job agendado para sincronizar status dos contratos. |

---

## 🛠️ Tecnologias Utilizadas

- [.NET 7+ Worker Service](https://learn.microsoft.com/pt-br/dotnet/core/extensions/workers)
- [Quartz.NET](https://www.quartz-scheduler.net/)
- [System.Threading.Channels](https://learn.microsoft.com/pt-br/dotnet/standard/parallel-programming/channels)
- Injeção de Dependência nativa do .NET
- Logging com `ILogger`

---

## 🧭 Estrutura do Projeto

```plaintext
EmprestimosWorkerService/
├── Interfaces/
│   └── IValidacaoEmprestimo.cs
├── Services/
│   └── ServicoValidacaoEmprestimo.cs
├── Workers/
│   ├── NotificacaoContratosWorkerBackgroundService.cs
│   ├── RelatorioDiarioWorkerTimedService.cs
│   ├── ContratosProcessorWorkerQueue.cs
│   ├── ValidacaoWorkerScopedService.cs
│   ├── WorkerCustomizadoHosted.cs
│   └── SincronizacaoStatusContratosWorkerQuartz.cs
├── Program.cs
└── README.md
````

---

## ▶️ Como Rodar

### 1. Pré-requisitos

* [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
* IDE como Visual Studio 2022 ou VS Code

### 2. Executar o projeto

```bash
dotnet restore
dotnet build
dotnet run
```

### 3. Comportamento Esperado

Ao iniciar, os logs exibem o funcionamento simulado de:

* Notificações periódicas;
* Geração de relatórios a cada 30 segundos;
* Processamento assíncrono de contratos em fila;
* Validação escopada de contratos;
* Job agendado com Quartz para sincronização de status.

---

## 🌱 Melhorias Futuras

* Persistência real de contratos e relatórios via banco de dados.
* Integração com serviços externos de notificação (e-mail, SMS, push).
* Observabilidade com Prometheus e Grafana.
* Configurações dinâmicas (ex: tempos, regras, endpoints).
* Resiliência com Polly (retry, timeout, circuit breaker).

---

## 📄 Licença

Este projeto tem caráter **educacional** e está disponível para uso, modificação e redistribuição conforme desejar.
