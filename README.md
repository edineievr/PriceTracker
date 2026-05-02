**PriceTracker**
Monitoramento automatizado de preços de produtos específicos com notificação via Telegram.

**O problema**

Queria acompanhar preços de peças para montar um PC novo, mas os scrapers existentes são genéricos demais. Precisava de algo fechado, focado exatamente nos produtos que me interessam.

**Como funciona**

Produtos são cadastrados com link e plataforma definidos
Um job consulta os produtos cadastrados periodicamente
A plataforma do produto define, via Factory, qual estratégia de scraping será usada
Após a consulta, o histórico é gravado e comparado com o registro anterior
Se o preço mudou, uma notificação é enviada via bot do Telegram com descrição, preço atual e plataforma

**Decisões técnicas**

Factory Pattern para estratégias de scraping por plataforma — fácil de estender sem modificar o orquestrador
Separação entre persistência de histórico, comparação de preço e notificação
Notification Service desacoplado — troca de canal sem impacto no fluxo principal

**Status atual**

✅ Banco de dados e histórico de preços

✅ Notification Service via Telegram

✅ Estratégia Mercado Livre implementada

🔄 Estratégia Kabum em desenvolvimento

🔄 Alimentação automática do banco em avaliação
