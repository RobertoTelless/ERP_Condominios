using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<USUARIO, UsuarioViewModel>();
            CreateMap<USUARIO, UsuarioLoginViewModel>();
            CreateMap<LOG, LogViewModel>();
            //CreateMap<CONFIGURACAO, ConfiguracaoViewModel>();
            CreateMap<FORNECEDOR, FornecedorViewModel>();
            //CreateMap<CARGO, CargoViewModel>();
            CreateMap<FORNECEDOR_CONTATO, FornecedorContatoViewModel>();
            CreateMap<NOTICIA, NoticiaViewModel>();
            CreateMap<NOTICIA_COMENTARIO, NoticiaComentarioViewModel>();
            CreateMap<NOTIFICACAO, NotificacaoViewModel>();
            //CreateMap<CONTA_RECEBER, ContaReceberViewModel>();
            //CreateMap<CATEGORIA_PRODUTO, CategoriaProdutoViewModel>();
            //CreateMap<CATEGORIA_FORNECEDOR, CategoriaFornecedorViewModel>();
            //CreateMap<TIPO_PESSOA, TipoPessoaViewModel>();
            CreateMap<UNIDADE, UnidadeViewModel>();
            //CreateMap<TEMPLATE, TemplateViewModel>();
            CreateMap<TAREFA, TarefaViewModel>();
            CreateMap<CATEGORIA_AGENDA, CategoriaAgendaViewModel>();
            CreateMap<AGENDA, AgendaViewModel>();
            CreateMap<TAREFA_ACOMPANHAMENTO, TarefaAcompanhamentoViewModel>();
            CreateMap<VAGA, VagaViewModel>();
            CreateMap<VEICULO, VeiculoViewModel>();
            CreateMap<TELEFONE, TelefoneViewModel>();
            CreateMap<AMBIENTE, AmbienteViewModel>();
            CreateMap<AMBIENTE_CHAVE, AmbienteChaveViewModel>();
            CreateMap<AMBIENTE_CUSTO, AmbienteCustoViewModel>();
            CreateMap<AUTORIZACAO_ACESSO, AutorizacaoViewModel>();
            CreateMap<OCORRENCIA, OcorrenciaViewModel>();
            CreateMap<OCORRENCIA_COMENTARIO, OcorrenciaComentarioViewModel>();
            CreateMap<BANCO, BancoViewModel>();
            CreateMap<CENTRO_CUSTO, CentroCustoViewModel>();
            CreateMap<CONTA_BANCO, ContaBancariaViewModel>();
            CreateMap<CONTA_BANCO_CONTATO, ContaBancariaContatoViewModel>();
            CreateMap<CONTA_BANCO_LANCAMENTO, ContaBancariaLancamentoViewModel>();
            CreateMap<GRUPO, GrupoViewModel>();
            CreateMap<SUBGRUPO, SubgrupoViewModel>();
            CreateMap<EQUIPAMENTO, EquipamentoViewModel>();
            CreateMap<EQUIPAMENTO_MANUTENCAO, EquipamentoManutencaoViewModel>();
            CreateMap<PRODUTO_FORNECEDOR, ProdutoFornecedorViewModel>();
            CreateMap<PRODUTO, ProdutoViewModel>();
            CreateMap<LISTA_CONVIDADO, ListaConvidadoViewModel>();
            CreateMap<CONVIDADO, ConvidadoViewModel>();
            CreateMap<LISTA_CONVIDADO_COMENTARIO, ListaConvidadoComentarioViewModel>();
            CreateMap<CONTROLE_VEICULO, ControleVeiculoViewModel>();
            CreateMap<CONTROLE_VEICULO_ACOMPANHAMENTO, ControleVeiculoAcompanhamentoViewModel>();
            CreateMap<CORPO_DIRETIVO, CorpoDiretivoViewModel>();
            CreateMap<SOLICITACAO_MUDANCA, MudancaViewModel>();
            CreateMap<SOLICITACAO_MUDANCA_COMENTARIO, MudancaComentarioViewModel>();
            CreateMap<ENTRADA_SAIDA, EntradaSaidaViewModel>();
            CreateMap<ENTRADA_SAIDA_COMENTARIO, EntradaSaidaComentarioViewModel>();

        }
    }
}
