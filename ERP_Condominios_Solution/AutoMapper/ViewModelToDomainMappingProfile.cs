using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using SystemBRPresentation.ViewModels;

namespace MvcMapping.Mappers
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UsuarioViewModel, USUARIO>();
            CreateMap<UsuarioLoginViewModel, USUARIO>();
            CreateMap<LogViewModel, LOG>();
            CreateMap<ConfiguracaoViewModel, CONFIGURACAO>();
            CreateMap<BancoViewModel, BANCO>();
            CreateMap<ContaBancariaViewModel, CONTA_BANCO>();
            CreateMap<MatrizViewModel, MATRIZ>();
            CreateMap<FilialViewModel, FILIAL>();
            CreateMap<ClienteViewModel, CLIENTE>();
            CreateMap<FornecedorViewModel, FORNECEDOR>();
            CreateMap<ProdutoViewModel, PRODUTO>();
            CreateMap<MateriaPrimaViewModel, MATERIA_PRIMA>();
            CreateMap<ServicoViewModel, SERVICO>();
            CreateMap<TransportadoraViewModel, TRANSPORTADORA>();
            CreateMap<EquipamentoViewModel, EQUIPAMENTO>();
            CreateMap<PatrimonioViewModel, PATRIMONIO>();
            CreateMap<CargoViewModel, CARGO>();
            CreateMap<ValorComissaoViewModel, VALOR_COMISSAO>();
            CreateMap<ClienteContatoViewModel, CLIENTE_CONTATO>();
            CreateMap<ClienteReferenciaViewModel, CLIENTE_REFERENCIA>();
            CreateMap<ClienteTagViewModel, CLIENTE_TAG>();
            CreateMap<ProdutoFornecedorViewModel, PRODUTO_FORNECEDOR>();
            CreateMap<ProdutoGradeViewModel, PRODUTO_GRADE>();
            CreateMap<ContratoViewModel, CONTRATO>();
            CreateMap<ContratoSolicitacaoAprovacaoViewModel, CONTRATO_SOLICITACAO_APROVACAO>();
            CreateMap<FornecedorContatoViewModel, FORNECEDOR_CONTATO>();
            CreateMap<NoticiaViewModel, NOTICIA>();
            CreateMap<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>();
            CreateMap<NotificacaoViewModel, NOTIFICACAO>();
            CreateMap<FichaTecnicaViewModel, FICHA_TECNICA>();
            CreateMap<FichaTecnicaDetalheViewModel, FICHA_TECNICA_DETALHE>();
            CreateMap<ContaReceberViewModel, CONTA_RECEBER>();
            CreateMap<ContratoComentarioViewModel, CONTRATO_COMENTARIO>();
            CreateMap<CategoriaMateriaViewModel, CATEGORIA_MATERIA>();
            CreateMap<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>();
            CreateMap<SubCategoriaProdutoViewModel, SUBCATEGORIA_PRODUTO>();
            CreateMap<CategoriaClienteViewModel, CATEGORIA_CLIENTE>();
            CreateMap<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>();
            CreateMap<CategoriaContratoViewModel, CATEGORIA_CONTRATO>();
            CreateMap<FormaPagamentoViewModel, FORMA_PAGAMENTO>();
            CreateMap<PeriodicidadeViewModel, PERIODICIDADE>();
            CreateMap<SexoViewModel, SEXO>();
            CreateMap<TipoContratoViewModel, TIPO_CONTRATO>();
            CreateMap<TamanhoViewModel, TAMANHO>();
            CreateMap<TipoPessoaViewModel, TIPO_PESSOA>();
            CreateMap<CategoriaEquipamentoViewModel, CATEGORIA_EQUIPAMENTO>();
            CreateMap<RegimeTributarioViewModel, REGIME_TRIBUTARIO>();
            CreateMap<UnidadeViewModel, UNIDADE>();
            CreateMap<TemplateViewModel, TEMPLATE>();
            CreateMap<ContaBancariaLancamentoViewModel, CONTA_BANCO_LANCAMENTO>();
            CreateMap<ContaPagarViewModel, CONTA_PAGAR>();
            CreateMap<ContaReceberParcelaViewModel, CONTA_RECEBER_PARCELA>();
            CreateMap<ContaPagarParcelaViewModel, CONTA_PAGAR_PARCELA>();
            CreateMap<TarefaViewModel, TAREFA>();
            CreateMap<PedidoCompraViewModel, PEDIDO_COMPRA>();
            CreateMap<ItemPedidoCompraViewModel, ITEM_PEDIDO_COMPRA>();
            CreateMap<PedidoVendaViewModel, PEDIDO_VENDA>();
            CreateMap<ItemPedidoVendaViewModel, ITEM_PEDIDO_VENDA>();
            CreateMap<FuncionarioViewModel, FUNCIONARIO>();
            CreateMap<CategoriaAgendaViewModel, CATEGORIA_AGENDA>();
            CreateMap<AgendaViewModel, AGENDA>();
            CreateMap<CategoriaAtendimentoViewModel, CATEGORIA_ATENDIMENTO>();
            CreateMap<AtendimentoViewModel, ATENDIMENTO>();
            CreateMap<AtendimentoAcompanhamentoViewModel, ATENDIMENTO_ACOMPANHAMENTO>();
            CreateMap<OrdemServicoViewModel, ORDEM_SERVICO>();
            CreateMap<DepartamentoViewModel, DEPARTAMENTO>();
            CreateMap<CentroCustoViewModel, CENTRO_CUSTO>();
            CreateMap<SubCategoriaMateriaViewModel, SUBCATEGORIA_MATERIA>();
            CreateMap<TarefaAcompanhamentoViewModel, TAREFA_ACOMPANHAMENTO>();

        }
    }
}