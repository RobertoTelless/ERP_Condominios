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
    public class EncomendaService : ServiceBase<ENCOMENDA>, IEncomendaService
    {
        private readonly IEncomendaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IFormaEntregaRepository _feRepository;
        private readonly IUnidadeRepository _toRepository;
        private readonly ITipoEncomendaRepository _teRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IEncomendaComentarioRepository _comRepository;
        private readonly IEncomendaAnexoRepository _anexoRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public EncomendaService(IEncomendaRepository baseRepository, ILogRepository logRepository, IFormaEntregaRepository feRepository, IUnidadeRepository toRepository, ITipoEncomendaRepository teRepository, IUsuarioRepository usuRepository, IEncomendaComentarioRepository comRepository, IEncomendaAnexoRepository anexoRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _feRepository = feRepository;
            _toRepository = toRepository;
            _teRepository = teRepository;
            _usuRepository = usuRepository;
            _comRepository = comRepository;
            _anexoRepository = anexoRepository;
        }

        public ENCOMENDA GetItemById(Int32 id)
        {
            ENCOMENDA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<ENCOMENDA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<ENCOMENDA> GetByUnidade(Int32 idUnid)
        {
            return _baseRepository.GetByUnidade(idUnid);
        }

        public List<ENCOMENDA> GetByData(DateTime data, Int32 idAss)
        {
            return _baseRepository.GetByData(data, idAss);
        }

        public List<ENCOMENDA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<FORMA_ENTREGA> GetAllFormas(Int32 idAss)
        {
            return _feRepository.GetAllItens(idAss);
        }

        public List<TIPO_ENCOMENDA> GetAllTipos(Int32 idAss)
        {
            return _teRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _toRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public ENCOMENDA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public ENCOMENDA_COMENTARIO GetComentarioById(Int32 id)
        {
            return _comRepository.GetItemById(id);
        }

        public Int32 Create(ENCOMENDA item, LOG log)
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

        public Int32 Create(ENCOMENDA item)
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


        public Int32 Edit(ENCOMENDA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.UNIDADE = null;
                    ENCOMENDA obj = _baseRepository.GetById(item.ENCO_CD_ID);
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

        public Int32 Edit(ENCOMENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ENCOMENDA obj = _baseRepository.GetById(item.ENCO_CD_ID);
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

        public Int32 Delete(ENCOMENDA item, LOG log)
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

        public List<ENCOMENDA> ExecuteFilter(Int32? unid, Int32? forma, Int32? tipo, DateTime? data, Int32? status, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(unid, forma, tipo, data, status, idAss);
        }
    }
}
