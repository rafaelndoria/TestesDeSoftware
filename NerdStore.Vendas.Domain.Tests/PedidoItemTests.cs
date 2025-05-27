using NerdStore.Core.DomainObjects;

namespace NerdStore.Vendas.Domain.Tests
{
    public class PedidoItemTests
    {
        [Fact(DisplayName = "Criar Pedido Item Quantidade Acima Do Permitido")]
        [Trait("Categoria", "Vendas - Pedido Item")]
        public void CriarPedidoItem_QuantidadeAcimaDoPermitido_DeveRetornarException()
        {
            // Arrange
            var produtoId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<DomainException>(() => new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM + 1, 100));
        }

        [Fact(DisplayName = "Criar Pedido Item Quantidade Abaixo Do Permitido")]
        [Trait("Categoria", "Vendas - Pedido Item")]
        public void CriarPedidoItem_QuantidadeAbaixoDoPermitido_DeveRetornarException()
        {
            // Arrange
            var produtoId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<DomainException>(() => new PedidoItem(produtoId, "Produto Teste", Pedido.MIN_UNIDADES_ITEM - 1, 100));
        }
    }
}
