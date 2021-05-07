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

        public List<LOG> GetAllItens()
        {
            return _baseService.GetAllItens();
        }

        public List<LOG> GetAllItensDataCorrente()
        {
            return _baseService.GetAllItensDataCorrente();
        }

        public List<LOG> GetAllItensUsuario(Int32 id)
        {
            return _baseService.GetAllItensUsuario(id);
        }


        public List<LOG> GetAllItensMesCorrente()
        {
            return _baseService.GetAllItensMesCorrente();
        }

        public List<LOG> GetAllItensMesAnterior()
        {
            return _baseService.GetAllItensMesAnterior();
        }

        public Int32 ExecuteFilter(Int32? usuId, DateTime? data, String operacao, out List<LOG> objeto)
        {
            try
            {
                objeto = new List<LOG>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(usuId, data, operacao);
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
