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
    public class LogAppService : AppServiceBase<LOG>, ILogAppService
    {
        private readonly ILogService _baseService;

        public LogAppService(ILogService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public LOG GetById(Int32 id)
        {
            return _baseService.GetById(id);
        }

        public List<LOG> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public List<LOG> GetAllItensDataCorrente(Int32 idAss)
        {
            return _baseService.GetAllItensDataCorrente(idAss);
        }

        public List<LOG> GetAllItensUsuario(Int32 id, Int32 idAss)
        {
            return _baseService.GetAllItensUsuario(id, idAss);
        }


        public List<LOG> GetAllItensMesCorrente(Int32 idAss)
        {
            return _baseService.GetAllItensMesCorrente(idAss);
        }

        public List<LOG> GetAllItensMesAnterior(Int32 idAss)
        {
            return _baseService.GetAllItensMesAnterior(idAss);
        }

        public Int32 ExecuteFilter(Int32? usuId, DateTime? data, String operacao, Int32 idAss, out List<LOG> objeto)
        {
            try
            {
                objeto = new List<LOG>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(usuId, data, operacao, idAss);
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
    }
}
