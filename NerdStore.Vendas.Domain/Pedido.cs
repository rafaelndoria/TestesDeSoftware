using FluentValidation.Results;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Vendas.Domain
{
    public class Pedido : Entity, IAggregateRoot
    {
        public static int MAX_UNIDADES_ITEM => 15;
        public static int MIN_UNIDADES_ITEM => 1;

        protected Pedido()
        {
            _pedidoItens = new List<PedidoItem>();
        }

        public Guid ClienteId { get; private set; }
        public decimal ValorTotal { get; private set; }
        public decimal Desconto { get; private set; }
        public EPedidoStatus PedidoStatus { get; private set; }
        public bool VoucherUtilizado { get; private set; }
        public Voucher Voucher { get; private set; }

        private readonly List<PedidoItem> _pedidoItens;
        public IReadOnlyCollection<PedidoItem> PedidoItens => _pedidoItens;

        private void CalcularValorPedido()
        {
            ValorTotal = PedidoItens.Sum(i => i.CalcularValor());
            CalcularValorTotalDesconto();
        }

        public bool PedidoItemJaAdicionado(PedidoItem item)
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

        public ValidationResult AplicarVoucher(Voucher voucher)
        {
            var resultado = voucher.ValidarSeAplicavel();

            if (resultado.IsValid)
            {
                Voucher = voucher;
                VoucherUtilizado = true;

                CalcularValorTotalDesconto();
            }
            
            return resultado;
        }

        private void CalcularValorTotalDesconto()
        {
            if (!VoucherUtilizado) return;

            decimal desconto = 0;
            var valor = ValorTotal;

            if (Voucher.TipoDescontoVoucher == ETipoDescontoVoucher.Valor)
            {
                if (Voucher.ValorDesconto.HasValue)
                {
                    desconto = Voucher.ValorDesconto.Value;
                    valor -= desconto;
                }
            }
            else
            {
                if (Voucher.PercentualDesconto.HasValue)
                {
                    desconto = (ValorTotal * Voucher.PercentualDesconto.Value) / 100;
                    valor -= desconto;
                }
            }

            ValorTotal = valor < 0 ? 0 : valor;
            Desconto = desconto;
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
}
