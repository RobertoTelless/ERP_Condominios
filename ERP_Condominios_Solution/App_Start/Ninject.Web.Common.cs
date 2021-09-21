using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Web.Common;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using ModelServices.Interfaces.Repositories;
using ApplicationServices.Services;
using ModelServices.EntitiesServices;
using DataServices.Repositories;
using Ninject.Web.Common.WebHost;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Presentation.Start.NinjectWebCommons), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Presentation.Start.NinjectWebCommons), "Stop")]

namespace Presentation.Start
{
    public class NinjectWebCommons
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(typeof(IAppServiceBase<>)).To(typeof(AppServiceBase<>));
            kernel.Bind<IUsuarioAppService>().To<UsuarioAppService>();
            kernel.Bind<ILogAppService>().To<LogAppService>();
            kernel.Bind<IPerfilAppService>().To<PerfilAppService>();
            kernel.Bind<IConfiguracaoAppService>().To<ConfiguracaoAppService>();
            kernel.Bind<INoticiaAppService>().To<NoticiaAppService>();
            kernel.Bind<INotificacaoAppService>().To<NotificacaoAppService>();
            kernel.Bind<ITipoPessoaAppService>().To<TipoPessoaAppService>();
            kernel.Bind<ITemplateAppService>().To<TemplateAppService>();
            kernel.Bind<ITarefaAppService>().To<TarefaAppService>();
            kernel.Bind<IAgendaAppService>().To<AgendaAppService>();
            kernel.Bind<IAssinanteAppService>().To<AssinanteAppService>();
            kernel.Bind<IUnidadeAppService>().To<UnidadeAppService>();
            kernel.Bind<IVagaAppService>().To<VagaAppService>();
            kernel.Bind<IVeiculoAppService>().To<VeiculoAppService>();
            kernel.Bind<ICategoriaFornecedorAppService>().To<CategoriaFornecedorAppService>();
            kernel.Bind<IFornecedorAppService>().To<FornecedorAppService>();
            kernel.Bind<IFornecedorCnpjAppService>().To<FornecedorCnpjAppService>();
            kernel.Bind<ITelefoneAppService>().To<TelefoneAppService>();
            kernel.Bind<IAmbienteAppService>().To<AmbienteAppService>();
            kernel.Bind<IAutorizacaoAppService>().To<AutorizacaoAppService>();
            kernel.Bind<IOcorrenciaAppService>().To<OcorrenciaAppService>();
            kernel.Bind<IBancoAppService>().To<BancoAppService>();
            kernel.Bind<ICentroCustoAppService>().To<CentroCustoAppService>();
            kernel.Bind<IContaBancariaAppService>().To<ContaBancariaAppService>();
            kernel.Bind<IGrupoAppService>().To<GrupoAppService>();
            kernel.Bind<ISubgrupoAppService>().To<SubgrupoAppService>();
            kernel.Bind<IEquipamentoAppService>().To<EquipamentoAppService>();
            kernel.Bind<IProdutoAppService>().To<ProdutoAppService>();
            kernel.Bind<IMovimentoEstoqueProdutoAppService>().To<MovimentoEstoqueProdutoAppService>();
            kernel.Bind<IListaConvidadoAppService>().To<ListaConvidadoAppService>();
            kernel.Bind<ICategoriaTelefoneAppService>().To<CategoriaTelefoneAppService>();
            kernel.Bind<IControleVeiculoAppService>().To<ControleVeiculoAppService>();
            kernel.Bind<ICorpoDiretivoAppService>().To<CorpoDiretivoAppService>();
            kernel.Bind<IMudancaAppService>().To<MudancaAppService>();
            kernel.Bind<IEntradaSaidaAppService>().To<EntradaSaidaAppService>();
            kernel.Bind<IEncomendaAppService>().To<EncomendaAppService>();
            kernel.Bind<IReservaAppService>().To<ReservaAppService>();

            kernel.Bind(typeof(IServiceBase<>)).To(typeof(ServiceBase<>));
            kernel.Bind<IUsuarioService>().To<UsuarioService>();
            kernel.Bind<ILogService>().To<LogService>();
            kernel.Bind<IPerfilService>().To<PerfilService>();
            kernel.Bind<IConfiguracaoService>().To<ConfiguracaoService>();
            kernel.Bind<INotificacaoService>().To<NotificacaoService>();
            kernel.Bind<INoticiaService>().To<NoticiaService>();
            kernel.Bind<ITipoPessoaService>().To<TipoPessoaService>();
            kernel.Bind<ITemplateService>().To<TemplateService>();
            kernel.Bind<ITarefaService>().To<TarefaService>();
            kernel.Bind<IAgendaService>().To<AgendaService>();
            kernel.Bind<IAssinanteService>().To<AssinanteService>();
            kernel.Bind<IUnidadeService>().To<UnidadeService>();
            kernel.Bind<IVagaService>().To<VagaService>();
            kernel.Bind<IVeiculoService>().To<VeiculoService>();
            kernel.Bind<ICategoriaFornecedorService>().To<CategoriaFornecedorService>();
            kernel.Bind<IFornecedorService>().To<FornecedorService>();
            kernel.Bind<IFornecedorCnpjService>().To<FornecedorCnpjService>();
            kernel.Bind<ITelefoneService>().To<TelefoneService>();
            kernel.Bind<IAmbienteService>().To<AmbienteService>();
            kernel.Bind<IAutorizacaoService>().To<AutorizacaoService>();
            kernel.Bind<IOcorrenciaService>().To<OcorrenciaService>();
            kernel.Bind<IBancoService>().To<BancoService>();
            kernel.Bind<ICentroCustoService>().To<CentroCustoService>();
            kernel.Bind<IContaBancariaService>().To<ContaBancariaService>();
            kernel.Bind<IGrupoService>().To<GrupoService>();
            kernel.Bind<ISubgrupoService>().To<SubgrupoService>();
            kernel.Bind<IEquipamentoService>().To<EquipamentoService>();
            kernel.Bind<IMovimentoEstoqueProdutoService>().To<MovimentoEstoqueProdutoService>();
            kernel.Bind<IProdutoService>().To<ProdutoService>();
            kernel.Bind<IListaConvidadoService>().To<ListaConvidadoService>();
            kernel.Bind<ICategoriaTelefoneService>().To<CategoriaTelefoneService>();
            kernel.Bind<IControleVeiculoService>().To<ControleVeiculoService>();
            kernel.Bind<ICorpoDiretivoService>().To<CorpoDiretivoService>();
            kernel.Bind<IMudancaService>().To<MudancaService>();
            kernel.Bind<IEntradaSaidaService>().To<EntradaSaidaService>();
            kernel.Bind<IEncomendaService>().To<EncomendaService>();
            kernel.Bind<IReservaService>().To<ReservaService>();

            kernel.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<>));
            kernel.Bind<IConfiguracaoRepository>().To<ConfiguracaoRepository>();
            kernel.Bind<IUsuarioRepository>().To<UsuarioRepository>();
            kernel.Bind<ILogRepository>().To<LogRepository>();
            kernel.Bind<IPerfilRepository>().To<PerfilRepository>();
            kernel.Bind<ITemplateRepository>().To<TemplateRepository>();
            kernel.Bind<ITipoPessoaRepository>().To<TipoPessoaRepository>();
            kernel.Bind<ICategoriaNotificacaoRepository>().To<CategoriaNotificacaoRepository>();
            kernel.Bind<INotificacaoRepository>().To<NotificacaoRepository>();
            kernel.Bind<INoticiaRepository>().To<NoticiaRepository>();
            kernel.Bind<INoticiaComentarioRepository>().To<NoticiaComentarioRepository>();
            kernel.Bind<INotificacaoAnexoRepository>().To<NotificacaoAnexoRepository>();
            kernel.Bind<ITarefaRepository>().To<TarefaRepository>();
            kernel.Bind<ITipoTarefaRepository>().To<TipoTarefaRepository>();
            kernel.Bind<ITarefaAnexoRepository>().To<TarefaAnexoRepository>();
            kernel.Bind<ITarefaNotificacaoRepository>().To<TarefaNotificacaoRepository>();
            kernel.Bind<IUsuarioAnexoRepository>().To<UsuarioAnexoRepository>();
            kernel.Bind<IUFRepository>().To<UFRepository>();
            kernel.Bind<IAgendaRepository>().To<AgendaRepository>();
            kernel.Bind<IAgendaAnexoRepository>().To<AgendaAnexoRepository>();
            kernel.Bind<ICategoriaAgendaRepository>().To<CategoriaAgendaRepository>();
            kernel.Bind<IAssinanteRepository>().To<AssinanteRepository>();
            kernel.Bind<IAssinanteAnexoRepository>().To<AssinanteAnexoRepository>();
            kernel.Bind<ITipoCondominioRepository>().To<TipoCondominioRepository>();
            kernel.Bind<ITipoUnidadeRepository>().To<TipoUnidadeRepository>();
            kernel.Bind<ITipoVagaRepository>().To<TipoVagaRepository>();
            kernel.Bind<IUnidadeRepository>().To<UnidadeRepository>();
            kernel.Bind<IUnidadeAnexoRepository>().To<UnidadeAnexoRepository>();
            kernel.Bind<ITorreRepository>().To<TorreRepository>();
            kernel.Bind<IVagaRepository>().To<VagaRepository>();
            kernel.Bind<IVeiculoRepository>().To<VeiculoRepository>();
            kernel.Bind<ITipoVeiculoRepository>().To<TipoVeiculoRepository>();
            kernel.Bind<IVeiculoAnexoRepository>().To<VeiculoAnexoRepository>();
            kernel.Bind<ICategoriaFornecedorRepository>().To<CategoriaFornecedorRepository>();
            kernel.Bind<IFornecedorRepository>().To<FornecedorRepository>();
            kernel.Bind<IFornecedorAnexoRepository>().To<FornecedorAnexoRepository>();
            kernel.Bind<IFornecedorCnpjRepository>().To<FornecedorCnpjRepository>();
            kernel.Bind<IFornecedorContatoRepository>().To<FornecedorContatoRepository>();
            kernel.Bind<ITelefoneRepository>().To<TelefoneRepository>();
            kernel.Bind<ICategoriaTelefoneRepository>().To<CategoriaTelefoneRepository>();
            kernel.Bind<ITipoAmbienteRepository>().To<TipoAmbienteRepository>();
            kernel.Bind<IAmbienteRepository>().To<AmbienteRepository>();
            kernel.Bind<IAmbienteChaveRepository>().To<AmbienteChaveRepository>();
            kernel.Bind<IAmbienteCustoRepository>().To<AmbienteCustoRepository>();
            kernel.Bind<IAmbienteImagemRepository>().To<AmbienteImagemRepository>();
            kernel.Bind<ITipoDocumentoRepository>().To<TipoDocumentoRepository>();
            kernel.Bind<IGrauParentescoRepository>().To<GrauParentescoRepository>();
            kernel.Bind<IAutorizacaoAnexoRepository>().To<AutorizacaoAnexoRepository>();
            kernel.Bind<IAutorizacaoRepository>().To<AutorizacaoRepository>();
            kernel.Bind<ICategoriaOcorrenciaRepository>().To<CategoriaOcorrenciaRepository>();
            kernel.Bind<IOcorrenciaRepository>().To<OcorrenciaRepository>();
            kernel.Bind<IOcorrenciaAnexoRepository>().To<OcorrenciaAnexoRepository>();
            kernel.Bind<IOcorrenciaComentarioRepository>().To<OcorrenciaComentarioRepository>();
            kernel.Bind<IBancoRepository>().To<BancoRepository>();
            kernel.Bind<ICentroCustoRepository>().To<CentroCustoRepository>();
            kernel.Bind<IContaBancariaRepository>().To<ContaBancariaRepository>();
            kernel.Bind<IContaBancariaContatoRepository>().To<ContaBancariaContatoRepository>();
            kernel.Bind<IContaBancariaLancamentoRepository>().To<ContaBancariaLancamentoRepository>();
            kernel.Bind<IGrupoRepository>().To<GrupoRepository>();
            kernel.Bind<ISubgrupoRepository>().To<SubgrupoRepository>();
            kernel.Bind<IEquipamentoRepository>().To<EquipamentoRepository>();
            kernel.Bind<IEquipamentoAnexoRepository>().To<EquipamentoAnexoRepository>();
            kernel.Bind<IEquipamentoManutencaoRepository>().To<EquipamentoManutencaoRepository>();
            kernel.Bind<IPeriodicidadeRepository>().To<PeriodicidadeRepository>();
            kernel.Bind<ICategoriaEquipamentoRepository>().To<CategoriaEquipamentoRepository>();
            kernel.Bind<ICategoriaProdutoRepository>().To<CategoriaProdutoRepository>();
            kernel.Bind<ISubcategoriaProdutoRepository>().To<SubcategoriaProdutoRepository>();
            kernel.Bind<IUnidadeMaterialRepository>().To<UnidadeMaterialRepository>();
            kernel.Bind<IMovimentoEstoqueProdutoRepository>().To<MovimentoEstoqueProdutoRepository>();
            kernel.Bind<IProdutoAnexoRepository>().To<ProdutoAnexoRepository>();
            kernel.Bind<IProdutoFornecedorRepository>().To<ProdutoFornecedorRepository>();
            kernel.Bind<IProdutoMovimentoEstoqueRepository>().To<ProdutoMovimentoEstoqueRepository>();
            kernel.Bind<IProdutoRepository>().To<ProdutoRepository>();
            kernel.Bind<IListaConvidadoRepository>().To<ListaConvidadoRepository>();
            kernel.Bind<IConvidadoRepository>().To<ConvidadoRepository>();
            kernel.Bind<IReservaRepository>().To<ReservaRepository>();
            kernel.Bind<IListaConvidadoComentarioRepository>().To<ListaConvidadoComentarioRepository>();
            kernel.Bind<ITipoContaRepository>().To<TipoContaRepository>();
            kernel.Bind<IControleVeiculoRepository>().To<ControleVeiculoRepository>();
            kernel.Bind<IControleVeiculoAcompanhamentoRepository>().To<ControleVeiculoAcompanhamentoRepository>();
            kernel.Bind<ICorpoDiretivoRepository>().To<CorpoDiretivoRepository>();
            kernel.Bind<IFuncaoCorpoDiretivoRepository>().To<FuncaoCorpoDiretivoRepository>();
            kernel.Bind<IMudancaRepository>().To<MudancaRepository>();
            kernel.Bind<IMudancaAnexoRepository>().To<MudancaAnexoRepository>();
            kernel.Bind<IMudancaComentarioRepository>().To<MudancaComentarioRepository>();
            kernel.Bind<IEntradaSaidaRepository>().To<EntradaSaidaRepository>();
            kernel.Bind<IEntradaSaidaComentarioRepository>().To<EntradaSaidaComentarioRepository>();
            kernel.Bind<IListaConvidadoAnexoRepository>().To<ListaConvidadoAnexoRepository>();
            kernel.Bind<IEncomendaRepository>().To<EncomendaRepository>();
            kernel.Bind<IEncomendaAnexoRepository>().To<EncomendaAnexoRepository>();
            kernel.Bind<IEncomendaComentarioRepository>().To<EncomendaComentarioRepository>();
            kernel.Bind<IFormaEntregaRepository>().To<FormaEntregaRepository>();
            kernel.Bind<ITipoEncomendaRepository>().To<TipoEncomendaRepository>();
            kernel.Bind<IReservaRepository>().To<ReservaRepository>();
            kernel.Bind<IReservaAnexoRepository>().To<ReservaAnexoRepository>();
            kernel.Bind<IReservaComentarioRepository>().To<ReservaComentarioRepository>();

        }
    }
}