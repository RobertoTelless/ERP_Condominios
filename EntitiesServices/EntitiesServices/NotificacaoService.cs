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
    public class NotificacaoService : ServiceBase<NOTIFICACAO>, INotificacaoService
    {
        private readonly INotificacaoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly INotificacaoAnexoRepository _anexoRepository;
        private readonly ICategoriaNotificacaoRepository _catRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public NotificacaoService(INotificacaoRepository baseRepository, ILogRepository logRepository, INotificacaoAnexoRepository anexoRepository, ICategoriaNotificacaoRepository catRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _anexoRepository = anexoRepository;
            _catRepository = catRepository;
        }

        public NOTIFICACAO GetItemById(Int32 id)
        {
            NOTIFICACAO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<NOTIFICACAO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<NOTIFICACAO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public NOTIFICACAO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<NOTIFICACAO> GetAllItensUser(Int32 id, Int32 idAss)
        {
            return _baseRepository.GetAllItensUser(id, idAss);
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllCategorias(Int32 idAss)
        {
            return _catRepository.GetAllItens(idAss);
        }

        public List<NOTIFICACAO> GetNotificacaoNovas(Int32 id, Int32 idAss)
        {
            return _baseRepository.GetNotificacaoNovas(id, idAss);
        }

        public List<NOTIFICACAO> ExecuteFilter(String titulo, DateTime? data, String texto, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(titulo, data, texto, idAss);

        }

        public Int32 Create(NOTIFICACAO item, LOG log)
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

        public Int32 Create(NOTIFICACAO item)
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


        public Int32 Edit(NOTIFICACAO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    NOTIFICACAO obj = _baseRepository.GetById(item.NOTI_CD_ID);
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

        public Int32 Edit(NOTIFICACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    item.ASSINANTE = null;
                    item.CATEGORIA_NOTIFICACAO = null;
                    NOTIFICACAO obj = _baseRepository.GetById(item.NOTI_CD_ID);
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

        public Int32 Delete(NOTIFICACAO item, LOG log)
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
