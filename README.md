# SoundSpace

API para catálogo musical, biblioteca e playlists, com busca e reprodução de vídeos do YouTube. O frontend estático é servido pela própria API.

## Tecnologias

- .NET 10 / ASP.NET Core
- PostgreSQL
- Entity Framework Core com Npgsql
- YouTube Data API v3
- Scalar para explorar e testar os endpoints

## Pré-requisitos

- .NET SDK 10
- PostgreSQL em execução
- Uma chave da YouTube Data API v3 para usar a busca e salvar vídeos do YouTube

## Configuração local

Na raiz do repositório, configure a conexão do PostgreSQL usando User Secrets. Substitua os valores de exemplo pelos da sua instalação:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=soundspace_db;Username=postgres;Password=SUA_SENHA" --project .\SoundSpace.Api.csproj
```

Configure a chave do YouTube no mesmo armazenamento local:

```powershell
dotnet user-secrets set "YouTube:ApiKey" "SUA_CHAVE_DA_YOUTUBE_DATA_API" --project .\SoundSpace.Api.csproj
```

User Secrets mantém esses valores fora do código-fonte. Não coloque senhas nem chaves de API em commits.

Crie o banco `soundspace_db` no PostgreSQL, caso ainda não exista. Depois, aplique as migrations:

```powershell
dotnet ef database update --project .\SoundSpace.Infrastructure\SoundSpace.Infrastructure.csproj --startup-project .\SoundSpace.Api.csproj
```

## Executar

Na raiz do repositório, rode:

```powershell
dotnet run --project .\SoundSpace.Api.csproj --launch-profile http
```

A API estará em `http://localhost:5068`. A interface web será servida pela própria API nesse mesmo endereço. A documentação interativa Scalar fica em `http://localhost:5068/scalar`, e o documento OpenAPI em `http://localhost:5068/openapi/v1.json` durante o ambiente de desenvolvimento.

Para compilar sem iniciar o servidor:

```powershell
dotnet build .\SoundSpace.Api.csproj
```

## Configuração de CORS

Quando uma página frontend for servida em uma origem diferente da API, adicione a origem exata à lista `Cors:AllowedOrigins` em `appsettings.Development.json`. O ambiente de desenvolvimento inclui `http://127.0.0.1:5168` e `http://localhost:5168`. As chamadas do frontend servido pela própria API em `http://localhost:5068` são da mesma origem e não precisam de CORS.

A política permite os métodos `GET`, `POST`, `PUT` e `DELETE`, e os cabeçalhos `Accept` e `Content-Type`. Não permite credenciais nem libera origens indiscriminadamente.

## Principais rotas

- `/api/musicas` — operações de consulta e CRUD de músicas
- `/api/artistas` — operações de consulta e CRUD de artistas
- `/api/albuns` — operações de consulta e CRUD de álbuns
- `/api/playlists` — operações de consulta e CRUD de playlists e seus vínculos com músicas
- `/api/biblioteca` — consulta e remoção de músicas salvas
- `/api/music-search` — busca no YouTube e salvamento de vídeos na biblioteca

Use o Scalar para consultar todos os métodos, parâmetros e formatos de entrada. Exemplos adicionais de chamadas estão em [SoundSpace.Api.http](./SoundSpace.Api.http).

## Observações

- A busca no YouTube depende de uma chave válida e da disponibilidade da API externa.
- O projeto ainda não possui autenticação de usuários; as rotas não são isoladas por conta.
- Não há um projeto de testes automatizados configurado atualmente.
