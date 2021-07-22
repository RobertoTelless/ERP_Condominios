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
    public class ListaConvidadoService : ServiceBase<LISTA_CONVIDADO>, IListaConvidadoService
    {
        private readonly IListaConvidadoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IReservaRepository _reRepository;
        private readonly IUnidadeRepository _toRepository;
        private readonly ICategoriaNotificacaoRepository _cnRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IConvidadoRepository _contRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public ListaConvidadoService(IListaConvidadoRepository baseRepository, ILogRepository logRepository, IReservaRepository reRepository, IUnidadeRepository toRepository, ICategoriaNotificacaoRepository cnRepository, IUsuarioRepository usuRepository, IConvidadoRepository contRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _toRepository = toRepository;
            _cnRepository = cnRepository;
            _usuRepository = usuRepository;
            _reRepository = reRepository;
            _contRepository = contRepository;
        }

        public LISTA_CONVIDADO CheckExist(LISTA_CONVIDADO tarefa, Int32 idAss)
        {
            LISTA_CONVIDADO item = _baseRepository.CheckExist(tarefa, idAss);
            return item;
        }

        public LISTA_CONVIDADO GetItemById(Int32 id)
        {
            LISTA_CONVIDADO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<LISTA_CONVIDADO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<LISTA_CONVIDADO> GetByUnidade(Int32 idUnid)
        {
            return _baseRepository.GetByUnidade(idUnid);
        }

        public List<LISTA_CONVIDADO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<RESERVA> GetAllReservas(Int32 idAss)
        {
            return _reRepository.GetAllItens(idAss);
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _toRepository.GetAllItens(idAss);
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss)
        {
            return _cnRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public Int32 Create(LISTA_CONVIDADO item, LOG log)
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

        public Int32 Create(LISTA_CONVIDADO item)
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


        public Int32 Edit(LISTA_CONVIDADO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    LISTA_CONVIDADO obj = _baseRepository.GetById(item.LICO_CD_ID);
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

        public Int32 Edit(LISTA_CONVIDADO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    LISTA_CONVIDADO obj = _baseRepository.GetById(item.LICO_CD_ID);
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

        public Int32 Delete(LISTA_CONVIDADO item, LOG log)
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

        public List<LISTA_CONVIDADO> ExecuteFilter(String nome, DateTime? data, Int32? unid, Int32? reserva, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(nome, data, unid, reserva, idAss);
        }

        public CONVIDADO GetConvidadoById(Int32 id)
        {
            return _contRepository.GetItemById(id);
        }

        public Int32 EditConvidado(CONVIDADO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONVIDADO obj = _contRepository.GetById(item.CONV_CD_ID);
                    _contRepository.Detach(obj);
                    _contRepository.Update(item);
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

        public Int32 CreateConvidado(CONVIDADO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _contRepository.Add(item);
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
