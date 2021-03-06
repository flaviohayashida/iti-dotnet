using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using FluentAssertions;

namespace ValidaSenha.Api.UnitTest
{
    [TestFixture]
    public class AddSwaggerToAppExtensionTest
    {
        [Test]
        public void AddSwaggerToApp_Should_Register_AtLeatOne_ServiceProvider()
        {
            //Arrange
            var configuration = new Mock<IConfiguration>().Object;
            var services = new ServiceCollection();

            //Act
            int countBeforeRegistration = services.Count;
            services.AddToAppSwagger(configuration);
            int countAfterRegistration = services.Count;
            var sp = services.BuildServiceProvider();
            
            //Assert
            countAfterRegistration.Should().BeGreaterThan(countBeforeRegistration);
            
        }
    }
}

