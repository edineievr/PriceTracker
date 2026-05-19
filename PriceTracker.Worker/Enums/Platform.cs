using System.ComponentModel;

namespace PriceTracker.Worker.Enums
{
    public enum Platform
    {
        [Description("Kabum")]
        Kabum = 1,

        [Description("Pichau")]
        Pichau = 2,

        [Description("Terabyte")]
        Terabyte = 3,
        // Adicione outras plataformas conforme necessário
    }
}
