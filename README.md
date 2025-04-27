# API de E-commerce - Teste Técnico Rota das Oficinas

Olá! Me chamo Samuel Ribeiro e quero apresentar minha solução para o teste técnico da Rota das Oficinas: uma API de e-commerce em .NET 8 que gerencia clientes, produtos e vendas com autenticação JWT, paginação avançada e análise de dados. Implementei testes unitários abrangentes e segui as melhores práticas de arquitetura, garantindo código limpo e funcional para todos os requisitos solicitados.

## Executando a API

### 🔐 Gerando Certificados para JWT
O projeto utiliza certificados para assinar e validar tokens JWT. Siga estes passos para gerar os certificados necessários:
> **Nota:** O OpenSSL é necessário para gerar certificados e lidar com aspectos de segurança da API.  
> Instalação:
> - **Windows**: `choco install openssl`
> - **Linux (Ubuntu)**: `sudo apt update && sudo apt install openssl`

1. Navegue para a pasta certificates na raiz do projeto:
```bash
cd Certificates
```

2. Gere uma chave privada RSA:
```bash
openssl genrsa -out private_key.pem 2048
```

3. Extraia a chave pública:
```bash
openssl rsa -in private_key.pem -pubout -out public_key.pem
```

4. Crie um certificado X509 autoassinado:
```bash
openssl req -new -x509 -key private_key.pem -out certificate.pem -days 365
```

5. Converta para o formato PFX que o .NET usa:
```bash
openssl pkcs12 -export -out jwt-key.pfx -inkey private_key.pem -in certificate.pem
```
> **Nota Importante:** Você precisará definir uma senha durante a criação do arquivo PFX. Esta senha deve ser configurada em dois lugares:
>
> 1. No arquivo `appsettings.json` na chave `Jwt:KeyPassword`
> 2. No arquivo `docker-compose.yml` na variável de ambiente `Jwt__KeyPassword`
>
> Certifique-se de usar a mesma senha em ambos os lugares para que a autenticação funcione corretamente.

### 💾 Configuração da View para Análise de Dados

> **Importante:** Os endpoints de análises depende de uma view específica no banco de dados. Após configurar e 
> executar a 
> API pelo 
> método de sua preferência (Docker ou local), você precisará executar o script SQL localizado na pasta `./Scripts/Views` na raiz do projeto. Este script criará a view necessária no PostgreSQL para garantir o funcionamento correto das análises de vendas.
### 🐋 Executando com Docker Compose

**Docker:** o projeto está configurado para ser facilmente executado com Docker Compose.

1. Certifique-se de ter o Docker e o Docker Compose instalados em seu sistema

2. Commando para rodar a API com o Docker Compose
```bash
"Windows:" docker-compose up -d
"Linux Ubuntu:" docker compose up -d
```

### ⚙️ Executando Localmente (Sem Docker Compose)

**Local:** Aqui está o passo a passo para rodar API localmente

1. Ter a imagem do postgres na sua máquina:
```bash
docker pull postgres
```

2. Rodar o seguinte comando para criar o container compatível com a ConnectionStrings:
```bash
docker run --name postgres -e POSTGRES_PASSWORD=postgres123 -e POSTGRES_USER=postgres -p 5432:5432 -d postgres
```
3. Entrar na camada da WebApi (Você deve está na raiz do projeto):
```bash 
cd src/RO.DevTest.Web
```

4. Rodar os seguintes comandos:
```bash
dotnet clean
dotnet restore
dotnet build
```

5. Voltar uma pasta e Entrar na camada Persistence:
```bash
cd ..
cd RO.DevTest.Persistence
```
6. Rodar o comando abaixo:
```bash
dotnet ef database update
```
7. Voltar uma pasta e Acessar a camada WebApi:
```bash
cd ..
cd RO.DevTest.WebApi
```
8. Rodar a api com seguinte comando:
```bash
dotnet run
```

### 🌐 Acessando a API

A API estará disponível nas seguintes URLs após a execução:

> **Swagger UI:** Acesse a documentação interativa e teste os endpoints diretamente pelo navegador:
> ```
> http://localhost:5087/swagger/index.html
> ```
>
> **Endpoints Diretos:** Para chamadas via Postman, Insomnia ou outras ferramentas:
> ```
> http://localhost:5087
> ```
>
> **Nota:** Para utilizar os endpoints protegidos, você precisará autenticar-se primeiro através do endpoint `/auth/login` e usar o token JWT retornado nos cabeçalhos de suas requisições.

### 📚 Documentação de Endpoints

#### Autenticação

A API utiliza autenticação JWT para proteger os endpoints. Abaixo estão os endpoints de autenticação disponíveis:

1. **Validar Token** - `GET /v1/auth`
    - Este endpoint requer autenticação e serve para validar se o token atual é válido
    - Retorno: 200 OK se autenticado, 401 Unauthorized caso contrário

2. **Login** - `POST /v1/auth/login`
    - Endpoint para autenticar usuários e obter um token JWT
    - Corpo da requisição: Credenciais do usuário (email/username e senha)
    - Retorno: Token de acesso, refresh token e informações do usuário

3. **Refresh Token** - `POST /v1/auth/refresh-token`
    - Permite renovar o token JWT usando um refresh token
    - Corpo da requisição: Id do User e Refresh token atual
    - Retorno: Novo token de acesso e refresh token

#### Monitoramento e Diagnóstico

A API inclui um endpoint de verificação de saúde para monitoramento e diagnóstico:

1. **Verificar Disponibilidade** - `GET /`
   - Endpoint para verificar se o sistema está operacional
   - Não requer autenticação
   - Retorno: Mensagem confirmando que o sistema está funcionando normalmente
   - Útil para monitoramento automático, health checks e verificações de disponibilidade

#### Gerenciamento de Usuários

A API oferece endpoints para gerenciamento de usuários com diferentes papéis:

1. **Criar Usuário Administrador** - `POST /v1/users/admin`
   - Cria um novo usuário com privilégios de administrador
   - Não requer autenticação
   - Corpo da requisição: Informações do usuário (nome, email, senha)
   - Retorno: Dados do usuário criado com status 201 Created

2. **Criar Usuário Cliente** - `POST /v1/users/customer`
   - Cria um novo usuário com papel de cliente
   - Não requer autenticação
   - Corpo da requisição: Informações do usuário (nome, email, senha)
   - Retorno: Dados do usuário criado com status 201 Created

3. **Atualizar Usuário** - `PUT /v1/users`
   - Atualiza informações do usuário autenticado
   - Requer autenticação (o ID do usuário é obtido do token)
   - Corpo da requisição: Informações atualizadas do usuário
   - Retorno: Dados atualizados do usuário

4. **Obter Usuário Autenticado** - `GET /v1/users`
   - Retorna informações do usuário autenticado
   - Requer autenticação (o ID do usuário é obtido do token)
   - Retorno: Dados do usuário

5. **Buscar Usuário por Nome ou Email** - `GET /v1/users/{nameOrEmail}`
   - Busca um usuário pelo nome ou email
   - Requer autenticação
   - Retorno: Dados do usuário encontrado

#### Gerenciamento de Produtos

A API oferece um conjunto completo de endpoints para gerenciamento de produtos:

1. **Criar Produto** - `POST /v1/products`
   - Cria um novo produto no sistema
   - Requer autenticação
   - Corpo da requisição: Dados do produto (nome, preço, descrição, etc.)
   - Retorno: Informações do produto criado com status 201 Created

2. **Atualizar Produto** - `PUT /v1/products`
   - Atualiza informações de um produto existente
   - Requer autenticação
   - Corpo da requisição: ID do produto e dados atualizados
   - Retorno: Informações atualizadas do produto

3. **Buscar Produto** - `GET /v1/products/{productId}`
   - Retorna informações detalhadas de um produto específico
   - Requer autenticação
   - Parâmetro de rota: ID do produto (formato GUID)
   - Retorno: Dados completos do produto solicitado

4. **Listar Todos os Produtos** - `GET /v1/products`
   - Lista todos os produtos disponíveis com suporte à paginação
   - Requer autenticação
   - Parâmetros de consulta opcionais: página, tamanho da página
   - Retorno: Lista paginada de produtos

5. **Excluir Produto** - `DELETE /v1/products`
   - Remove um produto do sistema
   - Requer autenticação
   - Parâmetro de consulta: ID do produto a ser excluído
   - Retorno: Confirmação da exclusão do produto

#### Gerenciamento de Vendas

A API oferece um conjunto completo de endpoints para gerenciamento e análise de vendas:

1. **Criar Venda** - `POST /v1/sales`
   - Registra uma nova venda no sistema
   - Requer autenticação
   - Corpo da requisição: Dados da venda (produtos, quantidades, cliente)
   - Retorno: Detalhes da venda criada com status 201 Created

2. **Atualizar Venda** - `PUT /v1/sales`
   - Atualiza informações de uma venda existente
   - Requer autenticação
   - Corpo da requisição: ID da venda e dados atualizados
   - Retorno: Informações atualizadas da venda

3. **Excluir Venda** - `DELETE /v1/sales`
   - Remove uma venda do sistema
   - Requer autenticação
   - Corpo da requisição: ID da venda a ser excluída
   - Retorno: Confirmação da exclusão da venda

4. **Minhas Compras** - `GET /v1/sales/customer`
   - Lista todas as compras do cliente autenticado
   - Requer autenticação (cliente)
   - Suporta paginação e filtros
   - Retorno: Lista de compras do cliente

5. **Detalhes da Venda** - `GET /v1/sales/{saleId}`
   - Retorna informações detalhadas de uma venda específica
   - Requer autenticação
   - Parâmetro de rota: ID da venda (formato GUID)
   - Retorno: Dados completos da venda solicitada

6. **Vendas por Período (Admin)** - `GET /v1/sales/admin`
   - Lista vendas realizadas em um período específico
   - Requer autenticação (administrador)
   - Parâmetros de consulta: data inicial, data final
   - Retorno: Lista de vendas no período especificado

7. **Análise de Vendas (Admin)**
   - **Visão Geral**: `GET /v1/sales/admin/analysis`
      - Fornece análise detalhada de vendas agregadas por dia e por produto
      - Requer autenticação (administrador)
      - Parâmetros de consulta: período para análise (startDate, endDate), paginação (pageNumber, pageSize)
      - Retorno: Lista de vendas diárias contendo quantidade total, receita total e detalhes de transações

   - **Por Produto**: `GET /v1/sales/admin/analysis/products`
      - Fornece análise detalhada dos produtos vendidos
      - Requer autenticação (administrador)
      - Parâmetros de consulta: período para análise (startDate, endDate)
      - Retorno: Dados analíticos sobre o produto, incluindo nome, total de itens vendidos, valor total e quantidade de transações

   - **Faturamento Total**: `GET /v1/sales/admin/revenue/total`
      - Fornece o faturamento consolidado de todos os produtos
      - Requer autenticação (administrador)
      - Parâmetros de consulta: período para análise (startDate, endDate)
      - Retorno: Dados agregados incluindo valor total, quantidade de itens, número de transações e lista dos 5 produtos mais vendidos no período

   - **Nota**: Todos os endpoints necessitam da view SQL `vw_admin_product_monthly_sales` para funcionamento correto
