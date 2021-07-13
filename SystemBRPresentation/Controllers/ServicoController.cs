using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using SystemBRPresentation.App_Start;
using EntitiesServices.Work_Classes;
using AutoMapper;
using SystemBRPresentation.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using EntitiesServices.WorkClasses;
using System.Threading.Tasks;
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class ServicoController : Controller
    {
        private readonly IServicoAppService servApp;
        private readonly ILogAppService logApp;
        private readonly IUnidadeAppService unApp;
        private readonly ICategoriaServicoAppService csApp;
        private readonly IFilialAppService filApp;
        private readonly IMatrizAppService matrizApp;
        private readonly IPedidoVendaAppService pedvApp;
        private readonly IServicoTabelaPrecoAppService stbApp;

        private String msg;
        private Exception exception;
        SERVICO objetoServ = new SERVICO();
        SERVICO objetoServAntes = new SERVICO();
        List<SERVICO> listaMasterServ = new List<SERVICO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public ServicoController(IServicoAppService servApps, ILogAppService logApps, IUnidadeAppService unApps, ICategoriaServicoAppService csApps, IFilialAppService filApps, IMatrizAppService matrizApps, IPedidoVendaAppService pedvApps, IServicoTabelaPrecoAppService stbApps)
        {
            servApp = servApps;
            logApp = logApps;
            unApp = unApps;
            csApp = csApps;
            filApp = filApps;
            matrizApp = matrizApps;
            pedvApp = pedvApps;
            stbApp = stbApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            SERVICO item = new SERVICO();
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            return View(vm);
        }

        public ActionResult Voltar()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult DashboardAdministracao()
        {
            
            listaMasterServ = new List<SERVICO>();
            SessionMocks.servico = null;
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaServico()
        {
            
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            usuario = SessionMocks.UserCredentials;
            //if (SessionMocks.UserCredentials != null)
            //{
            //    usuario = SessionMocks.UserCredentials;

            //    // Verfifica permissão
            //    if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
            //    {
            //        return RedirectToAction("CarregarBase", "BaseAdmin");
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Login", "ControleAcesso");
            //}

            // Carrega listas
            if (SessionMocks.listaServico == null)
            {
                listaMasterServ = servApp.GetAllItens();
                SessionMocks.listaServico = listaMasterServ;
            }
            ViewBag.Listas = SessionMocks.listaServico;
            ViewBag.Title = "Serviços";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos().OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(SessionMocks.Filiais, "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(SessionMocks.Unidades, "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE(), "NBSE_CD_ID", "NBSE_NM_NOME");

            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Servicos = servApp.GetAllItens().Count;
            
            // Abre view
            objetoServ = new SERVICO();
            return View(objetoServ);
        }

        public ActionResult RetirarFiltroServico()
        {
            
            SessionMocks.listaServico = null;
            SessionMocks.filtroServico = null;
            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult MostrarTudoServico()
        {
            
            listaMasterServ = servApp.GetAllItensAdm();
            SessionMocks.filtroServico = null;
            SessionMocks.listaServico = listaMasterServ;
            return RedirectToAction("MontarTelaServico");
        }

        [HttpPost]
        public ActionResult FiltrarServico(SERVICO item)
        {
            
            try
            {
                // Executa a operação
                List<SERVICO> listaObj = new List<SERVICO>();
                SessionMocks.filtroServico = item;
                Int32 volta = servApp.ExecuteFilter(item.CASE_CD_ID, item.SERV_NM_NOME, item.SERV_DS_DESCRICAO, item.SERV_TX_OBSERVACOES, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    ViewBag.Message = SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture);
                }

                // Sucesso
                listaMasterServ = listaObj;
                SessionMocks.listaServico = listaObj;
                return RedirectToAction("MontarTelaServico");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaProduto");
            }
        }

        public ActionResult VoltarBaseServico()
        {
            
            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult IncluirCategoriaServico()
        {
            
            return RedirectToAction("IncluirCategoriaServico", "TabelasAuxiliares");
        }

        [HttpGet]
        public ActionResult IncluirServico()
        {
            
            // Prepara listas
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos().OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens().Where(x => x.UNID_IN_TIPO_UNIDADE == 2).OrderBy(x => x.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(x => x.NBSE_NM_NOME).ToList<NOMENCLATURA_BRAS_SERVICOS>(), "NBSE_CD_ID", "NBSE_NM_NOME");
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            SessionMocks.voltaPop = 2;
            SERVICO item = new SERVICO();
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.SERV_DT_CADASTRO = DateTime.Today;
            vm.SERV_IN_ATIVO = 1;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public void MontarListaCusto(SERVICO_TABELA_PRECO item)
        {
            Task.Run(() => {
                if (SessionMocks.listaPrecoServico == null)
                {
                    SessionMocks.listaPrecoServico = new List<SERVICO_TABELA_PRECO>();
                }

                item.SETP_IN_ATIVO = 1;
                item.SETP_DT_DATA_REAJUSTE = DateTime.Now;

                SessionMocks.listaPrecoServico.Add(item);
            });
        }

        [HttpPost]
        public void RemovePrecoTabela(SERVICO_TABELA_PRECO item)
        {
            Task.Run(() => {
                if (SessionMocks.listaPrecoServico != null)
                {
                    SessionMocks.listaPrecoServico.RemoveAll(x => x.FILI_CD_ID == item.FILI_CD_ID);
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirServico(ServicoViewModel vm)
        {
            
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos().OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens().Where(x => x.UNID_IN_TIPO_UNIDADE == 2).OrderBy(x => x.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(x => x.NBSE_NM_NOME).ToList<NOMENCLATURA_BRAS_SERVICOS>(), "NBSE_CD_ID", "NBSE_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SERVICO item = Mapper.Map<ServicoViewModel, SERVICO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = servApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0112", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Servicos/" + item.SERV_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    SessionMocks.idVolta = item.SERV_CD_ID;
                    if (Session["FileQueueServico"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueServico"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueServico(file);
                        }

                        Session["FileQueueServico"] = null;
                    }

                    if (SessionMocks.listaPrecoServico != null)
                    {
                        IncluirTabelaServico(item.SERV_CD_ID);
                    }

                    // Sucesso
                    listaMasterServ = new List<SERVICO>();
                    SessionMocks.listaServico = null;
                    return RedirectToAction("MontarTelaServico");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public void IncluirTabelaServico(Int32 id)
        {
            SessionMocks.listaPrecoServico.Select(c => { c.SERV_CD_ID = id; return c; }).ToList<SERVICO_TABELA_PRECO>();

            Int32 volta = stbApp.ValidateCreateLista(SessionMocks.listaPrecoServico);

            SessionMocks.listaPrecoServico = null;
        }

        public ActionResult IncluirItemTabelaServico(SERVICO_TABELA_PRECO item)
        {
            try
            {
                // Executa a operação
                Int32 idAss = SessionMocks.IdAssinante;
                USUARIO usuarioLogado = SessionMocks.UserCredentials;

                item.SETP_IN_ATIVO = 1;
                item.SETP_DT_DATA_REAJUSTE = DateTime.Now;

                Int32 volta = stbApp.ValidateCreate(item);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensPreco"] = 1;
                }

                return RedirectToAction("EditarServico", new { id = item.SERV_CD_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("EditarServico", new { id = item.SERV_CD_ID });
            }
        }

        [HttpPost]
        public ActionResult EditarPC(SERVICO_TABELA_PRECO item, Int32 id)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;

                item.SERV_CD_ID = id;
                item.SETP_DT_DATA_REAJUSTE = DateTime.Today.Date;
                item.SETP_IN_ATIVO = 1;

                Int32 volta = stbApp.ValidateEdit(item, id);

                return RedirectToAction("EditarServico", new { id = item.SERV_CD_ID });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("EditarProduto", new { id = item.SERV_CD_ID });
            }
        }

        [HttpGet]
        public ActionResult ExcluirTabelaServico(Int32 id)
        {
            
            // Verifica se tem usuario logado
            Int32 idAss = SessionMocks.IdAssinante;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            SERVICO_TABELA_PRECO itemAntes = stbApp.GetById(id);
            SERVICO_TABELA_PRECO item = new SERVICO_TABELA_PRECO();
            item.FILI_CD_ID = itemAntes.FILI_CD_ID;
            item.SERV_CD_ID = itemAntes.SERV_CD_ID;
            item.SETP_CD_ID = itemAntes.SETP_CD_ID;
            item.SETP_DT_DATA_REAJUSTE = itemAntes.SETP_DT_DATA_REAJUSTE;
            item.SETP_IN_ATIVO = 0;
            item.SETP_NR_MARKUP = itemAntes.SETP_NR_MARKUP;
            item.SETP_VL_CUSTO = itemAntes.SETP_VL_CUSTO;
            item.SETP_VL_PRECO = itemAntes.SETP_VL_PRECO;
            item.SETP_VL_PRECO_PROMOCAO = itemAntes.SETP_VL_PRECO_PROMOCAO;
            item.SETP_VL_DESCONTO_MAXIMO = itemAntes.SETP_VL_DESCONTO_MAXIMO;
            Int32 volta = stbApp.ValidateDelete(item, id);
            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpGet]
        public ActionResult ReativarTabelaServico(Int32 id)
        {
            
            // Verifica se tem usuario logado
            Int32 idAss = SessionMocks.IdAssinante;
            USUARIO usuarioLogado = SessionMocks.UserCredentials;
            SERVICO_TABELA_PRECO itemAntes = stbApp.GetById(id);
            SERVICO_TABELA_PRECO item = new SERVICO_TABELA_PRECO();
            item.FILI_CD_ID = itemAntes.FILI_CD_ID;
            item.SERV_CD_ID = itemAntes.SERV_CD_ID;
            item.SETP_CD_ID = itemAntes.SETP_CD_ID;
            item.SETP_DT_DATA_REAJUSTE = itemAntes.SETP_DT_DATA_REAJUSTE;
            item.SETP_IN_ATIVO = 1;
            item.SETP_NR_MARKUP = itemAntes.SETP_NR_MARKUP;
            item.SETP_VL_CUSTO = itemAntes.SETP_VL_CUSTO;
            item.SETP_VL_PRECO = itemAntes.SETP_VL_PRECO;
            item.SETP_VL_PRECO_PROMOCAO = itemAntes.SETP_VL_PRECO_PROMOCAO;
            item.SETP_VL_DESCONTO_MAXIMO = itemAntes.SETP_VL_DESCONTO_MAXIMO;
            Int32 volta = stbApp.ValidateReativar(item, id);
            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpGet]
        public ActionResult VerServico(Int32 id)
        {
            
            // Prepara view
            SERVICO item = servApp.GetItemById(id);
            ViewBag.LstPedidos = pedvApp.GetAllItens();
            objetoServAntes = item;
            SessionMocks.servico = item;
            SessionMocks.idVolta = id;
            SessionMocks.voltaPop = 3;
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarServico(Int32 id)
        {
            
            // Prepara view
            SERVICO item = servApp.GetItemById(id);
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos().OrderBy(x => x.CASE_NM_NOME).ToList<CATEGORIA_SERVICO>(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens().Where(x => x.UNID_IN_TIPO_UNIDADE == 2).OrderBy(x => x.UNID_NM_NOME).ToList<UNIDADE>(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE().OrderBy(x => x.NBSE_NM_NOME).ToList<NOMENCLATURA_BRAS_SERVICOS>(), "NBSE_CD_ID", "NBSE_NM_NOME");
            ViewBag.LstPedidos = pedvApp.GetAllItens();
            objetoServAntes = item;

            if (Session["MensPreco"] != null)
            {
                if ((Int32)Session["MensPreco"] == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0108", CultureInfo.CurrentCulture));
                    Session["MensPreco"] = 0;
                }
            }

            SessionMocks.servico = item;
            SessionMocks.idVolta = id;
            SessionMocks.voltaPop = 3;
            ServicoViewModel vm = Mapper.Map<SERVICO, ServicoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarServico(ServicoViewModel vm)
        {
            
            ViewBag.Tipos = new SelectList(servApp.GetAllTipos(), "CASE_CD_ID", "CASE_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Unidades = new SelectList(unApp.GetAllItens(), "UNID_CD_ID", "UNID_NM_NOME");
            ViewBag.Nomes = new SelectList(servApp.GetAllNBSE(), "NBSE_CD_ID", "NBSE_NM_NOME");
            ViewBag.LstPedidos = pedvApp.GetAllItens();
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    SERVICO item = Mapper.Map<ServicoViewModel, SERVICO>(vm);
                    Int32 volta = servApp.ValidateEdit(item, objetoServAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterServ = new List<SERVICO>();
                    SessionMocks.listaServico = null;
                    return RedirectToAction("MontarTelaServico");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirServico(Int32 id)
        {
            
            // Verifica se tem usuario logado
            USUARIO usu = SessionMocks.UserCredentials;

            // Executar
            SERVICO item = servApp.GetItemById(id);
            SERVICO ex = new SERVICO();
            ex.ASSI_CD_ID = item.ASSI_CD_ID;
            ex.CASE_CD_ID = item.CASE_CD_ID;
            ex.FILI_CD_ID = item.FILI_CD_ID;
            ex.MATR_CD_ID = item.MATR_CD_ID;
            ex.NBSE_CD_ID = item.NBSE_CD_ID;
            ex.SERV_CD_CODIGO = item.SERV_CD_CODIGO;
            ex.SERV_CD_ID = item.SERV_CD_ID;
            ex.SERV_DS_DESCRICAO = item.SERV_DS_DESCRICAO;
            ex.SERV_DT_CADASTRO = item.SERV_DT_CADASTRO;
            ex.SERV_IN_ATIVO = 0;
            ex.SERV_NM_NOME = item.SERV_NM_NOME;
            ex.SERV_TX_OBSERVACOES = item.SERV_TX_OBSERVACOES;
            ex.SERV_VL_PRECO = item.SERV_VL_PRECO;
            ex.UNID_CD_ID = item.UNID_CD_ID;
            objetoServAntes = item;
            Int32 volta = servApp.ValidateDelete(ex, usu);
            listaMasterServ = new List<SERVICO>();
            SessionMocks.listaServico = null;
            return RedirectToAction("MontarTelaServico");
        }

        [HttpGet]
        public ActionResult ReativarServico(Int32 id)
        {
            
            // Verifica se tem usuario logado
            USUARIO usu = SessionMocks.UserCredentials;
            // Executar
            SERVICO item = servApp.GetItemById(id);
            SERVICO re = new SERVICO();
            re.ASSI_CD_ID = item.ASSI_CD_ID;
            re.CASE_CD_ID = item.CASE_CD_ID;
            re.FILI_CD_ID = item.FILI_CD_ID;
            re.MATR_CD_ID = item.MATR_CD_ID;
            re.NBSE_CD_ID = item.NBSE_CD_ID;
            re.SERV_CD_CODIGO = item.SERV_CD_CODIGO;
            re.SERV_CD_ID = item.SERV_CD_ID;
            re.SERV_DS_DESCRICAO = item.SERV_DS_DESCRICAO;
            re.SERV_DT_CADASTRO = item.SERV_DT_CADASTRO;
            re.SERV_IN_ATIVO = 1;
            re.SERV_NM_NOME = item.SERV_NM_NOME;
            re.SERV_TX_OBSERVACOES = item.SERV_TX_OBSERVACOES;
            re.SERV_VL_PRECO = item.SERV_VL_PRECO;
            re.UNID_CD_ID = item.UNID_CD_ID;
            objetoServAntes = item;
            Int32 volta = servApp.ValidateReativar(re, usu);
            listaMasterServ = new List<SERVICO>();
            SessionMocks.listaServico = null;
            return RedirectToAction("MontarTelaServico");
        }

        [HttpGet]
        public ActionResult VerAnexoServico(Int32 id)
        {
            
            // Prepara view
            SERVICO_ANEXO item = servApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoServico()
        {
            
            return RedirectToAction("EditarServico", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadServico(Int32 id)
        {
            SERVICO_ANEXO item = servApp.GetAnexoById(id);
            String arquivo = item.SEAN_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg"))
            {
                contentType = "image/jpg";
            }
            else if (arquivo.Contains(".png"))
            {
                contentType = "image/png";
            }
            return File(arquivo, contentType, nomeDownload);
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            List<FileQueue> queue = new List<FileQueue>();

            foreach (var file in files)
            {
                FileQueue f = new FileQueue();
                f.Name = Path.GetFileName(file.FileName);
                f.ContentType = Path.GetExtension(file.FileName);

                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                f.Contents = ms.ToArray();

                if (profile != null)
                {
                    if (file.FileName.Equals(profile))
                    {
                        f.Profile = 1;
                    }
                }

                queue.Add(f);
            }

            Session["FileQueueServico"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueServico(FileQueue file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoServico");
            }

            SERVICO item = servApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoServico");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Servicos/" + item.SERV_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            SERVICO_ANEXO foto = new SERVICO_ANEXO();
            foto.SEAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.SEAN_DT_ANEXO = DateTime.Today;
            foto.SEAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.SEAN_IN_TIPO = tipo;
            foto.SEAN_NM_TITULO = fileName;
            foto.SERV_CD_ID = item.SERV_CD_ID;

            item.SERVICO_ANEXO.Add(foto);
            objetoServAntes = item;
            Int32 volta = servApp.ValidateEdit(item, objetoServAntes);
            return RedirectToAction("VoltarAnexoServico");
        }

        [HttpPost]
        public ActionResult UploadFileServico(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoServico");
            }

            SERVICO item = servApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0015", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoServico");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Servicos/" + item.SERV_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            SERVICO_ANEXO foto = new SERVICO_ANEXO();
            foto.SEAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.SEAN_DT_ANEXO = DateTime.Today;
            foto.SEAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.SEAN_IN_TIPO = tipo;
            foto.SEAN_NM_TITULO = fileName;
            foto.SERV_CD_ID = item.SERV_CD_ID;

            item.SERVICO_ANEXO.Add(foto);
            objetoServAntes = item;
            Int32 volta = servApp.ValidateEdit(item, objetoServAntes);
            return RedirectToAction("VoltarAnexoServico");
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ServicoLista" + "_" + data + ".pdf";
            List<SERVICO> lista = SessionMocks.listaServico;
            SERVICO filtro = SessionMocks.filtroServico;
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Serviços - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(new float[] { 70f, 70f, 160f, 300f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Serviços selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 4;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Código", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (SERVICO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_SERVICO.CASE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_CD_CODIGO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.SERV_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.CASE_CD_ID > 0)
                {
                    parametros += "Categoria: " + filtro.CASE_CD_ID;
                    ja = 1;
                }
                if (filtro.SERV_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.SERV_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.SERV_NM_NOME;
                    }
                }
                if (filtro.SERV_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição: " + filtro.SERV_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição: " + filtro.SERV_DS_DESCRICAO;
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("MontarTelaServico");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            
            SERVICO serv = servApp.GetById(SessionMocks.idVolta);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Serviço" + serv.SERV_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/5.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Serviço - Detalhes", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);

            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Dados Gerais
            table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Dados Gerais", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (serv.CATEGORIA_SERVICO != null)
            {
                cell = new PdfPCell(new Paragraph("Categoria: " + serv.CATEGORIA_SERVICO.CASE_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Categoria: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Filial: " + serv.FILIAL.FILI_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (serv.UNIDADE != null)
            {
                cell = new PdfPCell(new Paragraph("Unidade: " + serv.UNIDADE.UNID_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Unidade: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (serv.NOMENCLATURA_BRAS_SERVICOS != null)
            {
                cell = new PdfPCell(new Paragraph("Cod.NBS: " + serv.NOMENCLATURA_BRAS_SERVICOS.NBSE_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Cod.NBS: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Paragraph("Código: " + serv.SERV_CD_CODIGO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome: " + serv.SERV_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Preço: " + serv.SERV_VL_PRECO, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            pdfDoc.Add(table);

            //cell = new PdfPCell(new Paragraph("Descrição: " + serv.SERV_DS_DESCRICAO, meuFont));
            //cell.Border = 0;
            //cell.Colspan = 9;
            //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //table.AddCell(cell);

            //Descirição
            Chunk chunk = new Chunk("Descrição: " + serv.SERV_DS_DESCRICAO, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + serv.SERV_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VoltarAnexoServico");
        }
    }
}