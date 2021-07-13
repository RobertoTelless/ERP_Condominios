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
using SystemBRPresentation.Filters;

namespace SystemBRPresentation.Controllers
{
    [LoginAuthenticationFilter(new String[] { "ADM", "GER", "USU" })]
    public class EquipamentoController : Controller
    {
        private readonly IEquipamentoAppService equiApp;
        private readonly ICategoriaEquipamentoAppService ceApp;
        private readonly IFilialAppService filApp;
        private readonly IMatrizAppService matrizApp;
        private readonly IFornecedorAppService forApp;

        private String msg;
        private Exception exception;
        EQUIPAMENTO objetoEqui = new EQUIPAMENTO();
        EQUIPAMENTO objetoEquiAntes = new EQUIPAMENTO();
        List<EQUIPAMENTO> listaMasterEqui = new List<EQUIPAMENTO>();
        String extensao;

        public EquipamentoController(IEquipamentoAppService equiApps, ICategoriaEquipamentoAppService ceApps, IFilialAppService filApps, IMatrizAppService matrizApps, IFornecedorAppService forApps)
        {
            equiApp = equiApps;
            ceApp = ceApps;
            filApp = filApps;
            matrizApp = matrizApps;
            forApp = forApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            
            EQUIPAMENTO item = new EQUIPAMENTO();
            EquipamentoViewModel vm = Mapper.Map<EQUIPAMENTO, EquipamentoViewModel>(item);
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
            
            listaMasterEqui = new List<EQUIPAMENTO>();
            SessionMocks.equipamento = null;
            return RedirectToAction("CarregarAdmin", "BaseAdmin");
        }

        public ActionResult IncluirCategoriaEquipamento()
        {
            
            SessionMocks.CategoriaToEquipamento = 1;
            return RedirectToAction("IncluirCategoriaEquipamento", "TabelasAuxiliares");
        }

        [HttpGet]
        public ActionResult MontarTelaEquipamento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if (SessionMocks.UserCredentials != null)
            {
                usuario = SessionMocks.UserCredentials;

            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            if (SessionMocks.listaEquipamento == null)
            {
                listaMasterEqui = equiApp.GetAllItens();
                SessionMocks.listaEquipamento = listaMasterEqui;
            }
            if (SessionMocks.listaEquipamento.Count == 0)
            {
                listaMasterEqui = equiApp.GetAllItens();
                SessionMocks.listaEquipamento = listaMasterEqui;
            }

            ViewBag.Listas = SessionMocks.listaEquipamento;
            ViewBag.Title = "Equipamentos";
            SessionMocks.Matriz = matrizApp.GetAllItens().FirstOrDefault();
            ViewBag.Tipos = new SelectList(ceApp.GetAllItens(), "CAEQ_CD_ID", "CAEQ_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");

            List<SelectListItem> depr = new List<SelectListItem>();
            depr.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            depr.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Depre = new SelectList(depr, "Value", "Text");

            List<SelectListItem> manut = new List<SelectListItem>();
            manut.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            manut.Add(new SelectListItem() { Text = "Não", Value = "0" });
            ViewBag.Manutencao = new SelectList(manut, "Value", "Text");

            // Indicadores
            ViewBag.Equipamentos = SessionMocks.listaEquipamento.Count;
            ViewBag.ManutencaoVencida = equiApp.CalcularManutencaoVencida();
            ViewBag.Depreciados = equiApp.CalcularDepreciados();
            ViewBag.Baixados = SessionMocks.listaEquipamento.Where(p => p.EQUI_DT_BAIXA != null & p.EQUI_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList().Count;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            if (Session["MensEquip"] != null)
            {
                if ((Int32)Session["MensEquip"] == 1)
                {
                    ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0010", CultureInfo.CurrentCulture));
                    Session["MensEquip"] = 0;
                }
            }

            // Abre view
            objetoEqui = new EQUIPAMENTO();
            SessionMocks.voltaEquipamento = 1;
            if (SessionMocks.filtroEquipamento != null)
            {
                objetoEqui = SessionMocks.filtroEquipamento;
            }
            return View(objetoEqui);
        }

        public ActionResult RetirarFiltroEquipamento()
        {
            
            SessionMocks.listaEquipamento = null;
            SessionMocks.filtroEquipamento = null;
            return RedirectToAction("MontarTelaEquipamento");
        }

        public ActionResult MostrarTudoEquipamento()
        {
            
            listaMasterEqui = equiApp.GetAllItensAdm();
            SessionMocks.filtroEquipamento = null;
            SessionMocks.listaEquipamento = listaMasterEqui;
            return RedirectToAction("MontarTelaEquipamento");
        }

        [HttpPost]
        public ActionResult FiltrarEquipamento(EQUIPAMENTO item)
        {
            
            try
            {
                // Executa a operação
                List<EQUIPAMENTO> listaObj = new List<EQUIPAMENTO>();
                SessionMocks.filtroEquipamento = item;
                Int32 volta = equiApp.ExecuteFilter(item.CAEQ_CD_ID, item.EQUI_NM_NOME, item.EQUI_NR_NUMERO, item.EQUI_IN_DEPRECIADOS, item.EQUI_IN_MANUTENCAO, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensEquip"] = 1;
                }

                // Sucesso
                listaMasterEqui = listaObj;
                SessionMocks.listaEquipamento = listaObj;
                return RedirectToAction("MontarTelaEquipamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaEquipamento");
            }
        }

        public ActionResult VoltarBaseEquipamento()
        {
            
            if (SessionMocks.voltaEquipamento == 1)
            {
                return RedirectToAction("MontarTelaEquipamento");
            }
            return RedirectToAction("CarregarDashboardInicial", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult IncluirEquipamento()
        {
            
            // Prepara listas
            ViewBag.Tipos = new SelectList(equiApp.GetAllTipos().OrderBy(x => x.CAEQ_NM_NOME).ToList<CATEGORIA_EQUIPAMENTO>(), "CAEQ_CD_ID", "CAEQ_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Periodicidades = new SelectList(equiApp.GetAllPeriodicidades(), "PERI_CD_ID", "PERI_NM_NOME");

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            EQUIPAMENTO item = new EQUIPAMENTO();
            EquipamentoViewModel vm = Mapper.Map<EQUIPAMENTO, EquipamentoViewModel>(item);
            vm.ASSI_CD_ID = SessionMocks.IdAssinante;
            vm.EQUI_DT_CADASTRO = DateTime.Today;
            vm.EQUI_IN_ATIVO = 1;
            vm.MATR_CD_ID = SessionMocks.Matriz.MATR_CD_ID;
            vm.FILI_CD_ID = SessionMocks.idFilial;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirEquipamento(EquipamentoViewModel vm)
        {
            ViewBag.Tipos = new SelectList(equiApp.GetAllTipos(), "CAEQ_CD_ID", "CAEQ_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Periodicidades = new SelectList(equiApp.GetAllPeriodicidades(), "PERI_CD_ID", "PERI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    EQUIPAMENTO item = Mapper.Map<EquipamentoViewModel, EQUIPAMENTO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = equiApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0031", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Carrega foto e processa alteracao
                    item.EQUI_AQ_FOTO = "~/Imagens/Base/FotoBase.jpg";
                    volta = equiApp.ValidateEdit(item, item, usuarioLogado);

                    // Cria pastas
                    String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Equipamento/" + item.EQUI_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Equipamento/" + item.EQUI_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    SessionMocks.idVolta = item.EQUI_CD_ID;
                    if (Session["FileQueueEquipamento"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueEquipamento"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueEquipamento(file);
                            }
                            else
                            {
                                UploadFotoQueueEquipamento(file);
                            }
                        }

                        Session["FileQueueEquipamento"] = null;
                    }

                    // Sucesso
                    listaMasterEqui = new List<EQUIPAMENTO>();
                    SessionMocks.listaEquipamento = null;
                    return RedirectToAction("MontarTelaEquipamento");
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
        public ActionResult EditarEquipamento(Int32 id)
        {
            
            // Prepara view
            ViewBag.Tipos = new SelectList(equiApp.GetAllTipos().OrderBy(x => x.CAEQ_NM_NOME).ToList<CATEGORIA_EQUIPAMENTO>(), "CAEQ_CD_ID", "CAEQ_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens().OrderBy(x => x.FILI_NM_NOME).ToList<FILIAL>(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Periodicidades = new SelectList(equiApp.GetAllPeriodicidades(), "PERI_CD_ID", "PERI_NM_NOME");
            EQUIPAMENTO item = equiApp.GetItemById(id);
            Int32 dias = equiApp.CalcularDiasDepreciacao(item);
            Int32 diasManutencao = equiApp.CalcularDiasManutencao(item);
            ViewBag.Dias = dias;
            ViewBag.Status = item.EQUI_DT_BAIXA != null ? "Baixado" : (dias > 0 ? "Ativo" : "Depreciado");
            ViewBag.DiasManutencao = diasManutencao;
            ViewBag.StatusManutencao = diasManutencao > 0 ? "Normal" : "Atrasada";
            objetoEquiAntes = item;
            SessionMocks.CategoriaToEquipamento = 2;
            SessionMocks.equipamento = item;
            SessionMocks.idVolta = id;
            EquipamentoViewModel vm = Mapper.Map<EQUIPAMENTO, EquipamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarEquipamento(EquipamentoViewModel vm)
        {
            
            ViewBag.Tipos = new SelectList(equiApp.GetAllTipos(), "CAEQ_CD_ID", "CAEQ_NM_NOME");
            ViewBag.Filiais = new SelectList(filApp.GetAllItens(), "FILI_CD_ID", "FILI_NM_NOME");
            ViewBag.Periodicidades = new SelectList(equiApp.GetAllPeriodicidades(), "PERI_CD_ID", "PERI_NM_NOME");
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    EQUIPAMENTO item = Mapper.Map<EquipamentoViewModel, EQUIPAMENTO>(vm);
                    Int32 volta = equiApp.ValidateEdit(item, objetoEquiAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterEqui = new List<EQUIPAMENTO>();
                    SessionMocks.listaEquipamento = null;
                    return RedirectToAction("MontarTelaEquipamento");
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
        public ActionResult ExcluirEquipamento(Int32 id)
        {
            
            // Prepara view
            EQUIPAMENTO item = equiApp.GetItemById(id);
            EquipamentoViewModel vm = Mapper.Map<EQUIPAMENTO, EquipamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ExcluirEquipamento(EquipamentoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                EQUIPAMENTO item = Mapper.Map<EquipamentoViewModel, EQUIPAMENTO>(vm);
                Int32 volta = equiApp.ValidateDelete(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterEqui = new List<EQUIPAMENTO>();
                SessionMocks.listaEquipamento = null;
                return RedirectToAction("MontarTelaEquipamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ReativarEquipamento(Int32 id)
        {
            
            // Prepara view
            EQUIPAMENTO item = equiApp.GetItemById(id);
            EquipamentoViewModel vm = Mapper.Map<EQUIPAMENTO, EquipamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ReativarEquipamento(EquipamentoViewModel vm)
        {
            
            try
            {
                // Executa a operação
                USUARIO usuarioLogado = SessionMocks.UserCredentials;
                EQUIPAMENTO item = Mapper.Map<EquipamentoViewModel, EQUIPAMENTO>(vm);
                Int32 volta = equiApp.ValidateReativar(item, usuarioLogado);

                // Verifica retorno

                // Sucesso
                listaMasterEqui = new List<EQUIPAMENTO>();
                SessionMocks.listaEquipamento = null;
                return RedirectToAction("MontarTelaEquipamento");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerAnexoEquipamento(Int32 id)
        {
            
            // Prepara view
            EQUIPAMENTO_ANEXO item = equiApp.GetAnexoById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoEquipamento()
        {
            
            return RedirectToAction("EditarEquipamento", new { id = SessionMocks.idVolta });
        }

        public FileResult DownloadEquipamento(Int32 id)
        {
            EQUIPAMENTO_ANEXO item = equiApp.GetAnexoById(id);
            String arquivo = item.EQAN_AQ_ARQUIVO;
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

            Session["FileQueueEquipamento"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueEquipamento(FileQueue file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoEquipamento");
            }

            EQUIPAMENTO item = equiApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoEquipamento");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Equipamento/" + item.EQUI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            EQUIPAMENTO_ANEXO foto = new EQUIPAMENTO_ANEXO();
            foto.EQAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.EQAN_DT_ANEXO = DateTime.Today;
            foto.EQAN_IN_ATIVO = 1;
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
            foto.EQAN_IN_TIPO = tipo;
            foto.EQAN_NM_TITULO = fileName;
            foto.EQUI_CD_ID = item.EQUI_CD_ID;

            item.EQUIPAMENTO_ANEXO.Add(foto);
            objetoEquiAntes = item;
            Int32 volta = equiApp.ValidateEdit(item, objetoEquiAntes);
            return RedirectToAction("VoltarAnexoEquipamento");
        }

        [HttpPost]
        public ActionResult UploadFileEquipamento(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoEquipamento");
            }

            EQUIPAMENTO item = equiApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoEquipamento");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Equipamento/" + item.EQUI_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            EQUIPAMENTO_ANEXO foto = new EQUIPAMENTO_ANEXO();
            foto.EQAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.EQAN_DT_ANEXO = DateTime.Today;
            foto.EQAN_IN_ATIVO = 1;
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
            foto.EQAN_IN_TIPO = tipo;
            foto.EQAN_NM_TITULO = fileName;
            foto.EQUI_CD_ID = item.EQUI_CD_ID;

            item.EQUIPAMENTO_ANEXO.Add(foto);
            objetoEquiAntes = item;
            Int32 volta = equiApp.ValidateEdit(item, objetoEquiAntes);
            listaMasterEqui = new List<EQUIPAMENTO>();
            SessionMocks.listaEquipamento = null;
            return RedirectToAction("VoltarAnexoEquipamento");
        }

        [HttpPost]
        public ActionResult UploadFotoQueueEquipamento(FileQueue file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoEquipamento");
            }

            EQUIPAMENTO item = equiApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = file.Name;
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoEquipamento");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Equipamento/" + item.EQUI_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.EQUI_AQ_FOTO = "~" + caminho + fileName;
            objetoEquiAntes = item;
            Int32 volta = equiApp.ValidateEdit(item, objetoEquiAntes);
            return RedirectToAction("VoltarAnexoEquipamento");
        }

        [HttpPost]
        public ActionResult UploadFotoEquipamento(HttpPostedFileBase file)
        {
            
            if (file == null)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0076", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoEquipamento");
            }

            EQUIPAMENTO item = equiApp.GetById(SessionMocks.idVolta);
            USUARIO usu = SessionMocks.UserCredentials;
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 100)
            {
                ModelState.AddModelError("", SystemBR_Resource.ResourceManager.GetString("M0041", CultureInfo.CurrentCulture));
                return RedirectToAction("VoltarAnexoEquipamento");
            }
            String caminho = "/Imagens/" + SessionMocks.IdAssinante.ToString() + "/Equipamento/" + item.EQUI_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.EQUI_AQ_FOTO = "~" + caminho + fileName;
            objetoEquiAntes = item;
            Int32 volta = equiApp.ValidateEdit(item, objetoEquiAntes);
            listaMasterEqui = new List<EQUIPAMENTO>();
            SessionMocks.listaEquipamento = null;
            return RedirectToAction("VoltarAnexoEquipamento");
        }

        [HttpGet]
        public ActionResult VerManutencaoEquipamento(Int32 id)
        {
            
            // Prepara view
            EQUIPAMENTO_MANUTENCAO item = equiApp.GetItemManutencaoById(id);
            return View(item);
        }

        [HttpGet]
        public ActionResult SlideShowEquipamento()
        {
            
            // Prepara view
            EQUIPAMENTO item = equiApp.GetItemById(SessionMocks.idVolta);
            objetoEquiAntes = item;
            EquipamentoViewModel vm = Mapper.Map<EQUIPAMENTO, EquipamentoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult IncluirManutencao()
        {
            
            SessionMocks.listaEquipamento = null;
            listaMasterEqui = null;

            // Prepara view
            USUARIO usuario = SessionMocks.UserCredentials;
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens().OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");
            EQUIPAMENTO_MANUTENCAO item = new EQUIPAMENTO_MANUTENCAO();
            EquipamentoManutencaoViewModel vm = Mapper.Map<EQUIPAMENTO_MANUTENCAO, EquipamentoManutencaoViewModel>(item);
            vm.EQUI_CD_ID = SessionMocks.idVolta;
            vm.EQMA_IN_ATIVO = 1;
            vm.EQMA_DT_MANUTENCAO = DateTime.Today.Date;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirManutencao(EquipamentoManutencaoViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    EQUIPAMENTO_MANUTENCAO item = Mapper.Map<EquipamentoManutencaoViewModel, EQUIPAMENTO_MANUTENCAO>(vm);
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    Int32 volta = equiApp.ValidateCreateManutencao(item);
                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoEquipamento");
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
        public ActionResult EditarManutencao(Int32 id)
        {
            
            // Prepara view
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            EQUIPAMENTO_MANUTENCAO item = equiApp.GetItemManutencaoById(id);
            EquipamentoManutencaoViewModel vm = Mapper.Map<EQUIPAMENTO_MANUTENCAO, EquipamentoManutencaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarManutencao(EquipamentoManutencaoViewModel vm)
        {
            
            ViewBag.Fornecedores = new SelectList(forApp.GetAllItens(), "FORN_CD_ID", "FORN_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = SessionMocks.UserCredentials;
                    EQUIPAMENTO_MANUTENCAO item = Mapper.Map<EquipamentoManutencaoViewModel, EQUIPAMENTO_MANUTENCAO>(vm);
                    Int32 volta = equiApp.ValidateEditManutencao(item);

                    // Verifica retorno
                    return RedirectToAction("VoltarAnexoEquipamento");
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
        public ActionResult ExcluirManutencao(Int32 id)
        {
            
            EQUIPAMENTO_MANUTENCAO item = equiApp.GetItemManutencaoById(id);
            item.EQMA_IN_ATIVO = 0;
            Int32 volta = equiApp.ValidateEditManutencao(item);
            return RedirectToAction("VoltarAnexoEquipamento");
        }

        [HttpGet]
        public ActionResult ReativarManutencao(Int32 id)
        {
            
            EQUIPAMENTO_MANUTENCAO item = equiApp.GetItemManutencaoById(id);
            item.EQMA_IN_ATIVO = 1;
            Int32 volta = equiApp.ValidateEditManutencao(item);
            return RedirectToAction("VoltarAnexoEquipamento");
        }

        public ActionResult VerManutencoesVencidas()
        {
            
            // Prepara view
            SessionMocks.listaManutencaoVencida = equiApp.GetAllItens().Where(p => p.EQUI_DT_MANUTENCAO != null).Where(p => p.EQUI_DT_MANUTENCAO.Value.AddDays(p.PERIODICIDADE.PERI_NR_DIAS) <= DateTime.Today.Date).ToList<EQUIPAMENTO>();
            SessionMocks.listaManutencaoVencida = SessionMocks.listaManutencaoVencida.Where(p => p.EQUI_DT_COMPRA.Value.AddDays(p.EQUI_NR_VIDA_UTIL.Value * 30) > DateTime.Today.Date).ToList<EQUIPAMENTO>();
            ViewBag.numero = SessionMocks.listaManutencaoVencida.Count;
            ViewBag.Lista = SessionMocks.listaManutencaoVencida;
            ViewBag.Perfil = SessionMocks.UserCredentials.PERFIL.PERF_SG_SIGLA;

            // Abre view
            objetoEqui = new EQUIPAMENTO();
            SessionMocks.voltaEquipamento = 2;
            return View(objetoEqui);
        }

        public ActionResult VerDepreciados()
        {
            
            SessionMocks.listaEquipamentoDepreciados = equiApp.GetAllItens().Where(p => p.EQUI_DT_COMPRA.Value.AddDays(p.EQUI_NR_VIDA_UTIL.Value * 30) < DateTime.Today.Date & p.EQUI_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList<EQUIPAMENTO>();
            ViewBag.Listas = SessionMocks.listaEquipamentoDepreciados;
            return View();
        }

        public ActionResult VerBaixados()
        {
            
            // Prepara view
            SessionMocks.listaEquipamentoBaixados = equiApp.GetAllItens().Where(p => p.EQUI_DT_BAIXA != null & p.EQUI_IN_ATIVO == 1 & p.ASSI_CD_ID == SessionMocks.IdAssinante).ToList<EQUIPAMENTO>();
            ViewBag.Listas = SessionMocks.listaEquipamentoBaixados;
            return View();
        }

        public ActionResult GerarRelatorioLista(Int32? tipo)
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = String.Empty;
            String titulo = String.Empty;
            List<EQUIPAMENTO> lista = new List<EQUIPAMENTO>();
            EQUIPAMENTO filtro = null;
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            if (tipo == null)
            {
                lista = SessionMocks.listaEquipamento;
                filtro = SessionMocks.filtroEquipamento;
                nomeRel = "PatrimonioLista" + "_" + data + ".pdf";
                titulo = "Patrimônio - Listagem";
            }
            else if (tipo == 1)
            {
                lista = SessionMocks.listaEquipamentoDepreciados;
                nomeRel = "DepreciadosLista" + "_" + data + ".pdf";
                titulo = "Equipamentos Depreciados - Listagem";
            }
            else if (tipo == 1)
            {
                lista = SessionMocks.listaEquipamentoBaixados;
                nomeRel = "BaixadosLista" + "_" + data + ".pdf";
                titulo = "Equipamentos Baixados - Listagem";
            }

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

            cell = new PdfPCell(new Paragraph(titulo, meuFont2))
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
            table = new PdfPTable(new float[] { 70f, 150f, 60f, 60f, 60f, 60f, 50f, 50f, 80f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 9;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
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
            cell = new PdfPCell(new Paragraph("Número", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Data Compra", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Data Baixa", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Depreciação", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Manutenção", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Imagem", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (EQUIPAMENTO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_EQUIPAMENTO.CAEQ_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.EQUI_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.EQUI_NR_NUMERO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.EQUI_VL_VALOR.Value), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.EQUI_DT_COMPRA.Value.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.EQUI_DT_BAIXA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.EQUI_DT_BAIXA.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(ApplicationServices.Services.EquipamentoAppService.CalcularDiasDepreciacaoStatic(item).ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(ApplicationServices.Services.EquipamentoAppService.CalcularDiasManutencaoStatic(item).ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (System.IO.File.Exists(Server.MapPath(item.EQUI_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.EQUI_AQ_FOTO));
                    image.ScaleAbsolute(30, 30);
                    cell.AddElement(image);
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            if (filtro != null)
            {
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
                    if (filtro.CAEQ_CD_ID > 0)
                    {
                        parametros += "Categoria: " + filtro.CAEQ_CD_ID;
                        ja = 1;
                    }
                    if (filtro.EQUI_NM_NOME != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Nome: " + filtro.EQUI_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Nome: " + filtro.EQUI_NM_NOME;
                        }
                    }
                    if (filtro.EQUI_NR_NUMERO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Número: " + filtro.EQUI_NR_NUMERO;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Número: " + filtro.EQUI_NR_NUMERO;
                        }
                    }
                    if (filtro.EQUI_IN_DEPRECIADOS != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Depreciados: Sim";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Depreciados: Sim";
                        }
                    }
                    if (filtro.EQUI_IN_BAIXADOS != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Baixados: Sim";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Baixados: Sim";
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
            }

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

            if (tipo == null)
            {
                return RedirectToAction("MontarTelaEquipamento");
            }
            else if (tipo == 1)
            {
                return RedirectToAction("VerDepreciados");
            }
            else if (tipo == 1)
            {
                return RedirectToAction("VerBaixados");
            }

            return RedirectToAction("MontarTelaEquipamento");
        }

        public ActionResult GerarRelatorioListaDepreciados()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = String.Empty;
            String titulo = String.Empty;
            List<EQUIPAMENTO> lista = new List<EQUIPAMENTO>();
            EQUIPAMENTO filtro = null;
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            lista = SessionMocks.listaEquipamentoDepreciados;
            nomeRel = "DepreciadosLista" + "_" + data + ".pdf";
            titulo = "Equipamentos Depreciados - Listagem";

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

            cell = new PdfPCell(new Paragraph(titulo, meuFont2))
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
            table = new PdfPTable(new float[] { 70f, 150f, 60f, 60f, 60f, 60f, 50f, 50f, 80f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 9;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
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
            cell = new PdfPCell(new Paragraph("Número", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Data Compra", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Data Baixa", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Depreciação (Dias)", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vida Útil (Meses)", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Imagem", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (EQUIPAMENTO item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_EQUIPAMENTO.CAEQ_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.EQUI_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.EQUI_NR_NUMERO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(CrossCutting.Formatters.DecimalFormatter(item.EQUI_VL_VALOR.Value), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.EQUI_DT_COMPRA.Value.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.EQUI_DT_BAIXA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.EQUI_DT_BAIXA.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(ApplicationServices.Services.EquipamentoAppService.CalcularDiasDepreciacaoStatic(item).ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.EQUI_NR_VIDA_UTIL != null)
                {
                    cell = new PdfPCell(new Paragraph(item.EQUI_NR_VIDA_UTIL.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (System.IO.File.Exists(Server.MapPath(item.EQUI_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.EQUI_AQ_FOTO));
                    image.ScaleAbsolute(30, 30);
                    cell.AddElement(image);
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            if (filtro != null)
            {
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
                    if (filtro.CAEQ_CD_ID > 0)
                    {
                        parametros += "Categoria: " + filtro.CAEQ_CD_ID;
                        ja = 1;
                    }
                    if (filtro.EQUI_NM_NOME != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Nome: " + filtro.EQUI_NM_NOME;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Nome: " + filtro.EQUI_NM_NOME;
                        }
                    }
                    if (filtro.EQUI_NR_NUMERO != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Número: " + filtro.EQUI_NR_NUMERO;
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Número: " + filtro.EQUI_NR_NUMERO;
                        }
                    }
                    if (filtro.EQUI_IN_DEPRECIADOS != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Depreciados: Sim";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Depreciados: Sim";
                        }
                    }
                    if (filtro.EQUI_IN_BAIXADOS != null)
                    {
                        if (ja == 0)
                        {
                            parametros += "Baixados: Sim";
                            ja = 1;
                        }
                        else
                        {
                            parametros += " e Baixados: Sim";
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
            }

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

            return RedirectToAction("VerDepreciados");
        }

        public ActionResult GerarRelatorioManutencaoVencida()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ManutencaoVencida" + "_" + data + ".pdf";
            List<EQUIPAMENTO> lista = SessionMocks.listaManutencaoVencida;
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

            cell = new PdfPCell(new Paragraph("Manutenção Vencida - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 70f, 150f, 60f, 60f, 60f, 60f, 50f, 50f, 80f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
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
            cell.Colspan = 2;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Número", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 2;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Data Manutenção", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Periodicidade", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Manutenção Vencida", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Imagem", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (EQUIPAMENTO item in lista)
            {
                if (item.CATEGORIA_EQUIPAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CATEGORIA_EQUIPAMENTO.CAEQ_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.EQUI_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.EQUI_NR_NUMERO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.Colspan = 2;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.EQUI_DT_MANUTENCAO.Value.ToShortDateString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PERIODICIDADE.PERI_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(ApplicationServices.Services.EquipamentoAppService.CalcularDiasManutencaoStatic(item).ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (System.IO.File.Exists(Server.MapPath(item.EQUI_AQ_FOTO)))
                {
                    cell = new PdfPCell();
                    image = Image.GetInstance(Server.MapPath(item.EQUI_AQ_FOTO));
                    image.ScaleAbsolute(30, 30);
                    cell.AddElement(image);
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VerManutencoesVencidas");
        }

        public ActionResult GerarRelatorioDetalhe()
        {
            
            // Prepara geração
            EQUIPAMENTO aten = equiApp.GetItemById(SessionMocks.idVolta);
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "Patrimonio_" + aten.EQUI_CD_ID.ToString() + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFontBold = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            Int32 dias = equiApp.CalcularDiasDepreciacao(aten);
            Int32 diasManutencao = equiApp.CalcularDiasManutencao(aten);
            String manut = diasManutencao > 0 ? "Normal" : "Atrasada";
            String deprec = dias > 0 ? "Ativo" : "Depreciado";

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

            cell = new PdfPCell(new Paragraph("Patrimônio - Detalhes", meuFont2))
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

            cell = new PdfPCell(new Paragraph("Indicadores", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Status de Manutenção: " + manut, meuFontBold));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Dias para Manutenção: " + diasManutencao.ToString(), meuFontBold));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Depreciação : " + deprec, meuFontBold));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Dias para Depreciação: " + dias.ToString(), meuFontBold));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
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

            if (System.IO.File.Exists(Server.MapPath(aten.EQUI_AQ_FOTO)))
            {
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 1;
                image = Image.GetInstance(Server.MapPath(aten.EQUI_AQ_FOTO));
                image.ScaleAbsolute(50, 50);
                cell.AddElement(image);
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Paragraph(" ", meuFontBold));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell();

            cell = new PdfPCell(new Paragraph("Categoria: " + aten.CATEGORIA_EQUIPAMENTO.CAEQ_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Filial: " + aten.FILIAL.FILI_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome: " + aten.EQUI_NM_NOME, meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Número: " + aten.EQUI_NR_NUMERO, meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Data de Compra: " + aten.EQUI_DT_COMPRA.Value.ToShortDateString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor (R$): " + CrossCutting.Formatters.DecimalFormatter(aten.EQUI_VL_VALOR.Value), meuFont));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vida Útil (Dias): " + aten.EQUI_NR_VIDA_UTIL.ToString(), meuFont));
            cell.Border = 0;
            cell.Colspan = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Descrição : " + aten.EQUI_DS_DESCRICAO, meuFont));
            cell.Border = 0;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            if (aten.EQUI_DT_BAIXA != null)
            {
                cell = new PdfPCell(new Paragraph("Data de Baixa: " + aten.EQUI_DT_BAIXA.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data de Baixa : -", meuFont));
                cell.Border = 0;
                cell.Colspan = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (String.IsNullOrEmpty(aten.EQUI_DS_MOTIVO_BAIXA))
            {
                cell = new PdfPCell(new Paragraph("Motivo da Baixa : " + aten.EQUI_DS_MOTIVO_BAIXA, meuFont));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Motivo da Baixa : -", meuFont));
                cell.Border = 0;
                cell.Colspan = 3;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }

            if (aten.PERIODICIDADE != null)
            {
                cell = new PdfPCell(new Paragraph("Periodicidade da Manutenção: " + aten.PERIODICIDADE.PERI_NM_NOME, meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Periodicidade da Manutenção: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            if (aten.EQUI_DT_MANUTENCAO != null)
            {
                cell = new PdfPCell(new Paragraph("Data da Última Manutenção: " + aten.EQUI_DT_MANUTENCAO.Value.ToShortDateString(), meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(new Paragraph("Data da Última Manutenção: -", meuFont));
                cell.Border = 0;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Manuteções
            if (aten.EQUIPAMENTO_MANUTENCAO.Count > 0)
            {
                table = new PdfPTable(new float[] { 120f, 120f, 120f, 120f});
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;

                cell = new PdfPCell(new Paragraph("Últimas Manutenções", meuFontBold));
                cell.Border = 0;
                cell.Colspan = 4;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);

                cell = new PdfPCell(new Paragraph("Data", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Responsável", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph("Telefones", meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);


                foreach (EQUIPAMENTO_MANUTENCAO item in aten.EQUIPAMENTO_MANUTENCAO)
                {
                    cell = new PdfPCell(new Paragraph(item.EQMA_DT_MANUTENCAO.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_EMAIL, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_TELEFONES, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                pdfDoc.Add(table);
            }

            // Linha Horizontal
            line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);

            // Observações
            Chunk chunk1 = new Chunk("Observações: " + aten.EQUI_TX_OBSERVACOES, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
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
            return RedirectToAction("VoltarAnexoEquipamento");
        }

    }
}