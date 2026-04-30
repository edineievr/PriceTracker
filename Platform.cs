using System.ComponentModel;

namespace PriceTracker
{
    public enum Platform
    {
        [Description("Mercado Livre")]
        Meli = 0,

        [Description("Kabum")]
        Kabum = 1,
        // Adicione outras plataformas conforme necessário
    }
}
