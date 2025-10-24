EstoqueAPI

API REST + CLI para gerenciar produtos (CRUD)
Stack: .NET 9, ASP.NET Core Web API, EF Core 9, SQLite, Swagger

# Os Pré-requisitos
- .NET 9 SDK instalado (verifique `dotnet --version`)
- VS Code (recomendado) + extensão C# (C# Dev Kit optional)
- (Opcional) VS Code REST Client extension para `requests.http`

# A Instalação
1. Abra o terminal no diretório do projeto (onde está `EstoqueAPI.csproj`).
2. Restaurar pacotes:
   dotnet restore

3. Configurar dotnet-ef (apenas se não tiver):
   dotnet new tool-manifest
   dotnet tool install dotnet-ef

4. Criar migrations e aplicar (EF Core)
   Execute estes comandos na pasta do projeto:

   # criar a migration inicial
   dotnet ef migrations add InitialCreate -o Data/Migrations

   # aplicar migration e criar o arquivo estoque.db
   dotnet ef database update

   Resultado: estoque.db será criado no mesmo diretório do projeto

5. Executar (API + CLI interativo juntos)
   dotnet run

   A saída mostrará:
   API online em http://localhost:5099 (Swagger em /swagger)
   Console e API executando juntos!

   A API ficará acessível em: http://localhost:5099/api/v1/products
   Swagger UI: http://localhost:5099/swagger

# O CLI Interativo
Ao executar `dotnet run`, você terá um menu no mesmo terminal que permite:
- Cadastrar produto
- Listar produtos
- Atualizar produto por Id
- Remover produto por Id

Também é possível testar a API via `requests.http` (VS Code REST Client) ou Postman.

# A Entidade: Product
Campo     | Tipo           | Restrições
--------- | -------------- | ---------------------------
Id        | integer        | PK, auto-increment
Name      | varchar(100)   | Obrigatório, único, 2..100 chars
Category  | varchar(50)    | Opcional
Price     | decimal(10,2)  | Obrigatório, >= 0
SKU       | varchar(50)    | Opcional, único (se preenchido)
CreatedAt | datetime       | Gerado automaticamente (UTC)

# As Rotas (endpoints)
Base: http://localhost:5099

- GET /api/v1/products — lista produtos (filtro: ?name=xxx)
- GET /api/v1/products/{id} — obter por id
- POST /api/v1/products — cria (body JSON)  
  Respostas: 201 Created, 422 Unprocessable Entity (validação), 409 Conflict (nome/SKU duplicado)
- PUT /api/v1/products/{id} — atualiza (body JSON)  
  Respostas: 204 No Content, 400 Bad Request (validação), 404 Not Found, 409 Conflict
- DELETE /api/v1/products/{id} — remove  
  Respostas: 204 No Content, 404 Not Found

# Os Testes
- Use `requests.http` com VS Code REST Client
- Ou use Postman e importe endpoints acima
- Use Swagger para testar via UI rapidamente

## Observações importantes
- SQLite: índices únicos permitem múltiplos NULL. Se quiser SKU sempre único e preenchido, marque [Required] e trate validação.
- Migrations + DB: sempre execute `dotnet ef database update` após adicionar novas migrations.

## Passo-a-passo rápido (VS Code)
1. Abra a pasta `EstoqueAPI` no VS Code
2. Instale extensão C# (se não tiver). (Opcional: REST Client)
3. No terminal integrado:
   dotnet restore
   dotnet new tool-manifest    # se ainda não tiver
   dotnet tool install dotnet-ef
   dotnet ef migrations add InitialCreate -o Data/Migrations
   dotnet ef database update
   dotnet run

Abra http://localhost:5099/swagger para testar pela UI.  
Ou abra `requests.http` e execute as requisições (clique no botão Send Request do REST Client).
