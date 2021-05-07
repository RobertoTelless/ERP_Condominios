using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using SystemBRPresentation.ViewModels;

namespace MvcMapping.Mappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<USUARIO, UsuarioViewModel>();
            CreateMap<USUARIO, UsuarioLoginViewModel>();
            CreateMap<LOG, LogViewModel>();
            CreateMap<CONFIGURACAO, ConfiguracaoViewModel>();
            CreateMap<BANCO, BancoViewModel>();
            CreateMap<CONTA_BANCO, ContaBancariaViewModel>();
            CreateMap<MATRIZ, MatrizViewModel>();
            CreateMap<FILIAL, FilialViewModel>();
            CreateMap<CLIENTE, ClienteViewModel>();
            CreateMap<FORNECEDOR, FornecedorViewModel>();
            CreateMap<PRODUTO, ProdutoViewModel>();
            CreateMap<MATERIA_PRIMA, MateriaPrimaViewModel>();
            CreateMap<SERVICO, ServicoViewModel>();
            CreateMap<TRANSPORTADORA, TransportadoraViewModel>();
            CreateMap<EQUIPAMENTO, EquipamentoViewModel>();
            CreateMap<PATRIMONIO, PatrimonioViewModel>();
            CreateMap<CARGO, CargoViewModel>();
            CreateMap<VALOR_COMISSAO, ValorComissaoViewModel>();
            CreateMap<CLIENTE_CONTATO, ClienteContatoViewModel>();
            CreateMap<CLIENTE_REFERENCIA, ClienteReferenciaViewModel>();
            CreateMap<CLIENTE_TAG, ClienteTagViewModel>();
            CreateMap<PRODUTO_FORNECEDOR, ProdutoFornecedorViewModel>();
            CreateMap<PRODUTO_GRADE, ProdutoGradeViewModel>();
            CreateMap<CONTRATO, ContratoViewModel>();
            CreateMap<CONTRATO_SOLICITACAO_APROVACAO, ContratoSolicitacaoAprovacaoViewModel>();
            CreateMap<FORNECEDOR_CONTATO, FornecedorContatoViewModel>();
            CreateMap<NOTICIA, NoticiaViewModel>();
            CreateMap<NOTICIA_COMENTARIO, NoticiaComentarioViewModel>();
            CreateMap<NOTIFICACAO, NotificacaoViewModel>();
            CreateMap<FICHA_TECNICA, FichaTecnicaViewModel>();
            CreateMap<FICHA_TECNICA_DETALHE, FichaTecnicaDetalheViewModel>();
            CreateMap<CONTA_RECEBER, ContaReceberViewModel>();
            CreateMap<CONTRATO_COMENTARIO, ContratoComentarioViewModel>();
            CreateMap<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>();
            CreateMap<CATEGORIA_MATERIA, CategoriaMateriaViewModel>();
            CreateMap<SUBCATEGORIA_PRODUTO, SubCategoriaProdutoViewModel>();
            CreateMap<CATEGORIA_CLIENTE, CategoriaClienteViewModel>();
            CreateMap<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>();
            CreateMap<CATEGORIA_CONTRATO, CategoriaContratoViewModel>();
            CreateMap<FORMA_PAGAMENTO, FormaPagamentoViewModel>();
            CreateMap<PERIODICIDADE, PeriodicidadeViewModel>();
            CreateMap<SEXO, SexoViewModel>();
            CreateMap<TIPO_CONTRATO, TipoContratoViewModel>();
            CreateMap<TAMANHO, TamanhoViewModel>();
            CreateMap<TIPO_PESSOA, TipoPessoaViewModel>();
            CreateMap<CATEGORIA_EQUIPAMENTO, CategoriaEquipamentoViewModel>();
            CreateMap<REGIME_TRIBUTARIO, RegimeTributarioViewModel>();
            CreateMap<UNIDADE, UnidadeViewModel>();
            CreateMap<TEMPLATE, TemplateViewModel>();
            CreateMap<CONTA_BANCO_LANCAMENTO, ContaBancariaLancamentoViewModel>();
            CreateMap<CONTA_PAGAR, ContaPagarViewModel>();
            CreateMap<CONTA_RECEBER_PARCELA, ContaReceberParcelaViewModel>();
            CreateMap<CONTA_PAGAR_PARCELA, ContaPagarParcelaViewModel>();
            CreateMap<TAREFA, TarefaViewModel>();
            CreateMap<PEDIDO_COMPRA, PedidoCompraViewModel>();
            CreateMap<ITEM_PEDIDO_COMPRA, ItemPedidoCompraViewModel>();
            CreateMap<PEDIDO_VENDA, PedidoVendaViewModel>();
            CreateMap<ITEM_PEDIDO_VENDA, ItemPedidoVendaViewModel>();
            CreateMap<FUNCIONARIO, FuncionarioViewModel>();
            CreateMap<CATEGORIA_AGENDA, CategoriaAgendaViewModel>();
            CreateMap<AGENDA, AgendaViewModel>();
            CreateMap<CATEGORIA_ATENDIMENTO, CategoriaAtendimentoViewModel>();
            CreateMap<ATENDIMENTO, AtendimentoViewModel>();
            CreateMap<ATENDIMENTO_ACOMPANHAMENTO, AtendimentoAcompanhamentoViewModel>();
            CreateMap<ORDEM_SERVICO, OrdemServicoViewModel>();
            CreateMap<DEPARTAMENTO, DepartamentoViewModel>();
            CreateMap<CENTRO_CUSTO, CentroCustoViewModel>();
            CreateMap<SUBCATEGORIA_MATERIA, SubCategoriaMateriaViewModel>();
            CreateMap<TAREFA_ACOMPANHAMENTO, TarefaAcompanhamentoViewModel>();

        }
    }
}
