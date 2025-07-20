# Tech Challenge Grupo 6

Sistema de gerenciamento de Contatos desenvolvido em .NET, estruturado em arquitetura de microsserviços e orquestrado com Kubernetes.

## Visão Geral do Projeto

O projeto consiste em:

```bash
- Uma API REST desenvolvida em ASP.NET Core
- Camadas Application, Domain, Infra organizando lógica de negócio e persistência
- Integração com RabbitMQ e SQL Server
- Orquestração via Kubernetes, com suporte a escalabilidade, monitoramento e configuração externa
```

## Tecnologias Utilizadas

```bash
- .NET 8
- ASP.NET Core
- RabbitMQ
- SQL Server
- Kubernetes
- Prometheus + Grafana
- Docker
```

## Estrutura de Pastas

ContatosGrupo4/
├── ContatosGrupo4.Api/         `# API principal`
├── ContatosGrupo4.Application/ `# Casos de uso e serviços`
├── ContatosGrupo4.Domain/      `# Entidades e interfaces`
├── ContatosGrupo4.Infra/       `# Repositórios e configurações infra`
├── ContatosGrupo4.Tests/       `# Testes unitários e de integração`
├── k8s/                        `# Manifests Kubernetes (YAML)`
└── ContatosGrupo4.sln          `# Solução .NET`

## Funcionalidades

```bash
- CRUD de Contatos
- Filtro por nome, e-mail e DDD
- Mensageria com RabbitMQ
- Logs e métricas com Prometheus Exporter
- Escalabilidade com Horizontal Pod Autoscaler (HPA)
```

## Orquestração Kubernetes (/k8s)

`api-deployment.yaml`

Cria e atualiza pods da API com réplicas

`api-service.yaml`

Expõe a API dentro do cluster

`api-configmap.yaml`

Armazena configurações da API externas

`api-hpa.yaml`

Escala a API dinamicamente com base no uso de CPU

`rabbitmq-deployment.yaml`

Inicia o servidor RabbitMQ

`rabbitmq-service.yaml`

Torna o RabbitMQ acessível no cluster

`rabbitmq-configmap.yaml`

Define variáveis de configuração do RabbitMQ

`rabbitmq-exporter.yaml`

Coleta métricas do RabbitMQ para o Prometheus

`sqlserver-deployment.yaml`

Cria um pod SQL Server com armazenamento persistente

`sqlserver-service.yaml`

Expõe o banco de dados internamente

`sqlserver-pvc.yaml`

Volume persistente para banco SQL Server

`prometheus-deployment.yaml`

Servidor de métricas Prometheus

`prometheus-config.yaml`

Configura fontes de métrica para o Prometheus

`grafana-deployment.yaml`

Visualiza métricas Prometheus

`grafana-service.yaml`

Acesso ao dashboard Grafana

## Como Subir o Projeto

```bash
Requisitos:
- kubectl configurado
- Docker instalado
- Cluster Kubernetes local ou remoto (ex: minikube)
```

## Etapas:

1. Construir imagem Docker
`cd ContatosGrupo4.Api`
`docker build -t contatos-grupo4-api .`

2. Aplicar configurações do cluster
`kubectl apply -f k8s/`

3. Acompanhar implantação
`kubectl get pods`
`kubectl get svc`

## Endpoints da API

`GET` /contatos
`GET` /contatos/{id}
`POST` /contatos
`PUT` /contatos/{id}
`DELETE` /contatos/{id}

## Testes

`cd ContatosGrupo4.Tests
`dotnet test`

## Contato

Alunos da Pós-Graduação em Arquitetura de Sistemas .NET