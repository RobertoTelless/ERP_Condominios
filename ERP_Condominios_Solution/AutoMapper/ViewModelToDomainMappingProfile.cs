using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UsuarioViewModel, USUARIO>();
            CreateMap<UsuarioLoginViewModel, USUARIO>();
            CreateMap<LogViewModel, LOG>();
            //CreateMap<ConfiguracaoViewModel, CONFIGURACAO>();
            CreateMap<FornecedorViewModel, FORNECEDOR>();
            //CreateMap<CargoViewModel, CARGO>();
            CreateMap<FornecedorContatoViewModel, FORNECEDOR_CONTATO>();
            CreateMap<NoticiaViewModel, NOTICIA>();
            CreateMap<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>();
            CreateMap<NotificacaoViewModel, NOTIFICACAO>();
            //CreateMap<ContaReceberViewModel, CONTA_RECEBER>();
            //CreateMap<CategoriaProdutoViewModel, CATEGORIA_PRODUTO>();
            //CreateMap<CategoriaFornecedorViewModel, CATEGORIA_FORNECEDOR>();
            //CreateMap<TipoPessoaViewModel, TIPO_PESSOA>();
            CreateMap<UnidadeViewModel, UNIDADE>();
            //CreateMap<TemplateViewModel, TEMPLATE>();
            CreateMap<TarefaViewModel, TAREFA>();
            CreateMap<CategoriaAgendaViewModel, CATEGORIA_AGENDA>();
            CreateMap<AgendaViewModel, AGENDA>();
            CreateMap<TarefaAcompanhamentoViewModel, TAREFA_ACOMPANHAMENTO>();
            CreateMap<VagaViewModel, VAGA>();
            CreateMap<VeiculoViewModel, VEICULO>();
            CreateMap<TelefoneViewModel, TELEFONE>();
            CreateMap<AmbienteViewModel, AMBIENTE>();
            CreateMap<AmbienteChaveViewModel, AMBIENTE_CHAVE>();
            CreateMap<AmbienteCustoViewModel, AMBIENTE_CUSTO>();
            CreateMap<AutorizacaoViewModel, AUTORIZACAO_ACESSO>();
            CreateMap<OcorrenciaViewModel, OCORRENCIA>();
            CreateMap<OcorrenciaComentarioViewModel, OCORRENCIA_COMENTARIO>();
            CreateMap<BancoViewModel, BANCO>();
            CreateMap<CentroCustoViewModel, CENTRO_CUSTO>();
            CreateMap<ContaBancariaContatoViewModel, CONTA_BANCO_CONTATO>();
            CreateMap<ContaBancariaLancamentoViewModel, CONTA_BANCO_LANCAMENTO>();
            CreateMap<CentroCustoViewModel, CENTRO_CUSTO>();
            CreateMap<GrupoViewModel, GRUPO>();
            CreateMap<SubgrupoViewModel, SUBGRUPO>();
            CreateMap<EquipamentoViewModel, EQUIPAMENTO>();
            CreateMap<EquipamentoManutencaoViewModel, EQUIPAMENTO_MANUTENCAO>();
            CreateMap<ProdutoFornecedorViewModel, PRODUTO_FORNECEDOR>();
            CreateMap<ProdutoViewModel, PRODUTO>();
            CreateMap<ListaConvidadoViewModel, LISTA_CONVIDADO>();
            CreateMap<ConvidadoViewModel, CONVIDADO>();
            CreateMap<ListaConvidadoComentarioViewModel, LISTA_CONVIDADO_COMENTARIO>();
            CreateMap<ControleVeiculoViewModel, CONTROLE_VEICULO>();
            CreateMap<ControleVeiculoAcompanhamentoViewModel, CONTROLE_VEICULO_ACOMPANHAMENTO>();
            CreateMap<CorpoDiretivoViewModel, CORPO_DIRETIVO>();
            CreateMap<MudancaViewModel, SOLICITACAO_MUDANCA>();
            CreateMap<MudancaComentarioViewModel, SOLICITACAO_MUDANCA_COMENTARIO>();
            CreateMap<EntradaSaidaComentarioViewModel, ENTRADA_SAIDA_COMENTARIO>();
            CreateMap<EntradaSaidaViewModel, ENTRADA_SAIDA>();

        }
    }
}