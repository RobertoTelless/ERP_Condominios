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
    public class CentroCustoService : ServiceBase<CENTRO_CUSTO>, ICentroCustoService
    {
        private readonly ICentroCustoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public CentroCustoService(ICentroCustoRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;

        }

        public CENTRO_CUSTO GetItemById(Int32 id)
        {
            CENTRO_CUSTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public CENTRO_CUSTO CheckExist(CENTRO_CUSTO obj)
        {
            CENTRO_CUSTO item = _baseRepository.CheckExist(obj);
            return item;
        }

        public List<CENTRO_CUSTO> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<CENTRO_CUSTO> GetAllReceitas()
        {
            return _baseRepository.GetAllReceitas();
        }

        public List<CENTRO_CUSTO> GetAllDespesas()
        {
            return _baseRepository.GetAllDespesas();
        }

        public List<CENTRO_CUSTO> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<CENTRO_CUSTO> ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome)
        {
            return _baseRepository.ExecuteFilter(grupoId, subGrupoId, tipo, movimento, numero, nome);

        }

        public Int32 Create(CENTRO_CUSTO item, LOG log)
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

        public Int32 Create(CENTRO_CUSTO item)
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


        public Int32 Edit(CENTRO_CUSTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CENTRO_CUSTO obj = _baseRepository.GetById(item.CECU_CD_ID);
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

        public Int32 Edit(CENTRO_CUSTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CENTRO_CUSTO obj = _baseRepository.GetById(item.CECU_CD_ID);
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

        public Int32 Delete(CENTRO_CUSTO item, LOG log)
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
