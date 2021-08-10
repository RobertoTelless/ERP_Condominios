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
    public class MudancaService : ServiceBase<SOLICITACAO_MUDANCA>, IMudancaService
    {
        private readonly IMudancaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IMudancaAnexoRepository _anexoRepository;
        private readonly IUnidadeRepository _toRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IMudancaComentarioRepository _mudRepository;
        private readonly ICategoriaNotificacaoRepository _cnRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public MudancaService(IMudancaRepository baseRepository, ILogRepository logRepository, IMudancaAnexoRepository anexoRepository, IUnidadeRepository toRepository, IUsuarioRepository usuRepository, IMudancaComentarioRepository mudRepository, ICategoriaNotificacaoRepository cnRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _anexoRepository = anexoRepository;
            _toRepository = toRepository;
            _usuRepository = usuRepository;
            _mudRepository = mudRepository;
            _cnRepository = cnRepository;
        }

        public SOLICITACAO_MUDANCA CheckExist(SOLICITACAO_MUDANCA tarefa, Int32 idAss)
        {
            SOLICITACAO_MUDANCA item = _baseRepository.CheckExist(tarefa, idAss);
            return item;
        }

        public SOLICITACAO_MUDANCA GetItemById(Int32 id)
        {
            SOLICITACAO_MUDANCA item = _baseRepository.GetItemById(id);
            return item;
        }

        public SOLICITACAO_MUDANCA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<SOLICITACAO_MUDANCA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<SOLICITACAO_MUDANCA> GetByUnidade(Int32 idUnid)
        {
            return _baseRepository.GetByUnidade(idUnid);
        }

        public SOLICITACAO_MUDANCA_COMENTARIO GetComentarioById(Int32 id)
        {
            return _mudRepository.GetItemById(id);
        }

        public List<SOLICITACAO_MUDANCA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _toRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss)
        {
            return _cnRepository.GetAllItens(idAss);
        }

        public Int32 Create(SOLICITACAO_MUDANCA item, LOG log)
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

        public Int32 Create(SOLICITACAO_MUDANCA item)
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


        public Int32 Edit(SOLICITACAO_MUDANCA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.UNIDADE = null;
                    SOLICITACAO_MUDANCA obj = _baseRepository.GetById(item.SOMU_CD_ID);
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

        public Int32 Edit(SOLICITACAO_MUDANCA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    SOLICITACAO_MUDANCA obj = _baseRepository.GetById(item.SOMU_CD_ID);
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

        public Int32 Delete(SOLICITACAO_MUDANCA item, LOG log)
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

        public List<SOLICITACAO_MUDANCA> ExecuteFilter(DateTime? data, Int32? entrada, Int32? status, Int32? idUnid, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(data, entrada, status, idUnid, idAss);
        }
    }
}
