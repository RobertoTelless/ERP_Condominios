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
    public class AmbienteService : ServiceBase<AMBIENTE>, IAmbienteService
    {
        private readonly IAmbienteRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoAmbienteRepository _tipoRepository;
        private readonly IAmbienteImagemRepository _anexoRepository;
        private readonly IUnidadeRepository _uniRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IAmbienteCustoRepository _cusRepository;
        private readonly IAmbienteChaveRepository _chaRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public AmbienteService(IAmbienteRepository baseRepository, ILogRepository logRepository, ITipoAmbienteRepository tipoRepository, IAmbienteImagemRepository anexoRepository, IUnidadeRepository uniRepository, IUsuarioRepository usuRepository, IAmbienteCustoRepository cusRepository, IAmbienteChaveRepository chaRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _uniRepository = uniRepository;
            _usuRepository = usuRepository;
            _cusRepository = cusRepository;
            _chaRepository = chaRepository;
        }

        public AMBIENTE CheckExist(AMBIENTE conta, Int32 idAss)
        {
            AMBIENTE item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public AMBIENTE GetItemById(Int32 id)
        {
            AMBIENTE item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<AMBIENTE> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<AMBIENTE> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<TIPO_AMBIENTE> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public AMBIENTE_IMAGEM GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<AMBIENTE> ExecuteFilter(Int32? tipo, String nome, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(tipo, nome, idAss);

        }

        public Int32 Create(AMBIENTE item, LOG log)
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

        public Int32 Create(AMBIENTE item)
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


        public Int32 Edit(AMBIENTE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AMBIENTE obj = _baseRepository.GetById(item.AMBI_CD_ID);
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

        public Int32 Edit(AMBIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AMBIENTE obj = _baseRepository.GetById(item.AMBI_CD_ID);
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

        public Int32 Delete(AMBIENTE item, LOG log)
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

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _uniRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public AMBIENTE_CUSTO GetAmbienteCustoById(Int32 id)
        {
            return _cusRepository.GetItemById(id);
        }

        public Int32 EditAmbienteCusto(AMBIENTE_CUSTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AMBIENTE_CUSTO obj = _cusRepository.GetById(item.AMCU_CD_ID);
                    _cusRepository.Detach(obj);
                    _cusRepository.Update(item);
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

        public Int32 CreateAmbienteCusto(AMBIENTE_CUSTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _cusRepository.Add(item);
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

        public AMBIENTE_CHAVE GetAmbienteChaveById(Int32 id)
        {
            return _chaRepository.GetItemById(id);
        }

        public Int32 EditAmbienteChave(AMBIENTE_CHAVE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AMBIENTE_CHAVE obj = _chaRepository.GetById(item.AMCH_CD_ID);
                    _chaRepository.Detach(obj);
                    _chaRepository.Update(item);
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

        public Int32 CreateAmbienteChave(AMBIENTE_CHAVE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _chaRepository.Add(item);
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
