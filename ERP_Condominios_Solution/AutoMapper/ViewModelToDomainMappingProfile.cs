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
        }
    }
}