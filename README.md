# Tech Challenge Grupo 4

Este repositório contém a aplicação **ContatosGrupo4**, construída com ASP.NET.

## Executando com Kubernetes

Arquivos de manifesto estão localizados no diretório `k8s/`. Esses arquivos
criam os recursos necessários para executar a API e um banco de dados SQL
Server dentro de um cluster Kubernetes.

1. Crie os objetos de configuração e armazenamento:
   ```bash
   kubectl apply -f k8s/sqlserver-pvc.yaml
   kubectl apply -f k8s/sqlserver-deployment.yaml
   kubectl apply -f k8s/sqlserver-service.yaml
   kubectl apply -f k8s/api-configmap.yaml
   kubectl apply -f k8s/api-deployment.yaml
   kubectl apply -f k8s/api-service.yaml
   ```
2. A API ficará disponível via `NodePort` configurado em `api-service.yaml`.

Os valores de conexão com a base de dados e RabbitMQ são definidos em
`api-configmap.yaml`, facilitando ajustes de configuração sem a necessidade de
novos deployments.
