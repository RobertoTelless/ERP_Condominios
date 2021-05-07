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
            kernel.Bind<IBancoAppService>().To<BancoAppService>();
            kernel.Bind<IContaBancariaAppService>().To<ContaBancariaAppService>();
            kernel.Bind<IMatrizAppService>().To<MatrizAppService>();
            kernel.Bind<IFilialAppService>().To<FilialAppService>();
            kernel.Bind<IClienteAppService>().To<ClienteAppService>();
            kernel.Bind<IFornecedorAppService>().To<FornecedorAppService>();
            kernel.Bind<IProdutoAppService>().To<ProdutoAppService>();
            kernel.Bind<IMateriaPrimaAppService>().To<MateriaPrimaAppService>();
            kernel.Bind<IServicoAppService>().To<ServicoAppService>();
            kernel.Bind<ITransportadoraAppService>().To<TransportadoraAppService>();
            kernel.Bind<IPatrimonioAppService>().To<PatrimonioAppService>();
            kernel.Bind<IEquipamentoAppService>().To<EquipamentoAppService>();
            kernel.Bind<ICargoAppService>().To<CargoAppService>();
            kernel.Bind<IValorComissaoAppService>().To<ValorComissaoAppService>();
            kernel.Bind<IContratoAppService>().To<ContratoAppService>();
            kernel.Bind<IContratoSolicitacaoAprovacaoAppService>().To<ContratoSolicitacaoAprovacaoAppService>();
            kernel.Bind<INoticiaAppService>().To<NoticiaAppService>();
            kernel.Bind<INotificacaoAppService>().To<NotificacaoAppService>();
            kernel.Bind<IFichaTecnicaAppService>().To<FichaTecnicaAppService>();
            kernel.Bind<IContaReceberAppService>().To<ContaReceberAppService>();
            kernel.Bind<ICategoriaMateriaAppService>().To<CategoriaMateriaAppService>();
            kernel.Bind<ICategoriaProdutoAppService>().To<CategoriaProdutoAppService>();
            kernel.Bind<ISubcategoriaProdutoAppService>().To<SubcategoriaProdutoAppService>();
            kernel.Bind<ICategoriaClienteAppService>().To<CategoriaClienteAppService>();
            kernel.Bind<ICategoriaFornecedorAppService>().To<CategoriaFornecedorAppService>();
            kernel.Bind<ICategoriaContratoAppService>().To<CategoriaContratoAppService>();
            kernel.Bind<IFormaPagamentoAppService>().To<FormaPagamentoAppService>();
            kernel.Bind<IPeriodicidadeAppService>().To<PeriodicidadeAppService>();
            kernel.Bind<ITipoContratoAppService>().To<TipoContratoAppService>();
            kernel.Bind<ITamanhoAppService>().To<TamanhoAppService>();
            kernel.Bind<ITipoPessoaAppService>().To<TipoPessoaAppService>();
            kernel.Bind<ICategoriaEquipamentoAppService>().To<CategoriaEquipamentoAppService>();
            kernel.Bind<IRegimeTributarioAppService>().To<RegimeTributarioAppService>();
            kernel.Bind<IUnidadeAppService>().To<UnidadeAppService>();
            kernel.Bind<ITemplateAppService>().To<TemplateAppService>();
            kernel.Bind<IContaPagarAppService>().To<ContaPagarAppService>();
            kernel.Bind<IPlanoContaAppService>().To<PlanoContaAppService>();
            kernel.Bind<ICentroCustoAppService>().To<CentroCustoAppService>();
            kernel.Bind<IContaReceberParcelaAppService>().To<ContaReceberParcelaAppService>();
            kernel.Bind<IContaReceberTagAppService>().To<ContaReceberTagAppService>();
            kernel.Bind<IContaPagarParcelaAppService>().To<ContaPagarParcelaAppService>();
            kernel.Bind<IContaPagarTagAppService>().To<ContaPagarTagAppService>();
            kernel.Bind<ITarefaAppService>().To<TarefaAppService>();
            kernel.Bind<IPedidoCompraAppService>().To<PedidoCompraAppService>();
            kernel.Bind<IPedidoVendaAppService>().To<PedidoVendaAppService>();
            kernel.Bind<ITipoContaAppService>().To<TipoContaAppService>();
            kernel.Bind<ITipoContribuinteAppService>().To<TipoContribuinteAppService>();
            kernel.Bind<IFuncionarioAppService>().To<FuncionarioAppService>();
            kernel.Bind<IFuncaoAppService>().To<FuncaoAppService>();
            kernel.Bind<ICategoriaPatrimonioAppService>().To<CategoriaPatrimonioAppService>();
            kernel.Bind<ICategoriaServicoAppService>().To<CategoriaServicoAppService>();
            kernel.Bind<IGrupoAppService>().To<GrupoAppService>();
            kernel.Bind<ISubgrupoAppService>().To<SubgrupoAppService>();
            kernel.Bind<IAgendaAppService>().To<AgendaAppService>();
            kernel.Bind<ITipoTagAppService>().To<TipoTagAppService>();
            kernel.Bind<ICategoriaAtendimentoAppService>().To<CategoriaAtendimentoAppService>();
            kernel.Bind<IAtendimentoAppService>().To<AtendimentoAppService>();
            kernel.Bind<IDepartamentoAppService>().To<DepartamentoAppService>();
            kernel.Bind<IContaPagarRateioAppService>().To<ContaPagarRateioAppService>();
            kernel.Bind<IContaReceberRateioAppService>().To<ContaReceberRateioAppService>();
            kernel.Bind<IProdutotabelaPrecoAppService>().To<ProdutoTabelaPrecoAppService>();
            kernel.Bind<IMateriaPrimaPrecoAppService>().To<MateriaPrimaPrecoAppService>();
            kernel.Bind<ISubcategoriaMateriaAppService>().To<SubcategoriaMateriaAppService>();
            kernel.Bind<IMovimentoEstoqueMateriaAppService>().To<MovimentoEstoqueMateriaAppService>();
            kernel.Bind<IMovimentoEstoqueProdutoAppService>().To<MovimentoEstoqueProdutoAppService>();
            kernel.Bind<IClienteCnpjAppService>().To<ClienteCnpjAppService>();
            kernel.Bind<IFornecedorCnpjAppService>().To<FornecedorCnpjAppService>();
            kernel.Bind<IAtendimentoAgendaAppService>().To<AtendimentoAgendaAppService>();
            kernel.Bind<IMateriaEstoqueFilialAppService>().To<MateriaEstoqueFilialAppService>();
            kernel.Bind<IProdutoEstoqueFilialAppService>().To<ProdutoEstoqueFilialAppService>();

            kernel.Bind(typeof(IServiceBase<>)).To(typeof(ServiceBase<>));
            kernel.Bind<IUsuarioService>().To<UsuarioService>();
            kernel.Bind<ILogService>().To<LogService>();
            kernel.Bind<IPerfilService>().To<PerfilService>();
            kernel.Bind<IConfiguracaoService>().To<ConfiguracaoService>();
            kernel.Bind<IBancoService>().To<BancoService>();
            kernel.Bind<IContaBancariaService>().To<ContaBancariaService>();
            kernel.Bind<IMatrizService>().To<MatrizService>();
            kernel.Bind<IFilialService>().To<FilialService>();
            kernel.Bind<IClienteService>().To<ClienteService>();
            kernel.Bind<IFornecedorService>().To<FornecedorService>();
            kernel.Bind<IProdutoService>().To<ProdutoService>();
            kernel.Bind<IMovimentoEstoqueProdutoService>().To<MovimentoEstoqueProdutoService>();
            kernel.Bind<IMateriaPrimaService>().To<MateriaPrimaService>();
            kernel.Bind<IMovimentoEstoqueMateriaService>().To<MovimentoEstoqueMateriaService>();
            kernel.Bind<IServicoService>().To<ServicoService>();
            kernel.Bind<ITransportadoraService>().To<TransportadoraService>();
            kernel.Bind<IPatrimonioService>().To<PatrimonioService>();
            kernel.Bind<IEquipamentoService>().To<EquipamentoService>();
            kernel.Bind<ICargoService>().To<CargoService>();
            kernel.Bind<IValorComissaoService>().To<ValorComissaoService>();
            kernel.Bind<IContratoService>().To<ContratoService>();
            kernel.Bind<INotificacaoService>().To<NotificacaoService>();
            kernel.Bind<IContratoSolicitacaoAprovacaoService>().To<ContratoSolicitacaoAprovacaoService>();
            kernel.Bind<INoticiaService>().To<NoticiaService>();
            kernel.Bind<IFichaTecnicaService>().To<FichaTecnicaService>();
            kernel.Bind<IContaReceberService>().To<ContaReceberService>();
            kernel.Bind<ICategoriaMateriaService>().To<CategoriaMateriaService>();
            kernel.Bind<ICategoriaProdutoService>().To<CategoriaProdutoService>();
            kernel.Bind<ISubcategoriaProdutoService>().To<SubcategoriaProdutoService>();
            kernel.Bind<ICategoriaClienteService>().To<CategoriaClienteService>();
            kernel.Bind<ICategoriaFornecedorService>().To<CategoriaFornecedorService>();
            kernel.Bind<ICategoriaContratoService>().To<CategoriaContratoService>();
            kernel.Bind<IFormaPagamentoService>().To<FormaPagamentoService>();
            kernel.Bind<IPeriodicidadeService>().To<PeriodicidadeService>();
            kernel.Bind<ISexoService>().To<SexoService>();
            kernel.Bind<ITipoContratoService>().To<TipoContratoService>();
            kernel.Bind<ITamanhoService>().To<TamanhoService>();
            kernel.Bind<ITipoPessoaService>().To<TipoPessoaService>();
            kernel.Bind<ICategoriaEquipamentoService>().To<CategoriaEquipamentoService>();
            kernel.Bind<IRegimeTributarioService>().To<RegimeTributarioService>();
            kernel.Bind<IUnidadeService>().To<UnidadeService>();
            kernel.Bind<ITemplateService>().To<TemplateService>();
            kernel.Bind<IContaPagarService>().To<ContaPagarService>();
            kernel.Bind<IPlanoContaService>().To<PlanoContaService>();
            kernel.Bind<ICentroCustoService>().To< CentroCustoService>();
            kernel.Bind<IContaReceberParcelaService>().To<ContaReceberParcelaService>();
            kernel.Bind<IContaReceberTagService>().To<ContaReceberTagService>();
            kernel.Bind<IContaPagarParcelaService>().To<ContaPagarParcelaService>();
            kernel.Bind<IContaPagarTagService>().To<ContaPagarTagService>();
            kernel.Bind<ITarefaService>().To<TarefaService>();
            kernel.Bind<IPedidoCompraService>().To<PedidoCompraService>();
            kernel.Bind<IPedidoVendaService>().To<PedidoVendaService>();
            kernel.Bind<ITipoContaService>().To<TipoContaService>();
            kernel.Bind<ITipoContribuinteService>().To<TipoContribuinteService>();
            kernel.Bind<IFuncionarioService>().To<FuncionarioService>();
            kernel.Bind<IFuncaoService>().To<FuncaoService>();
            kernel.Bind<ICategoriaPatrimonioService>().To<CategoriaPatrimonioService>();
            kernel.Bind<ICategoriaServicoService>().To<CategoriaServicoService>();
            kernel.Bind<IGrupoService>().To<GrupoService>();
            kernel.Bind<ISubgrupoService>().To<SubgrupoService>();
            kernel.Bind<IAgendaService>().To<AgendaService>();
            kernel.Bind<ITipoTagService>().To<TipoTagService>();
            kernel.Bind<ICategoriaAtendimentoService>().To<CategoriaAtendimentoService>();
            kernel.Bind<IAtendimentoService>().To<AtendimentoService>();
            kernel.Bind<IDepartamentoService>().To<DepartamentoService>();
            kernel.Bind<IContaPagarRateioService>().To<ContaPagarRateioService>();
            kernel.Bind<IContaReceberRateioService>().To<ContaReceberRateioService>();
            kernel.Bind<IProdutoTabelaPrecoService>().To<ProdutoTabelaPrecoService>();
            kernel.Bind<IMateriaPrimaPrecoService>().To<MateriaPrimaPrecoService>();
            kernel.Bind<ISubCategoriaMateriaService>().To<SubcategoriaMateriaService>();
            kernel.Bind<IProdutoEstoqueFilialService>().To<ProdutoEstoqueFilialService>();
            kernel.Bind<IMateriaEstoqueFilialService>().To<MateriaEstoqueFilialService>();
            kernel.Bind<IClienteCnpjService>().To<ClienteCnpjService>();
            kernel.Bind<IFornecedorCnpjService>().To<FornecedorCnpjService>();
            kernel.Bind<IAtendimentoAgendaService>().To<AtendimentoAgendaService>();

            kernel.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<>));
            kernel.Bind<IConfiguracaoRepository>().To<ConfiguracaoRepository>();
            kernel.Bind<IUsuarioRepository>().To<UsuarioRepository>();
            kernel.Bind<ILogRepository>().To<LogRepository>();
            kernel.Bind<IPerfilRepository>().To<PerfilRepository>();
            kernel.Bind<ITemplateRepository>().To<TemplateRepository>();
            kernel.Bind<IBancoRepository>().To<BancoRepository>();
            kernel.Bind<IContaBancariaRepository>().To<ContaBancariaRepository>();
            kernel.Bind<ITipoContaRepository>().To<TipoContaRepository>();
            kernel.Bind<IMatrizRepository>().To<MatrizRepository>();
            kernel.Bind<IFilialRepository>().To<FilialRepository>();
            kernel.Bind<IClienteRepository>().To<ClienteRepository>();
            kernel.Bind<ICategoriaClienteRepository>().To<CategoriaClienteRepository>();
            kernel.Bind<IClienteAnexoRepository>().To<ClienteAnexoRepository>();
            kernel.Bind<IFornecedorRepository>().To<FornecedorRepository>();
            kernel.Bind<IFornecedorAnexoRepository>().To<FornecedorAnexoRepository>();
            kernel.Bind<ICategoriaFornecedorRepository>().To<CategoriaFornecedorRepository>();
            kernel.Bind<IProdutoRepository>().To<ProdutoRepository>();
            kernel.Bind<IProdutoAnexoRepository>().To<ProdutoAnexoRepository>();
            kernel.Bind<ICategoriaProdutoRepository>().To<CategoriaProdutoRepository>();
            kernel.Bind<IUnidadeRepository>().To<UnidadeRepository>();
            kernel.Bind<IMovimentoEstoqueProdutoRepository>().To<MovimentoEstoqueProdutoRepository>();
            kernel.Bind<ICategoriaMateriaPrimaRepository>().To<CategoriaMateriaPrimaRepository>();
            kernel.Bind<IMateriaPrimaRepository>().To<MateriaPrimaRepository>();
            kernel.Bind<IMateriaPrimaAnexoRepository>().To<MateriaPrimaAnexoRepository>();
            kernel.Bind<IMovimentoEstoqueMateriaRepository>().To<MovimentoEstoqueMateriaRepository>();
            kernel.Bind<IServicoRepository>().To<ServicoRepository>();
            kernel.Bind<IServicoAnexoRepository>().To<ServicoAnexoRepository>();
            kernel.Bind<ICategoriaServicoRepository>().To<CategoriaServicoRepository>();
            kernel.Bind<ITransportadoraRepository>().To<TransportadoraRepository>();
            kernel.Bind<ITransportadoraAnexoRepository>().To<TransportadoraAnexoRepository>();
            kernel.Bind<ICategoriaEquipamentoRepository>().To<CategoriaEquipamentoRepository>();
            kernel.Bind<ICategoriaPatrimonioRepository>().To<CategoriaPatrimonioRepository>();
            kernel.Bind<IPatrimonioAnexoRepository>().To<PatrimonioAnexoRepository>();
            kernel.Bind<IEquipamentoAnexoRepository>().To<EquipamentoAnexoRepository>();
            kernel.Bind<IPatrimonioRepository>().To<PatrimonioRepository>();
            kernel.Bind<IEquipamentoRepository>().To<EquipamentoRepository>();
            kernel.Bind<ICargoRepository>().To<CargoRepository>();
            kernel.Bind<ITipoComissaoRepository>().To<TipoComissaoRepository>();
            kernel.Bind<IValorComissaoRepository>().To<ValorComissaoRepository>();
            kernel.Bind<IPeriodicidadeRepository>().To<PeriodicidadeRepository>();
            kernel.Bind<IEquipamentoManutencaoRepository>().To<EquipamentoManutencaoRepository>();
            kernel.Bind<IClienteContatoRepository>().To<ClienteContatoRepository>();
            kernel.Bind<IClienteReferenciaRepository>().To<ClienteReferenciaRepository>();
            kernel.Bind<IClienteTagRepository>().To<ClienteTagRepository>();
            kernel.Bind<IColaboradorRepository>().To<ColaboradorRepository>();
            kernel.Bind<ITipoContribuinteRepository>().To<TipoContribuinteRepository>();
            kernel.Bind<ITipoPessoaRepository>().To<TipoPessoaRepository>();
            kernel.Bind<ISubcategoriaProdutoRepository>().To<SubcategoriaProdutoRepository>();
            kernel.Bind<IProdutoFornecedorRepository>().To<ProdutoFornecedorRepository>();
            kernel.Bind<IProdutoGradeRepository>().To<ProdutoGradeRepository>();
            kernel.Bind<ITamanhoRepository>().To<TamanhoRepository>();
            kernel.Bind<IContratoRepository>().To<ContratoRepository>();
            kernel.Bind<IContratoAnexoRepository>().To<ContratoAnexoRepository>();
            kernel.Bind<ITipoContratoRepository>().To<TipoContratoRepository>();
            kernel.Bind<ICategoriaContratoRepository>().To<CategoriaContratoRepository>();
            kernel.Bind<IFormaPagamentoRepository>().To<FormaPagamentoRepository>();
            kernel.Bind<IPlanoContaRepository>().To<PlanoContaRepository>();
            kernel.Bind<ICentroCustoRepository>().To<CentroCustoRepository>();
            kernel.Bind<INomenclaturaRepository>().To<NomenclaturaRepository>();
            kernel.Bind<IStatusContratoRepository>().To<StatusContratoRepository>();
            kernel.Bind<ICategoriaNotificacaoRepository>().To<CategoriaNotificacaoRepository>();
            kernel.Bind<INotificacaoRepository>().To<NotificacaoRepository>();
            kernel.Bind<IContratoSolicitacaoAprovacaoRepository>().To<ContratoSolicitacaoAprovacaoRepository>();
            kernel.Bind<IContaBancariaContatoRepository>().To<ContaBancariaContatoRepository>();
            kernel.Bind<IFornecedorContatoRepository>().To<FornecedorContatoRepository>();
            kernel.Bind<INoticiaRepository>().To<NoticiaRepository>();
            kernel.Bind<INoticiaComentarioRepository>().To<NoticiaComentarioRepository>();
            kernel.Bind<INoticiaTagRepository>().To<NoticiaTagRepository>();
            kernel.Bind<ITipoTagRepository>().To<TipoTagRepository>();
            kernel.Bind<INotificacaoAnexoRepository>().To<NotificacaoAnexoRepository>();
            kernel.Bind<IFichaTecnicaDetalheRepository>().To<FichaTecnicaDetalheRepository>();
            kernel.Bind<IFichaTecnicaRepository>().To<FichaTecnicaRepository>();
            kernel.Bind<IContaReceberRepository>().To<ContaReceberRepository>();
            kernel.Bind<IContaReceberAnexoRepository>().To<ContaReceberAnexoRepository>();
            kernel.Bind<IContratoComentarioRepository>().To<ContratoComentarioRepository>();
            kernel.Bind<ISexoRepository>().To<SexoRepository>();
            kernel.Bind<IRegimeTributarioRepository>().To<RegimeTributarioRepository>();
            kernel.Bind<IContaBancariaLancamentoRepository>().To<ContaBancariaLancamentoRepository>();
            kernel.Bind<IContaPagarRepository>().To<ContaPagarRepository>();
            kernel.Bind<IContaPagarAnexoRepository>().To<ContaPagarAnexoRepository>();
            kernel.Bind<IContaReceberParcelaRepository>().To<ContaReceberParcelaRepository>();
            kernel.Bind<IContaReceberTagRepository>().To<ContaReceberTagRepository>();
            kernel.Bind<IContaPagarParcelaRepository>().To<ContaPagarParcelaRepository>();
            kernel.Bind<IContaPagarTagRepository>().To<ContaPagarTagRepository>();
            kernel.Bind<ITarefaRepository>().To<TarefaRepository>();
            kernel.Bind<ITipoTarefaRepository>().To<TipoTarefaRepository>();
            kernel.Bind<ITarefaAnexoRepository>().To<TarefaAnexoRepository>();
            kernel.Bind<ITarefaNotificacaoRepository>().To<TarefaNotificacaoRepository>();
            kernel.Bind<IPedidoCompraRepository>().To<PedidoCompraRepository>();
            kernel.Bind<IPedidoCompraAnexoRepository>().To<PedidoCompraAnexoRepository>();
            kernel.Bind<IItemPedidoCompraRepository>().To<ItemPedidoCompraRepository>();
            kernel.Bind<IPedidoVendaRepository>().To<PedidoVendaRepository>();
            kernel.Bind<IPedidoVendaAnexoRepository>().To<PedidoVendaAnexoRepository>();
            kernel.Bind<IItemPedidoVendaRepository>().To<ItemPedidoVendaRepository>();
            kernel.Bind<IUsuarioAnexoRepository>().To<UsuarioAnexoRepository>();
            kernel.Bind<IUFRepository>().To<UFRepository>();
            kernel.Bind<IResumoVendaRepository>().To<ResumoVendaRepository>();
            kernel.Bind<IMateriaPrimaFornecedorRepository>().To<MateriaPrimaFornecedorRepository>();
            kernel.Bind<TipoContaRepository>().To<TipoContaRepository>();
            kernel.Bind<IFuncionarioRepository>().To<FuncionarioRepository>();
            kernel.Bind<IFuncionarioAnexoRepository>().To<FuncionarioAnexoRepository>();
            kernel.Bind<ISituacaoRepository>().To<SituacaoRepository>();
            kernel.Bind<IFuncaoRepository>().To<FuncaoRepository>();
            kernel.Bind<IEscolaridadeRepository>().To<EscolaridadeRepository>();
            kernel.Bind<INomencBrasServicosRepository>().To<NomencBrasServicosRepository>();
            kernel.Bind<IGrupoRepository>().To<GrupoRepository>();
            kernel.Bind<ISubgrupoRepository>().To<SubgrupoRepository>();
            kernel.Bind<IAgendaRepository>().To<AgendaRepository>();
            kernel.Bind<IAgendaAnexoRepository>().To<AgendaAnexoRepository>();
            kernel.Bind<ICategoriaAgendaRepository>().To<CategoriaAgendaRepository>();
            kernel.Bind<ICategoriaAtendimentoRepository>().To<CategoriaAtendimentoRepository>();
            kernel.Bind<IAtendimentoRepository>().To<AtendimentoRepository>();
            kernel.Bind<IAtendimentoAnexoRepository>().To<AtendimentoAnexoRepository>();
            kernel.Bind<IDepartamentoRepository>().To<DepartamentoRepository>();
            kernel.Bind<IContaPagarRateioRepository>().To<ContaPagarRateioRepository>();
            kernel.Bind<IContaReceberRateioRepository>().To<ContaReceberRateioRepository>();
            kernel.Bind<IProdutoOrigemRepository>().To<ProdutoOrigemRepository>();
            kernel.Bind<IProdutoTabelaPrecoRepository>().To<ProdutoTabelaPrecoRepository>();
            kernel.Bind<IMateriaPrimaPrecoRepository>().To<MateriaPrimaPrecoRepository>();
            kernel.Bind<ISubcategoriaMateriaRepository>().To<SubcategoriaMateriaRepository>();
            kernel.Bind<IProdutoEstoqueFilialRepository>().To<ProdutoEstoqueFilialRepository>();
            kernel.Bind<IMateriaEstoqueFilialRepository>().To<MateriaEstoqueFilialRepository>();
            kernel.Bind<IClienteCnpjRepository>().To<ClienteCnpjRepository>();
            kernel.Bind<IFornecedorCnpjRepository>().To<FornecedorCnpjRepository>();
            kernel.Bind<ITipoVeiculoRepository>().To<TipoVeiculoRepository>();
            kernel.Bind<ITipoTransporteRepository>().To<TipoTransporteRepository>();
            kernel.Bind<IAtendimentoAgendaRepository>().To<AtendimentoAgendaRepository>();
            kernel.Bind<IPedidoCompraAcompanhamentoRepository>().To<PedidoCompraAcompanhamentoRepository>();

        }
    }
}