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
    public class FuncionarioAppService : AppServiceBase<FUNCIONARIO>, IFuncionarioAppService
    {
        private readonly IFuncionarioService _baseService;

        public FuncionarioAppService(IFuncionarioService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<FUNCIONARIO> GetAllItens()
        {
            List<FUNCIONARIO> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<FUNCIONARIO> GetAllItensAdm()
        {
            List<FUNCIONARIO> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public FUNCIONARIO GetItemById(Int32 id)
        {
            FUNCIONARIO item = _baseService.GetItemById(id);
            return item;
        }

        public FUNCIONARIO GetByNome(String nome)
        {
            FUNCIONARIO item = _baseService.GetByNome(nome);
            return item;
        }

        public FUNCIONARIO CheckExist(FUNCIONARIO conta)
        {
            FUNCIONARIO item = _baseService.CheckExist(conta);
            return item;
        }

        public List<SITUACAO> GetAllSituacao()
        {
            List<SITUACAO> lista = _baseService.GetAllSituacao();
            return lista;
        }

        public List<SEXO> GetAllSexo()
        {
            List<SEXO> lista = _baseService.GetAllSexo();
            return lista;
        }

        public FUNCIONARIO_ANEXO GetAnexoById(Int32 id)
        {
            FUNCIONARIO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public List<FUNCAO> GetAllFuncao()
        {
            List<FUNCAO> lista = _baseService.GetAllFuncao();
            return lista;
        }

        public List<ESCOLARIDADE> GetAllEscolaridade()
        {
            List<ESCOLARIDADE> lista = _baseService.GetAllEscolaridade();
            return lista;
        }

        public Int32 ExecuteFilter(Int32? sitId, String nome, String cpf, String rg, Int32? funId, out List<FUNCIONARIO> objeto)
        {
            try
            {
                objeto = new List<FUNCIONARIO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(sitId, nome, cpf, rg, funId);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(FUNCIONARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.FUNC_IN_ATIVO = 1;
                item.ASSI_CD_ID = SessionMocks.IdAssinante;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddFUNC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FUNCIONARIO>(item)
                };

                // Persiste item
                Int32 volta = _baseService.Create(item, log);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FUNCIONARIO item, FUNCIONARIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditFUNC",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FUNCIONARIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<FUNCIONARIO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FUNCIONARIO item, FUNCIONARIO itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(FUNCIONARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                //if (item.INVENTARIO_ITEM.Count > 0)
                //{
                //    return 1;
                //}
                //if (item.ITEM_PEDIDO_COMPRA.Count > 0)
                //{
                //    return 1;
                //}
                //if (item.INSUMO_MOVIMENTO_ESTOQUE.Count > 0)
                //{
                //    return 1;
                //}

                // Acerta campos
                item.FUNC_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelFUNC",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FUNCIONARIO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FUNCIONARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.FUNC_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = SessionMocks.IdAssinante,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatFUNC",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FUNCIONARIO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
