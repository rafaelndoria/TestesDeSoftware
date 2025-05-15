namespace Demo.Tests
{
    public class CalculadoraTests
    {
        [Fact]
        public void Calculadora_Somar_RetornarValorSoma()
        {
            // Arrange
            var calculadora = new Calculadora();

            // Act
            var resultado = calculadora.Somar(1, 1);

            // Assert
            Assert.Equal(2, resultado);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2, 1, 3)]
        [InlineData(10, 53, 63)]
        [InlineData(11, 11, 22)]
        [InlineData(4, 4, 8)]
        public void Calculadora_Somar_RetornarValoresCorretos(int n1, int n2, double total)
        {
            // Arrange
            var calculadora = new Calculadora();

            // Act
            var resultado = calculadora.Somar(n1, n2);

            // Assert
            Assert.Equal(total, resultado);
        }
    }
}
