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
    public class UnidadeAppService : AppServiceBase<UNIDADE>, IUnidadeAppService
    {
        private readonly IUnidadeService _baseService;

        public UnidadeAppService(IUnidadeService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public UNIDADE CheckExist(UNIDADE unid, Int32 idAss)
        {
            UNIDADE item = _baseService.CheckExist(unid, idAss);
            return item;
        }

        public List<UNIDADE> GetAllItens(Int32 idAss)
        {
            List<UNIDADE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<UNIDADE> GetAllItensAdm(Int32 idAss)
        {
            List<UNIDADE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public UNIDADE GetItemById(Int32 id)
        {
            UNIDADE item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(UNIDADE item, USUARIO usuario)
        {
            try
            {
                //Verifica Campos
                if (item.TIPO_UNIDADE != null)
                {
                    item.TIPO_UNIDADE = null;
                }
                if (item.USUARIO != null)
                {
                    item.USUARIO = null;
                }
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.ENCOMENDA != null)
                {
                    item.ENCOMENDA = null;
                }
                if (item.AMBIENTE_CHAVE != null)
                {
                    item.AMBIENTE_CHAVE = null;
                }
                if (item.AUTORIZACAO_ACESSO != null)
                {
                    item.AUTORIZACAO_ACESSO = null;
                }
                if (item.ENTRADA_SAIDA != null)
                {
                    item.ENTRADA_SAIDA = null;
                }
                if (item.LISTA_CONVIDADO != null)
                {
                    item.LISTA_CONVIDADO = null;
                }
                if (item.OCORRENCIA != null)
                {
                    item.OCORRENCIA = null;
                }
                if (item.RESERVA != null)
                {
                    item.RESERVA = null;
                }
                if (item.SOLICITACAO_MUDANCA != null)
                {
                    item.SOLICITACAO_MUDANCA = null;
                }
                if (item.VAGA != null)
                {
                    item.VAGA = null;
                }
                if (item.CONTROLE_VEICULO != null)
                {
                    item.CONTROLE_VEICULO = null;
                }
                if (item.VEICULO != null)
                {
                    item.VEICULO = null;
                }

                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.UNID_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddUNID",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<UNIDADE>(item)
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

        public Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditUNID",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<UNIDADE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<UNIDADE>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes)
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

        public Int32 ValidateDelete(UNIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.USUARIO.Count > 0)
                {
                    return 1;
                }
                if (item.AMBIENTE_CHAVE.Count > 0)
                {
                    return 1;
                }
                if (item.AUTORIZACAO_ACESSO.Count > 0)
                {
                    return 1;
                }
                if (item.ENCOMENDA.Count > 0)
                {
                    return 1;
                }
                if (item.ENTRADA_SAIDA.Count > 0)
                {
                    return 1;
                }
                if (item.LISTA_CONVIDADO.Count > 0)
                {
                    return 1;
                }
                if (item.OCORRENCIA.Count > 0)
                {
                    return 1;
                }
                if (item.RESERVA.Count > 0)
                {
                    return 1;
                }
                if (item.SOLICITACAO_MUDANCA.Count > 0)
                {
                    return 1;
                }
                if (item.VAGA.Count > 0)
                {
                    return 1;
                }
                if (item.CONTROLE_VEICULO.Count > 0)
                {
                    return 1;
                }
                if (item.VEICULO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.UNID_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleUNID",
                    LOG_TX_REGISTRO = "Unidade: " + item.UNID_NM_EXIBE
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(UNIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.UNID_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatUNID",
                    LOG_TX_REGISTRO = "Unidade: " + item.UNID_NM_EXIBE
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
