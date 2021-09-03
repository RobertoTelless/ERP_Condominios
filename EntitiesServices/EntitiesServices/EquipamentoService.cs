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
    public class EquipamentoService : ServiceBase<EQUIPAMENTO>, IEquipamentoService
    {
        private readonly IEquipamentoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICategoriaEquipamentoRepository _tipoRepository;
        private readonly IEquipamentoAnexoRepository _anexoRepository;
        private readonly IEquipamentoManutencaoRepository _manRepository;
        private readonly IPeriodicidadeRepository _perRepository;

        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public EquipamentoService(IEquipamentoRepository baseRepository, ILogRepository logRepository, ICategoriaEquipamentoRepository tipoRepository, IEquipamentoAnexoRepository anexoRepository, IPeriodicidadeRepository perRepository, IEquipamentoManutencaoRepository manRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _perRepository = perRepository;
            _manRepository = manRepository;
        }

        public EQUIPAMENTO CheckExist(EQUIPAMENTO conta, Int32 idAss)
        {
            EQUIPAMENTO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public EQUIPAMENTO GetItemById(Int32 id)
        {
            EQUIPAMENTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public EQUIPAMENTO_MANUTENCAO GetItemManutencaoById(Int32 id)
        {
            EQUIPAMENTO_MANUTENCAO item = _manRepository.GetItemById(id);
            return item;
        }

        public EQUIPAMENTO GetByNumero(String numero, Int32 idAss)
        {
            EQUIPAMENTO item = _baseRepository.GetByNumero(numero, idAss);
            return item;
        }

        public List<EQUIPAMENTO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<EQUIPAMENTO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<CATEGORIA_EQUIPAMENTO> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public List<PERIODICIDADE> GetAllPeriodicidades(Int32 idAss)
        {
            return _perRepository.GetAllItens(idAss);
        }

        public EQUIPAMENTO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<EQUIPAMENTO> ExecuteFilter(Int32? catId, String nome, String numero, Int32? depreciado, Int32? manutencao, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(catId, nome, numero, depreciado, manutencao, idAss);

        }

        public Int32 CalcularManutencaoVencida(Int32 idAss)
        {
            return _baseRepository.CalcularManutencaoVencida(idAss);
        }

        public Int32 CalcularDepreciados(Int32 idAss)
        {
            return _baseRepository.CalcularDepreciados(idAss);
        }

        public Int32 Create(EQUIPAMENTO item, LOG log)
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

        public Int32 Create(EQUIPAMENTO item)
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


        public Int32 Edit(EQUIPAMENTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EQUIPAMENTO obj = _baseRepository.GetById(item.EQUI_CD_ID);
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

        public Int32 Edit(EQUIPAMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EQUIPAMENTO obj = _baseRepository.GetById(item.EQUI_CD_ID);
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

        public Int32 Delete(EQUIPAMENTO item, LOG log)
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

        public Int32 EditManutencao(EQUIPAMENTO_MANUTENCAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EQUIPAMENTO_MANUTENCAO obj = _manRepository.GetById(item.EQMA_CD_ID);
                    _manRepository.Detach(obj);
                    _manRepository.Update(item);
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

        public Int32 CreateManutencao(EQUIPAMENTO_MANUTENCAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.ASSINANTE = null;
                    _manRepository.Add(item);
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
