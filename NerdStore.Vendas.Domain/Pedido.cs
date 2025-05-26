using NerdStore.Core.DomainObjects;

namespace NerdStore.Vendas.Domain
{
   public class Pedido
   {
      public static int MAX_UNIDADES_ITEM => 15;
      public static int MIN_UNIDADES_ITEM => 1;

      protected Pedido()
      {
         _pedidoItens = new List<PedidoItem>();
      }

      public Guid ClienteId { get; private set; }
      public decimal ValorTotal { get; private set; }
      public EPedidoStatus PedidoStatus { get; private set; }

      private readonly List<PedidoItem> _pedidoItens;
      public IReadOnlyCollection<PedidoItem> PedidoItens => _pedidoItens;

      private void CalcularValorPedido()
      {
         ValorTotal = PedidoItens.Sum(i => i.CalcularValor());
      }

      private bool PedidoItemJaAdicionado(PedidoItem item)
      {
         return _pedidoItens.Any(p => p.ProdutoId == item.ProdutoId);
      }

      private void ValidarPedidoItemInexistente(PedidoItem item)
      {
         if (!PedidoItemJaAdicionado(item))
            throw new DomainException($"O item não existe no pedido.");
      }

      private void ValidarQuantidadeItemPermitida(PedidoItem item)
      {
         var quantidadeItens = item.Quantidade;
         if (PedidoItemJaAdicionado(item))
         {
            var itemExistente = _pedidoItens.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);
            quantidadeItens += itemExistente.Quantidade;
         }

         if (quantidadeItens > MAX_UNIDADES_ITEM)
            throw new DomainException($"O pedido não pode ter mais de {MAX_UNIDADES_ITEM} unidades do mesmo produto.");
      }

      public void AdicionarItem(PedidoItem item)
      {
         ValidarQuantidadeItemPermitida(item);
         if (PedidoItemJaAdicionado(item))
         {
            var itemExistente = _pedidoItens.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);
            itemExistente.AdicionarUnidade(item.Quantidade);
            item = itemExistente;
            _pedidoItens.Remove(itemExistente);
         }

         _pedidoItens.Add(item);
         CalcularValorPedido();
      }

      public void AtualizarItem(PedidoItem item)
      {
         ValidarPedidoItemInexistente(item);
         ValidarQuantidadeItemPermitida(item);

         var itemExistente = PedidoItens.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);

         _pedidoItens.Remove(itemExistente);
         _pedidoItens.Add(item);

         CalcularValorPedido();
      }

      public void RemoverItem(PedidoItem item)
      {
         ValidarPedidoItemInexistente(item);

         _pedidoItens.Remove(item);

         CalcularValorPedido();
      }

      public void TornarRascunho()
      {
         PedidoStatus = EPedidoStatus.Rascunho;
      }

      public static class PedidoFactory
      {
         public static Pedido NovoPedidoRascunho(Guid clienteId)
         {
            var pedido = new Pedido
            {
               ClienteId = clienteId
            };

            pedido.TornarRascunho();
            return pedido;
         }
      }
   }

   public enum EPedidoStatus
   {
      Rascunho = 0,
      Iniciado = 1,
      Pago = 4,
      Entregue = 5,
      Cancelado = 6
   }

   public class PedidoItem
   {
      public PedidoItem(Guid produtoId, string produtoNome, int quantidade, decimal valorUnitario)
      {
         if (quantidade < Pedido.MIN_UNIDADES_ITEM)
            throw new DomainException($"O pedido não pode ter menos de {Pedido.MIN_UNIDADES_ITEM} unidades do produto.");
         if (quantidade > Pedido.MAX_UNIDADES_ITEM)
            throw new DomainException($"O pedido não pode ter mais de {Pedido.MAX_UNIDADES_ITEM} unidades do mesmo produto.");

         ProdutoId = produtoId;
         ProdutoNome = produtoNome;
         Quantidade = quantidade;
         ValorUnitario = valorUnitario;
      }

      public Guid ProdutoId { get; private set; }
      public string ProdutoNome { get; private set; }
      public int Quantidade { get; private set; }
      public decimal ValorUnitario { get; private set; }

      internal void AdicionarUnidade(int unidades)
      {
         Quantidade += unidades;
      }

      internal decimal CalcularValor()
      {
         return Quantidade * ValorUnitario;
      }
   }
}
