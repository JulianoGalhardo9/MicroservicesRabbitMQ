# 🐇 Microservices with .NET 9 & RabbitMQ

> Projeto de estudo sobre **Event-Driven Architecture (EDA)** utilizando .NET 9 e RabbitMQ — demonstrando comunicação assíncrona entre microsserviços via broker de mensagens.

---

## 📋 Sumário

- [Visão Geral](#-visão-geral)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Pré-requisitos](#-pré-requisitos)
- [Como Executar](#-como-executar)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Conceitos Aprendidos](#-conceitos-aprendidos)
- [Autor](#-autor)

---

## 🎯 Visão Geral

Este projeto simula um fluxo real de pedidos em uma arquitetura de microsserviços desacoplados. O **OrderProducer** publica eventos em uma fila e o **OrderConsumer** os processa de forma assíncrona — sem que os dois serviços precisem se conhecer diretamente.

```
[OrderProducer] ──── publica ────▶ [RabbitMQ: order_queue] ──── consome ────▶ [OrderConsumer]
```

---

## 🚀 Tecnologias

| Tecnologia | Versão | Função |
|---|---|---|
| .NET SDK | 9.0 | Runtime e compilação |
| C# | 13 | Linguagem de programação |
| RabbitMQ | 3-management | Message broker (via Docker) |
| RabbitMQ.Client | 7.0+ | Biblioteca cliente AMQP |
| Docker | - | Containerização do broker |
| Visual Studio Code | - | IDE |

> **Plataforma:** macOS Apple Silicon M3

---

## 🏗️ Arquitetura

O sistema é composto por dois microsserviços independentes que se comunicam exclusivamente através do broker:

```
┌─────────────────────────────────────────────────────────┐
│                     Docker Network                      │
│                                                         │
│  ┌──────────────┐    AMQP     ┌──────────────────────┐  │
│  │ OrderProducer│ ──────────▶ │  RabbitMQ Broker     │  │
│  │              │             │                      │  │
│  │ Cria pedido  │             │  Queue: order_queue  │  │
│  │ Serializa    │             │  Port: 5672 (AMQP)   │  │
│  │ Publica      │             │  Port: 15672 (UI)    │  │
│  └──────────────┘             └──────────────────────┘  │
│                                         │               │
│                               AMQP      ▼               │
│                         ┌───────────────────────────┐   │
│                         │       OrderConsumer        │   │
│                         │                           │   │
│                         │  Escuta a fila            │   │
│                         │  Desserializa             │   │
│                         │  Processa o pedido        │   │
│                         └───────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

### Componentes

**OrderProducer** — Aplicação Console que:
- Cria um objeto de pedido
- Serializa para `ReadOnlyMemory<byte>`
- Publica na fila `order_queue` via protocolo AMQP

**OrderConsumer** — Aplicação Console que:
- Permanece em execução contínua
- Escuta a fila `order_queue`
- Desserializa e processa cada mensagem recebida

---

## 🛠️ Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started)

---

## ▶️ Como Executar

### 1. Subir o RabbitMQ via Docker

```bash
docker run -d \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```

Acesse a interface de gerenciamento em [http://localhost:15672](http://localhost:15672)
> Credenciais padrão: `guest` / `guest`

---

### 2. Clonar o Repositório e Restaurar Dependências

```bash
git clone <url-do-repositorio>
cd MicroservicesRabbitMQ
dotnet restore
```

---

### 3. Iniciar o Consumidor (Terminal 1)

> ⚠️ Inicie o consumer **antes** do producer para garantir que a fila esteja pronta para receber mensagens.

```bash
cd OrderConsumer
dotnet run
```

---

### 4. Iniciar o Produtor (Terminal 2)

```bash
cd OrderProducer
dotnet run
```

---

## 📁 Estrutura do Projeto

```
MicroservicesRabbitMQ/
├── OrderProducer/
│   ├── OrderProducer.csproj
│   └── Program.cs
├── OrderConsumer/
│   ├── OrderConsumer.csproj
│   └── Program.cs
└── README.md
```

---

## 📚 Conceitos Aprendidos

### 🔌 Conexão vs Canal
Implementação de `IConnection` e `IChannel` seguindo o padrão do protocolo AMQP — uma conexão TCP é compartilhada por múltiplos canais lógicos, otimizando recursos de rede.

### ⚡ Programação Assíncrona
Uso intensivo de `async/await` e métodos `*Async` da biblioteca `RabbitMQ.Client v7.0+`, evitando bloqueio de threads — especialmente relevante para aproveitar a performance do chip Apple M3.

### 🔁 Idempotência na Infraestrutura
Declaração de filas com `QueueDeclareAsync` garantindo que a fila exista antes de qualquer operação, independente da ordem de inicialização dos serviços.

### 📦 Serialização AMQP
Conversão de objetos C# para `ReadOnlyMemory<byte>` para transporte via protocolo AMQP — entendendo o contrato binário do broker.

### 🔀 Desacoplamento de Serviços
Producer e Consumer não se conhecem diretamente — a única dependência entre eles é o contrato da mensagem e o nome da fila, princípio central da EDA.

---

## 👨‍💻 Autor

Desenvolvido por **Juliano Galhardo** como parte de estudos sobre sistemas distribuídos e microsserviços.

[![GitHub](https://img.shields.io/badge/GitHub-@JulianoGalhardo-181717?style=flat&logo=github)](https://github.com)

---

<p align="center">
  Feito com ☕ e curiosidade por sistemas distribuídos
</p>
```
