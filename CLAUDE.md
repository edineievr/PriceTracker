# PriceTracker

Worker .NET 10 de monitoramento de preços com notificação via Telegram. Projeto pessoal e fechado — produtos, plataformas e destino das notificações são escolhas do dono.

## Contexto do projeto

- Escopo 100% pessoal: não há multi-usuário, multi-tenant, nem planos de abertura pública
- Paths hardcoded (banco SQLite, logs) são intencionais por ora — não mover para configuração a menos que explicitamente solicitado
- O banco SQLite é o único storage; sem ORM, acesso direto via `Microsoft.Data.Sqlite`

## Regras de negócio que não devem ser alteradas sem pedido explícito

- Notificação só é disparada quando o preço à vista **cai** em relação ao histórico anterior (`oldHistory.SpotPrice > newHistory.SpotPrice`)
- Todo scraping gera um registro em `PriceHistory`, independentemente de mudança de preço — 1 consulta = 1 histórico, sempre
- A comparação usa exclusivamente `SpotPrice`; `CreditCardPrice` não entra na lógica de disparo

## Estratégias de scraping

- Cada plataforma tem sua própria `Strategy` implementando `IPriceScraper`
- A escolha entre **AngleSharp** (HTML estático) e **Playwright** (renderização JS) é tomada pelo dono do projeto com base na estabilidade do scraping para aquela plataforma — não inferir nem trocar a abordagem por conta própria
- Quando uma estratégia começa a falhar, a decisão de mudar a abordagem virá como instrução explícita

## Adicionando nova plataforma

1. Criar a `Strategy` em `PriceTracker.Worker/Strategies/`
2. Registrar no DI em `Program.cs`
3. Adicionar ao `enum Platform`
4. Adicionar ao `switch` em `PriceTrackerFactory`

Todos os quatro passos são obrigatórios — esquecer qualquer um quebra o fluxo silenciosamente.

## Testes

- Os testes de integração dependem de rede real e isso é intencional — o problema exigiu validar o retorno real das plataformas
- Não mockar rede nos testes de integração existentes
