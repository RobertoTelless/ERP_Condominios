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
    public class UnidadeService : ServiceBase<UNIDADE>, IUnidadeService
    {
        private readonly IUnidadeRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IUnidadeAnexoRepository _anexoRepository;
        private readonly ITipoUnidadeRepository _tuRepository;
        private readonly ITorreRepository _toRepository;
        private readonly ICategoriaNotificacaoRepository _cnRepository;
        private readonly IUsuarioRepository _usuRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public UnidadeService(IUnidadeRepository baseRepository, ILogRepository logRepository, IUnidadeAnexoRepository anexoRepository, ITipoUnidadeRepository tuRepository, ITorreRepository toRepository, ICategoriaNotificacaoRepository cnRepository, IUsuarioRepository usuRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _anexoRepository = anexoRepository;
            _tuRepository = tuRepository;
            _toRepository = toRepository;
            _cnRepository = cnRepository;
            _usuRepository = usuRepository;
        }

        public UNIDADE CheckExist(UNIDADE tarefa, Int32 idAss)
        {
            UNIDADE item = _baseRepository.CheckExist(tarefa, idAss);
            return item;
        }

        public UNIDADE GetItemById(Int32 id)
        {
            UNIDADE item = _baseRepository.GetItemById(id);
            return item;
        }

        public TORRE GetTorreById(Int32 id)
        {
            TORRE item = _toRepository.GetItemById(id);
            return item;
        }

        public UNIDADE_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<UNIDADE> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<UNIDADE> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<TIPO_UNIDADE> GetAllTipos(Int32 idAss)
        {
            return _tuRepository.GetAllItens(idAss);
        }

        public List<TORRE> GetAllTorres(Int32 idAss)
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

        public Int32 Create(UNIDADE item, LOG log)
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

        public Int32 Create(UNIDADE item)
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


        public Int32 Edit(UNIDADE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    UNIDADE obj = _baseRepository.GetById(item.UNID_CD_ID);
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

        public Int32 Edit(UNIDADE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    UNIDADE obj = _baseRepository.GetById(item.UNID_CD_ID);
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

        public Int32 Delete(UNIDADE item, LOG log)
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

        public List<UNIDADE> ExecuteFilter(String numero, Int32? torre, Int32? idTipo, Int32? alugada, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(numero, torre, idTipo, alugada, idAss);

        }
    }
}
