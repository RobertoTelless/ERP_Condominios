using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class LogService : ServiceBase<LOG>, ILogService
    {
        private readonly ILogRepository _logRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public LogService(ILogRepository logRepository) : base(logRepository)
        {
            _logRepository = logRepository;
        }

        public LOG GetById(Int32 id)
        {
            LOG item = _logRepository.GetById(id);
            return item;
        }

        public List<LOG> GetAllItens()
        {
            return _logRepository.GetAllItens();
        }

        public List<LOG> GetAllItensDataCorrente()
        {
            return _logRepository.GetAllItensDataCorrente();
        }

        public List<LOG> GetAllItensMesCorrente()
        {
            return _logRepository.GetAllItensMesCorrente();
        }

        public List<LOG> GetAllItensMesAnterior()
        {
            return _logRepository.GetAllItensMesAnterior();
        }

        public List<LOG> GetAllItensUsuario(Int32 id)
        {
            return _logRepository.GetAllItensUsuario(id);
        }

        public List<LOG> ExecuteFilter(Int32? usuId, DateTime? data, String operacao)
        {
            List<LOG> lista = _logRepository.ExecuteFilter(usuId, data, operacao);
            return lista;
        }
    }
}
