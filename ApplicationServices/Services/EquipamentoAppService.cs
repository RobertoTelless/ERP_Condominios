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
    public class EquipamentoAppService : AppServiceBase<EQUIPAMENTO>, IEquipamentoAppService
    {
        private readonly IEquipamentoService _baseService;

        public EquipamentoAppService(IEquipamentoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<EQUIPAMENTO> GetAllItens(Int32 idAss)
        {
            List<EQUIPAMENTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<EQUIPAMENTO> GetAllItensAdm(Int32 idAss)
        {
            List<EQUIPAMENTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public EQUIPAMENTO GetItemById(Int32 id)
        {
            EQUIPAMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public EQUIPAMENTO_MANUTENCAO GetItemManutencaoById(Int32 id)
        {
            EQUIPAMENTO_MANUTENCAO item = _baseService.GetItemManutencaoById(id);
            return item;
        }

        public EQUIPAMENTO GetByNumero(String numero, Int32 idAss)
        {
            EQUIPAMENTO item = _baseService.GetByNumero(numero, idAss);
            return item;
        }

        public EQUIPAMENTO CheckExist(EQUIPAMENTO conta, Int32 idAss)
        {
            EQUIPAMENTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_EQUIPAMENTO> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_EQUIPAMENTO> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<PERIODICIDADE> GetAllPeriodicidades(Int32 idAss)
        {
            List<PERIODICIDADE> lista = _baseService.GetAllPeriodicidades(idAss);
            return lista;
        }

        public Int32 CalcularDiasDepreciacao(EQUIPAMENTO item)
        {
            Int32 totalDias = 0;
            if (item.EQUI_DT_COMPRA != null & item.EQUI_NR_VIDA_UTIL != null)
            {
                if (item.EQUI_DT_COMPRA != DateTime.MinValue & item.EQUI_NR_VIDA_UTIL > 0)
                {
                    DateTime dataLimite = item.EQUI_DT_COMPRA.Value.AddDays(item.EQUI_NR_VIDA_UTIL.Value * 30);
                    totalDias = dataLimite.Subtract(DateTime.Today).Days;
                }
            }
            return totalDias;
        }

        public static Int32 CalcularDiasDepreciacaoStatic(EQUIPAMENTO item)
        {
            Int32 totalDias = 0;
            if (item.EQUI_DT_COMPRA != null & item.EQUI_NR_VIDA_UTIL != null)
            {
                if (item.EQUI_DT_COMPRA != DateTime.MinValue & item.EQUI_NR_VIDA_UTIL > 0)
                {
                    DateTime dataLimite = item.EQUI_DT_COMPRA.Value.AddDays(item.EQUI_NR_VIDA_UTIL.Value * 30);
                    totalDias = dataLimite.Subtract(DateTime.Today).Days;
                }
            }
            return totalDias;
        }

        public Int32 CalcularDiasManutencao(EQUIPAMENTO item)
        {
            Int32 totalDias = 0;
            if (item.EQUI_DT_COMPRA != null & item.EQUI_NR_VIDA_UTIL != null & item.PERIODICIDADE != null & item.EQUI_IN_AVISA_MANUTENCAO == 1)
            {
                if (item.EQUI_DT_COMPRA != DateTime.MinValue & item.EQUI_NR_VIDA_UTIL > 0)
                {
                    if (item.EQUI_DT_MANUTENCAO == null)
                    {
                        DateTime dataLimite = item.EQUI_DT_COMPRA.Value.AddDays(item.PERIODICIDADE.PERI_NR_DIAS);
                        totalDias = dataLimite.Subtract(DateTime.Today).Days;
                    }
                    else
                    {
                        DateTime dataLimite = item.EQUI_DT_MANUTENCAO.Value.AddDays(item.PERIODICIDADE.PERI_NR_DIAS);
                        totalDias = dataLimite.Subtract(DateTime.Today).Days;
                    }
                }
            }
            return totalDias;
        }

        public static Int32 CalcularDiasManutencaoStatic(EQUIPAMENTO item)
        {
            Int32 totalDias = 0;
            if (item.EQUI_DT_COMPRA != null & item.EQUI_NR_VIDA_UTIL != null & item.PERIODICIDADE != null & item.EQUI_IN_AVISA_MANUTENCAO == 1)
            {
                if (item.EQUI_DT_COMPRA != DateTime.MinValue & item.EQUI_NR_VIDA_UTIL > 0)
                {
                    if (item.EQUI_DT_MANUTENCAO == null)
                    {
                        DateTime dataLimite = item.EQUI_DT_COMPRA.Value.AddDays(item.PERIODICIDADE.PERI_NR_DIAS);
                        totalDias = dataLimite.Subtract(DateTime.Today).Days;
                    }
                    else
                    {
                        DateTime dataLimite = item.EQUI_DT_MANUTENCAO.Value.AddDays(item.PERIODICIDADE.PERI_NR_DIAS);
                        totalDias = dataLimite.Subtract(DateTime.Today).Days;
                    }
                }
            }
            return totalDias;
        }

        public Int32 CalcularManutencaoVencida(Int32 idAss)
        {
            Int32 lista = _baseService.CalcularManutencaoVencida(idAss);
            return lista;
        }

        public Int32 CalcularDepreciados(Int32 idAss)
        {
            Int32 lista = _baseService.CalcularDepreciados(idAss);
            return lista;
        }

        public EQUIPAMENTO_ANEXO GetAnexoById(Int32 id)
        {
            EQUIPAMENTO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? catId, String nome, String numero, Int32? depreciado, Int32? manutencao, Int32 idAss, out List<EQUIPAMENTO> objeto)
        {
            try
            {
                objeto = new List<EQUIPAMENTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, nome, numero, depreciado, manutencao, idAss);
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

        public Int32 ValidateCreate(EQUIPAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.EQUI_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                if (item.EQUI_DT_COMPRA != null & item.EQUI_DT_COMPRA != DateTime.MinValue & item.EQUI_IN_AVISA_MANUTENCAO == 1)
                {
                    item.EQUI_DT_MANUTENCAO = item.EQUI_DT_COMPRA;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddEQUI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EQUIPAMENTO>(item)
                };

                // Persiste patrimonio
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(EQUIPAMENTO item, EQUIPAMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                if (item.ASSINANTE != null)
                {
                    item.ASSINANTE = null;
                }
                if (item.CATEGORIA_EQUIPAMENTO != null)
                {
                    item.CATEGORIA_EQUIPAMENTO = null;
                }
                if (item.EQUIPAMENTO_ANEXO != null)
                {
                    item.EQUIPAMENTO_ANEXO = null;
                }
                if (item.EQUIPAMENTO_MANUTENCAO != null)
                {
                    item.EQUIPAMENTO_MANUTENCAO = null;
                }
                if (item.PERIODICIDADE != null)
                {
                    item.PERIODICIDADE = null;
                }
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditEQUI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EQUIPAMENTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<EQUIPAMENTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(EQUIPAMENTO item, EQUIPAMENTO itemAntes)
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

        public Int32 ValidateDelete(EQUIPAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.EQUI_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelEQUI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EQUIPAMENTO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(EQUIPAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.EQUI_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatEQUI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EQUIPAMENTO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditManutencao(EQUIPAMENTO_MANUTENCAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditManutencao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateManutencao(EQUIPAMENTO_MANUTENCAO item)
        {
            try
            {
                // Persiste
                item.EQMA_IN_ATIVO = 1;
                Int32 volta = _baseService.CreateManutencao(item);

                // Atualiza Equipamento
                EQUIPAMENTO equi = _baseService.GetItemById(item.EQUI_CD_ID);
                equi.EQUI_DT_MANUTENCAO = item.EQMA_DT_MANUTENCAO;
                volta = _baseService.Edit(equi);

                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
