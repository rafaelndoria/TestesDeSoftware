namespace NerdStore.Vendas.Domain
{
   public class Pedido
   {
      public Pedido()
      {
         _pedidoItens = new List<PedidoItem>();
      }

      public decimal ValorTotal { get; private set; }

      private readonly List<PedidoItem> _pedidoItens;
      public IReadOnlyCollection<PedidoItem> PedidoItens => _pedidoItens;

      public void AdicionarItem(PedidoItem item)
      {
         _pedidoItens.Add(item);
         ValorTotal = PedidoItens.Sum(i => i.ValorUnitario * i.Quantidade);
      }
   }

   public class PedidoItem
   {
      public PedidoItem(Guid produtoId, string produtoNome, int quantidade, decimal valorUnitario)
      {
         ProdutoId = produtoId;
         ProdutoNome = produtoNome;
         Quantidade = quantidade;
         ValorUnitario = valorUnitario;
      }

      public Guid ProdutoId { get; private set; }
      public string ProdutoNome { get; private set; }
      public int Quantidade { get; private set; }
      public decimal ValorUnitario { get; private set; }
   }
}
