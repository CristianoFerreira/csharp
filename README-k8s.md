# csharp

Gerado pelo template "Aplicação genérica completa" do Backstage. Coloque todos os
arquivos deste pacote na raiz do seu repositório (ou na subpasta informada ao
registrar, se for monorepo) e registre o app na página **Pipelines** do Backstage.

| Arquivo | Para quê |
|---|---|
| `Dockerfile` | O Dockerfile que você informou, sem alteração. |
| `k8s.yaml` | `Deployment` + `Service` no formato lido pelo pipeline. |
| `catalog-info.yaml` | Cadastro do app no catálogo do Backstage (Overview, Kubernetes, TechDocs). |
| `mkdocs.yml` e `docs/` | A documentação do app (TechDocs), já com o que você informou. |

Se o seu repositório já tem algum destes arquivos (principalmente o `Dockerfile`),
compare antes de sobrescrever: o do repositório é a fonte da verdade.

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

O pipeline publica este arquivo no catálogo do Backstage a cada execução
bem-sucedida (repositório público ou privado): o app aparece no catálogo, em poucos
minutos, com a aba Kubernetes já funcionando. O `metadata.name` do Component precisa
ser igual ao nome do app cadastrado em Pipelines. Não deixe nenhuma anotação do
arquivo vazia: o pipeline recusa o arquivo (a execução falha, com o motivo no log) e
o catálogo mantém a versão anterior.

Dois campos ficam SEMPRE no arquivo para você conferir no final:

- `spec.system: unknown`: o agrupamento em Systems ainda não foi definido pelo
  negócio. Troque pelo System do seu app quando ele existir.
- `spec.dependsOn`: os serviços da plataforma que o app usa. Vazio (`[]`) se você
  não escolheu nenhum; complete se o app usar MinIO, Keycloak ou outro.

## Documentação (TechDocs)

`mkdocs.yml` e `docs/` são publicados pelo pipeline a cada execução bem-sucedida e
aparecem na aba **TechDocs** do componente. Edite os `.md` em `docs/` e acrescente
páginas no `nav` do `mkdocs.yml`. Se a geração falhar, a execução aparece como falha
na página Pipelines e a última documentação publicada continua disponível.

## Tracing (opcional)

Sem instrumentação nenhuma, este app já é coberto pelo monitoramento básico
sempre ligado (CPU/memória/saúde do pod). Se quiser tracing distribuído, o
endpoint OTLP compartilhado do cluster é:

```
jaeger-collector.observability.svc.cluster.local:4317
```
