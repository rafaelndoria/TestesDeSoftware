﻿using MediatR;
using NerdStore.Core.DomainObjects;
using NerdStore.Vendas.Application.Events;
using NerdStore.Vendas.Domain;

namespace NerdStore.Vendas.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarItemPedidoCommand, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMediator _mediator;

        public PedidoCommandHandler(IPedidoRepository pedidoRepository, IMediator mediator)
        {
            _pedidoRepository = pedidoRepository;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AdicionarItemPedidoCommand message, CancellationToken cancellationToken)
        {
            if (message.EhValido())
            {
                foreach (var error in message.ValidationResult.Errors)
                {
                    await _mediator.Publish(new DomainNotification(message.MessageType, error.ErrorMessage), cancellationToken);
                }

                return false;
            }

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);
            var pedidoItem = new PedidoItem(message.ProdutoId, message.Nome, message.Quantidade, message.ValorUnitario);

            if (pedido == null)
            {
                pedido = Pedido.PedidoFactory.NovoPedidoRascunho(message.ClienteId);
                pedido.AdicionarItem(pedidoItem);
                _pedidoRepository.Adicionar(pedido);
            }
            else
            {
                var itemExistente = pedido.PedidoItemJaAdicionado(pedidoItem);
                pedido.AdicionarItem(pedidoItem);

                if (itemExistente)
                {
                    _pedidoRepository.AtualizarItem(pedido.PedidoItens.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId));
                }
                else
                {
                    pedido.AdicionarItem(pedidoItem);
                }
                
                _pedidoRepository.Atualizar(pedido);
            }

            pedido.AdicionarEvento(new PedidoItemAdicionadoEvent(pedido.ClienteId,
                                                                  pedido.Id,
                                                                  message.ProdutoId,
                                                                  message.Nome,
                                                                  message.ValorUnitario,
                                                                  message.Quantidade));

            return await _pedidoRepository.UnitOfWork.Commit();
        }
    }
}
