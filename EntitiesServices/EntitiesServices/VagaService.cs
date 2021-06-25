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
    public class VagaService : ServiceBase<VAGA>, IVagaService
    {
        private readonly IVagaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoVagaRepository _tvRepository;
        private readonly IUnidadeRepository _unRepository;
        private readonly ICategoriaNotificacaoRepository _cnRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IVeiculoRepository _veRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public VagaService(IVagaRepository baseRepository, ILogRepository logRepository, ITipoVagaRepository tvRepository, IUnidadeRepository unRepository, ICategoriaNotificacaoRepository cnRepository, IUsuarioRepository usuRepository, IVeiculoRepository veRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tvRepository = tvRepository;
            _unRepository = unRepository;
            _cnRepository = cnRepository;
            _usuRepository = usuRepository;
            _veRepository = veRepository;
        }

        public VAGA CheckExist(VAGA tarefa, Int32 idAss)
        {
            VAGA item = _baseRepository.CheckExist(tarefa, idAss);
            return item;
        }

        public VAGA GetItemById(Int32 id)
        {
            VAGA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<VAGA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<VAGA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<TIPO_VAGA> GetAllTipos(Int32 idAss)
        {
            return _tvRepository.GetAllItens(idAss);
        }

        public List<VEICULO> GetVeiculosUnidade(Int32 idUnid)
        {
            return _veRepository.GetByUnidade(idUnid);
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _unRepository.GetAllItens(idAss);
        }

        public List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss)
        {
            return _cnRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public Int32 Create(VAGA item, LOG log)
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

        public Int32 Create(VAGA item)
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


        public Int32 Edit(VAGA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    VAGA obj = _baseRepository.GetById(item.VAGA_CD_ID);
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

        public Int32 Edit(VAGA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    VAGA obj = _baseRepository.GetById(item.VAGA_CD_ID);
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

        public Int32 Delete(VAGA item, LOG log)
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

        public List<VAGA> ExecuteFilter(String numero, String andar, Int32? unid, Int32? idTipo, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(numero, andar, unid, idTipo, idAss);

        }
    }
}
