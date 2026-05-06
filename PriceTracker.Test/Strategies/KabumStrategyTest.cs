using PriceTracker.Worker.Strategies;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTracker.Test.Strategies
{
    [TestFixture]
    public class KabumStrategyTest
    {
        private KabumStrategy _kabumStrategy;

        [SetUp]
        public void Setup()
        {
            _kabumStrategy = new KabumStrategy();
        }

        [Test]
        public async Task ExtractPrice_ShouldReturnValidResult()
        {
            
            string url = "https://www.kabum.com.br/produto/426262/processador-amd-ryzen-7-7800x3d-5-0ghz-max-turbo-cache-104mb-am5-8-nucleos-video-integrado-100-100000910wof?gclsrc=aw.ds&&utm_id=22429436063&gad_source=1&gad_campaignid=22429436063&gbraid=0AAAAADx-HyGREsKUM_3puMtKuxO9rWXjD&gclid=CjwKCAjwqubPBhBOEiwAzgZX2laSDYkpQIXY7apeGFp2dcC5lvVd0SCko3FpfszhpyIUvL_fV9cBvhoC0T0QAvD_BwE";
            
            var result = await _kabumStrategy.ExtractPrice(url);
            
            result.ShouldNotBeNull();
            result.ProductDescription.ShouldBe("Processador AMD Ryzen 7 7800X3D, 5.0GHz Max Turbo, Cache 104MB, AM5, 8 Núcleos, Vídeo Integrado - 100-100000910WOF");
            result.Price.ShouldBeGreaterThan(0);
        }
    }
}
