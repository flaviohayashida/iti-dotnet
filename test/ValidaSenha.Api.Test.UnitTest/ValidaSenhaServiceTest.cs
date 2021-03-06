using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using ValidaSenha.Api.Services;

namespace ValidaSenha.Api.UnitTest
{
    [TestFixture]
    public class ComplexidadeSenhaValidatorTest
    {
        [TestCase("", false)]
        [TestCase("aa", false)]
        [TestCase("ab", false)]
        [TestCase("AAAbbbCc", false)]
        [TestCase("AbTp9!foo", false)]
        [TestCase("AbTp9!foA", false)]
        [TestCase("AbTp9 fok", false)]
        [TestCase("AbTp9!fok", true)] // true
        public void Deve_Validar_Senhas_Corretamente_Para_ConfigTotalCaracteres(
            string password,
            bool expectedIsValid)
        {
            // Arrange
            var service = new ValidaSenhaService();

            //Act
            var result = service.Valida(password);

            //Assert
            Assert.AreEqual(expectedIsValid, result);

        }
    }
}