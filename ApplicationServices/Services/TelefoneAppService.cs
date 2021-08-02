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
    public class TelefoneAppService : AppServiceBase<TELEFONE>, ITelefoneAppService
    {
        private readonly ITelefoneService _baseService;

        public TelefoneAppService(ITelefoneService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TELEFONE> GetAllItens(Int32 idAss)
        {
            List<TELEFONE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<UF> GetAllUF()
        {
            List<UF> lista = _baseService.GetAllUF();
            return lista;
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public List<TELEFONE> GetAllItensAdm(Int32 idAss)
        {
            List<TELEFONE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TELEFONE GetItemById(Int32 id)
        {
            TELEFONE item = _baseService.GetItemById(id);
            return item;
        }

        public TELEFONE CheckExist(TELEFONE conta, Int32 idAss)
        {
            TELEFONE item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_TELEFONE> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_TELEFONE> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? catId, String nome, String telefone, String cidade, Int32? uf, String celular, String email, Int32 idAss, out List<TELEFONE> objeto)
        {
            try
            {
                objeto = new List<TELEFONE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, nome, telefone, cidade, uf, celular, email, idAss);
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

        public Int32 ValidateCreate(TELEFONE item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.TELE_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Checa endereço
                if (String.IsNullOrEmpty(item.TELE_NM_ENDERECO))
                {
                    item.TELE_NM_ENDERECO = "-";
                }
                if (String.IsNullOrEmpty(item.TELE_NM_BAIRRO))
                {
                    item.TELE_NM_BAIRRO = "-";
                }
                if (String.IsNullOrEmpty(item.TELE_NM_CIDADE))
                {
                    item.TELE_NM_CIDADE = "-";
                }
                if (String.IsNullOrEmpty(item.TELE_NR_CEP))
                {
                    item.TELE_NR_CEP = "-";
                }
                if (item.UF_CD_ID == null)
                {
                    item.UF_CD_ID = 18;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddTELE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TELEFONE>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TELEFONE item, TELEFONE itemAntes, USUARIO usuario)
        {
            try
            {
                // Checa endereço
                if (String.IsNullOrEmpty(item.TELE_NM_ENDERECO))
                {
                    item.TELE_NM_ENDERECO = "-";
                }
                if (String.IsNullOrEmpty(item.TELE_NM_BAIRRO))
                {
                    item.TELE_NM_BAIRRO = "-";
                }
                if (String.IsNullOrEmpty(item.TELE_NM_CIDADE))
                {
                    item.TELE_NM_CIDADE = "-";
                }
                if (String.IsNullOrEmpty(item.TELE_NR_CEP))
                {
                    item.TELE_NR_CEP = "-";
                }
                if (item.UF_CD_ID == null)
                {
                    item.UF_CD_ID = 28;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditTELE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TELEFONE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TELEFONE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TELEFONE item, TELEFONE itemAntes)
        {
            try
            {
                // Checa endereço
                if (String.IsNullOrEmpty(item.TELE_NM_ENDERECO))
                {
                    item.TELE_NM_ENDERECO = "-";
                }
                if (String.IsNullOrEmpty(item.TELE_NM_BAIRRO))
                {
                    item.TELE_NM_BAIRRO = "-";
                }
                if (String.IsNullOrEmpty(item.TELE_NM_CIDADE))
                {
                    item.TELE_NM_CIDADE = "-";
                }
                if (String.IsNullOrEmpty(item.TELE_NR_CEP))
                {
                    item.TELE_NR_CEP = "-";
                }
                if (item.UF_CD_ID == null)
                {
                    item.UF_CD_ID = 28;
                }

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TELEFONE item, USUARIO usuario)
        {
            try
            {
               
                // Acerta campos
                item.TELE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTELE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TELEFONE>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TELEFONE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TELE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTELE",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TELEFONE>(item)
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
