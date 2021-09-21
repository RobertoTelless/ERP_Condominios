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
    public class ReservaService : ServiceBase<RESERVA>, IReservaService
    {
        private readonly IReservaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IFinalidadeReservaRepository _feRepository;
        private readonly IUnidadeRepository _toRepository;
        private readonly IAmbienteRepository _ambRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IReservaComentarioRepository _comRepository;
        private readonly IReservaAnexoRepository _anexoRepository;
        private readonly ICategoriaNotificacaoRepository _cnRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public ReservaService(IReservaRepository baseRepository, ILogRepository logRepository, IFinalidadeReservaRepository feRepository, IUnidadeRepository toRepository, IAmbienteRepository ambRepository, IUsuarioRepository usuRepository, IReservaComentarioRepository comRepository, IReservaAnexoRepository anexoRepository, ICategoriaNotificacaoRepository cnRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _feRepository = feRepository;
            _toRepository = toRepository;
            _ambRepository = ambRepository;
            _usuRepository = usuRepository;
            _comRepository = comRepository;
            _anexoRepository = anexoRepository;
            _cnRepository = cnRepository;
        }

        public RESERVA CheckExist(RESERVA tarefa, Int32 idAss)
        {
            RESERVA item = _baseRepository.CheckExist(tarefa, idAss);
            return item;
        }
        
        public RESERVA GetItemById(Int32 id)
        {
            RESERVA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<RESERVA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<RESERVA> GetByUnidade(Int32 idUnid)
        {
            return _baseRepository.GetByUnidade(idUnid);
        }

        public List<RESERVA> GetByData(DateTime data, Int32 idAss)
        {
            return _baseRepository.GetByData(data, idAss);
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss)
        {
            return _cnRepository.GetAllItens(idAss);
        }

        public List<RESERVA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<FINALIDADE_RESERVA> GetAllFinalidades(Int32 idAss)
        {
            return _feRepository.GetAllItens(idAss);
        }

        public List<AMBIENTE> GetAllAmbientes(Int32 idAss)
        {
            return _ambRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _toRepository.GetAllItens(idAss);
        }

        public RESERVA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public RESERVA_COMENTARIO GetComentarioById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public Int32 Create(RESERVA item, LOG log)
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

        public Int32 Create(RESERVA item)
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


        public Int32 Edit(RESERVA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.UNIDADE = null;
                    RESERVA obj = _baseRepository.GetById(item.RESE_CD_ID);
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

        public Int32 Edit(RESERVA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    RESERVA obj = _baseRepository.GetById(item.RESE_CD_ID);
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

        public Int32 Delete(RESERVA item, LOG log)
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

        public List<RESERVA> ExecuteFilter(String nome, DateTime? data, Int32? finalidade, Int32? ambiente, Int32? unidade, Int32? status, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(nome, data, finalidade, ambiente, unidade, status, idAss);
        }
    }
}
