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
    public class NoticiaAppService : AppServiceBase<NOTICIA>, INoticiaAppService
    {
        private readonly INoticiaService _baseService;

        public NoticiaAppService(INoticiaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<NOTICIA> GetAllItens(Int32 idAss)
        {
            List<NOTICIA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<NOTICIA> GetAllItensAdm(Int32 idAss)
        {
            List<NOTICIA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public NOTICIA GetItemById(Int32 id)
        {
            NOTICIA item = _baseService.GetItemById(id);
            return item;
        }

        public List<NOTICIA> GetAllItensValidos(Int32 idAss)
        {
            List<NOTICIA> lista = _baseService.GetAllItensValidos(idAss);
            return lista;
        }

        public NOTICIA_COMENTARIO GetComentarioById(Int32 id)
        {
            NOTICIA_COMENTARIO lista = _baseService.GetComentarioById(id);
            return lista;
        }

        public Int32 ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link, Int32 idAss, out List<NOTICIA> objeto)
        {
            try
            {
                objeto = new List<NOTICIA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(titulo, autor, data, texto, link, idAss);
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

        public Int32 ValidateCreate(NOTICIA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.NOTC_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddNOTI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<NOTICIA>(item)
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

        public Int32 ValidateEdit(NOTICIA item, NOTICIA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditNOTI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<NOTICIA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<NOTICIA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(NOTICIA item, NOTICIA itemAntes)
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

        public Int32 ValidateDelete(NOTICIA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.NOTC_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelNOTI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<NOTICIA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(NOTICIA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.NOTC_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatNOTI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<NOTICIA>(item)
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
