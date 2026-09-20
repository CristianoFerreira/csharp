# csharp

Gerado pelo template "Aplicação genérica (a partir do Dockerfile)" do Backstage.
Coloque `k8s.yaml` e `catalog-info.yaml` no seu repositório, ao lado do
`Dockerfile` que você já tem (raiz, ou numa subpasta se for monorepo), e
registre o app na página **Pipelines** do Backstage.

## Contrato exigido pelo pipeline

Na raiz do repositório (ou na subpasta informada ao registrar):

- `Dockerfile` - builda a imagem. Sem `ARG` de commit: o pipeline sempre
  builda o HEAD detectado no momento da execução. Testes ficam por conta do
  próprio Dockerfile (ex.: um estágio que roda os testes antes do estágio
  final). Ele também passa pelo `hadolint` (erro falha o pipeline).
- `k8s.yaml` - `Deployment` + `Service` neste formato. O campo `image:` é
  sobrescrito pelo pipeline antes do apply; o valor inicial não importa.

Sem isso, o pipeline falha no step `clone-validate` com uma mensagem apontando
exatamente o que falta - ele nunca tenta adivinhar ou criar nada.

## Porta e health check

A porta deste `k8s.yaml` (`containerPort`, `readinessProbe` e `targetPort`) é
`5001` e precisa ser a porta em que a sua aplicação realmente
escuta. Ela também precisa responder `GET /health` (usado pelo
`readinessProbe`): sem isso o pod nunca fica `Ready` e o `Service` não recebe
tráfego. Se você mudar a porta da aplicação, altere os três lugares.


## Acesso externo (NodePort automática)

O `Service` deste app é `type: NodePort` sem número: o pipeline escolhe uma
porta livre (31000-32767) no deploy e a mantém nos deploys seguintes. O número
aparece no log do deploy (página Pipelines, botão Log) e na aba Kubernetes.

## catalog-info.yaml

Registre-o no Backstage (Catalog -> Register Existing Component, com a URL
deste arquivo no seu repositório) para o app aparecer no catálogo com a aba
Kubernetes já funcionando.

## Tracing (opcional)

Sem instrumentação nenhuma, este app já é coberto pelo monitoramento básico
sempre ligado (CPU/memória/saúde do pod). Se quiser tracing distribuído, o
endpoint OTLP compartilhado do cluster é:

```
jaeger-collector.observability.svc.cluster.local:4317
```
