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
    public class ControleVeiculoService : ServiceBase<CONTROLE_VEICULO>, IControleVeiculoService
    {
        private readonly IControleVeiculoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoVeiculoRepository _tuRepository;
        private readonly IUnidadeRepository _toRepository;
        private readonly ICategoriaNotificacaoRepository _cnRepository;
        private readonly IUsuarioRepository _usuRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public ControleVeiculoService(IControleVeiculoRepository baseRepository, ILogRepository logRepository, ITipoVeiculoRepository tuRepository, IUnidadeRepository toRepository, ICategoriaNotificacaoRepository cnRepository, IUsuarioRepository usuRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tuRepository = tuRepository;
            _toRepository = toRepository;
            _cnRepository = cnRepository;
            _usuRepository = usuRepository;
        }

        public CONTROLE_VEICULO GetItemById(Int32 id)
        {
            CONTROLE_VEICULO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<CONTROLE_VEICULO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CONTROLE_VEICULO> GetByUnidade(Int32 idUnid)
        {
            return _baseRepository.GetByUnidade(idUnid);
        }

        public List<CONTROLE_VEICULO> GetByData(DateTime data, Int32 idAss)
        {
            return _baseRepository.GetByData(data, idAss);
        }

        public List<CONTROLE_VEICULO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<TIPO_VEICULO> GetAllTipos(Int32 idAss)
        {
            return _tuRepository.GetAllItens(idAss);
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

        public Int32 Create(CONTROLE_VEICULO item, LOG log)
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

        public Int32 Create(CONTROLE_VEICULO item)
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


        public Int32 Edit(CONTROLE_VEICULO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.UNIDADE = null;
                    CONTROLE_VEICULO obj = _baseRepository.GetById(item.COVE_CD_ID);
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

        public Int32 Edit(CONTROLE_VEICULO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONTROLE_VEICULO obj = _baseRepository.GetById(item.COVE_CD_ID);
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

        public Int32 Delete(CONTROLE_VEICULO item, LOG log)
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

        public List<CONTROLE_VEICULO> ExecuteFilter(String placa, String marca, Int32? unid, Int32? idTipo, DateTime? dataEntrada, DateTime? dataSaida, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(placa, marca, unid, idTipo, dataEntrada, dataSaida, idAss);
        }
    }
}
