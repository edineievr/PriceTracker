using PriceTracker.Worker.Enums;

namespace PriceTracker.Worker.Models
{
    public class Product
    {
        public int Id { get; protected set; }
        public string Description { get; private set; }
        public Platform Platform { get; private set; }
        public string Url { get; private set; }
        public bool IsActive { get; private set; }//criado apenas para desativar uma estrategia temporariamente (meli), mas será implementado na automação da alimentação do banco

        public static Product Create(int id, string description, Platform platform, string url)
        {
            return new Product
            {
                Id = id,
                Description = description,
                Platform = platform,
                Url = url,
            };
        }
    }
}
