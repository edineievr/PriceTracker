using System.ComponentModel;

namespace PriceTracker.Worker.Enums
{
    public enum Platform
    {
        [Description("Mercado Livre")]
        Meli = 0,

        [Description("Kabum")]
        Kabum = 1,

        [Description("Pichau")]
        Pichau = 2,
        Terabyte = 3,
        // Adicione outras plataformas conforme necessário
    }
}
