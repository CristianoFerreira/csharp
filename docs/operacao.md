# Operação

## Como o app chega ao cluster

A cada novo commit no repositório, o pipeline do Backstage (página **Pipelines**)
constrói a imagem a partir do `Dockerfile`, publica no registry e faz o deploy do
`k8s.yaml`. O resultado e o log de cada etapa ficam nessa página. Uma execução que
falha não derruba a versão que já está no ar.

## Porta e saúde

- O container escuta na porta **5001**.
- O `k8s.yaml` usa `GET /health` na porta 5001 como `readinessProbe`:
  o pod só recebe tráfego depois que responde com sucesso.
  Se o app mudar de porta ou de endpoint de saúde, o `k8s.yaml` precisa mudar junto.

## Como acessar

O Service é `NodePort` sem número fixo: o pipeline escolhe uma porta livre
(31000-32767) no primeiro deploy e a mantém nos seguintes. O número aparece no log do
deploy (página Pipelines) e na aba Kubernetes do componente.

## Documentação e catálogo

Esta documentação (pasta `docs/`) e o `catalog-info.yaml` moram no repositório e são
publicados pelo pipeline a cada execução bem-sucedida. Se a publicação falhar, a
execução aparece como falha na página Pipelines e a última versão publicada continua
disponível.

## Contato

dev@dev.com.br
