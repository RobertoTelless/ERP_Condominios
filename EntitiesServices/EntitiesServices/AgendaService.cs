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
    public class AgendaService : ServiceBase<AGENDA>, IAgendaService
    {
        private readonly IAgendaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICategoriaAgendaRepository _tipoRepository;
        private readonly IAgendaAnexoRepository _anexoRepository;

        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public AgendaService(IAgendaRepository baseRepository, ILogRepository logRepository, ICategoriaAgendaRepository tipoRepository, IAgendaAnexoRepository anexoRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;

        }

        public List<AGENDA> GetByDate(DateTime data)
        {
            List<AGENDA> item = _baseRepository.GetByDate(data);
            return item;
        }

        public List<AGENDA> GetByUser(Int32 id)
        {
            List<AGENDA> item = _baseRepository.GetByUser(id);
            return item;
        }

        public AGENDA GetItemById(Int32 id)
        {
            AGENDA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<AGENDA> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<AGENDA> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<AGENDA> ExecuteFilter(DateTime? data, Int32? cat, String titulo, String descricao)
        {
            List<AGENDA> lista = _baseRepository.ExecuteFilter(data, cat, titulo, descricao);
            return lista;
        }

        public AGENDA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<CATEGORIA_AGENDA> GetAllTipos()
        {
            return _tipoRepository.GetAllItens();
        }

        public Int32 Create(AGENDA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(AGENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public Int32 Edit(AGENDA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AGENDA obj = _baseRepository.GetById(item.AGEN_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(AGENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AGENDA obj = _baseRepository.GetById(item.AGEN_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(AGENDA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
