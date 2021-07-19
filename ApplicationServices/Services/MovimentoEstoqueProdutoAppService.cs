using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class MovimentoEstoqueProdutoAppService : AppServiceBase<MOVIMENTO_ESTOQUE_PRODUTO>, IMovimentoEstoqueProdutoAppService
    {
        private readonly IMovimentoEstoqueProdutoService _baseService;

        public MovimentoEstoqueProdutoAppService(IMovimentoEstoqueProdutoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public MOVIMENTO_ESTOQUE_PRODUTO GetByProdId(Int32 prod, Int32 idAss)
        {
            MOVIMENTO_ESTOQUE_PRODUTO item = _baseService.GetByProdId(prod, idAss);
            return item;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItens(Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public MOVIMENTO_ESTOQUE_PRODUTO GetItemById(Int32 id)
        {
            MOVIMENTO_ESTOQUE_PRODUTO item = _baseService.GetItemById(id);
            return item;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensEntrada(Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseService.GetAllItensEntrada(idAss);
            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensSaida(Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseService.GetAllItensSaida(idAss);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? catId, Int32? subCatId, String nome, String barcode, Int32? idAss, DateTime? dtMov, out List<MOVIMENTO_ESTOQUE_PRODUTO> objeto)
        {
            try
            {
                objeto = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, subCatId, nome, barcode, idAss, dtMov);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                //SessionMocks.listaMovimentoProduto = objeto;
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ExecuteFilterAvulso(Int32? operacao, Int32? tipoMovimento, DateTime? dtInicial, DateTime? dtFinal, Int32? filial, Int32? prod, out List<MOVIMENTO_ESTOQUE_PRODUTO> objeto)
        {
            try
            {
                objeto = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
                Int32 volta = 0;

                objeto = _baseService.ExecuteFilterAvulso(operacao, tipoMovimento, dtInicial, dtFinal, filial, prod);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                //SessionMocks.listaMovimentoProduto = objeto;
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(MOVIMENTO_ESTOQUE_PRODUTO item, USUARIO usuario)
        {
            try
            {
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditMOEP",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MOVIMENTO_ESTOQUE_PRODUTO>(item)
                };

                Int32 volta = _baseService.Edit(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(MOVIMENTO_ESTOQUE_PRODUTO mov, USUARIO usuario)
        {
            try
            {
                if (mov.PRODUTO != null)
                {
                    mov.PRODUTO = null;
                }

                mov.MOEP_IN_ATIVO = 1;

                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddMOEP",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MOVIMENTO_ESTOQUE_PRODUTO>(mov)
                };

                Int32 volta = _baseService.Create(mov);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateLeve(MOVIMENTO_ESTOQUE_PRODUTO mov, USUARIO usuario)
        {
            try
            {
                if (mov.PRODUTO != null)
                {
                    mov.PRODUTO = null;
                }

                mov.MOEP_IN_ATIVO = 1;

                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddMOEP",
                    LOG_IN_ATIVO = 1,
                };

                Int32 volta = _baseService.Create(mov);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateLista(List<MOVIMENTO_ESTOQUE_PRODUTO> lista)
        {
            try
            {
                foreach (var item in lista)
                {

                    if (item.PRODUTO != null)
                    {
                        item.PRODUTO = null;
                    }

                    item.MOEP_IN_ATIVO = 1;

                    Int32 volta = _baseService.Create(item);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return 0;
        }

        public Int32 ValidateDelete(MOVIMENTO_ESTOQUE_PRODUTO item, USUARIO usuario)
        {
            try
            {
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                item.MOEP_IN_ATIVO = 0;

                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelMovPROD",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MOVIMENTO_ESTOQUE_PRODUTO>(item)
                };

                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(MOVIMENTO_ESTOQUE_PRODUTO item, USUARIO usuario)
        {
            try
            {
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.PRODUTO != null)
                {
                    item.PRODUTO = null;
                }

                item.MOEP_IN_ATIVO = 1;

                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatMovPROD",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MOVIMENTO_ESTOQUE_PRODUTO>(item)
                };

                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
