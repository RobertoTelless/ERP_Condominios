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
    public class EntradaSaidaService : ServiceBase<ENTRADA_SAIDA>, IEntradaSaidaService
    {
        private readonly IEntradaSaidaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IGrauParentescoRepository _gpRepository;
        private readonly IUnidadeRepository _toRepository;
        private readonly ICategoriaNotificacaoRepository _cnRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IAutorizacaoRepository _auRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public EntradaSaidaService(IEntradaSaidaRepository baseRepository, ILogRepository logRepository, IGrauParentescoRepository gpRepository, IUnidadeRepository toRepository, ICategoriaNotificacaoRepository cnRepository, IUsuarioRepository usuRepository, IAutorizacaoRepository auRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _gpRepository = gpRepository;
            _toRepository = toRepository;
            _cnRepository = cnRepository;
            _usuRepository = usuRepository;
            _auRepository = auRepository;
        }

        public ENTRADA_SAIDA GetItemById(Int32 id)
        {
            ENTRADA_SAIDA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<ENTRADA_SAIDA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<ENTRADA_SAIDA> GetByUnidade(Int32 idUnid)
        {
            return _baseRepository.GetByUnidade(idUnid);
        }

        public List<ENTRADA_SAIDA> GetByData(DateTime data, Int32 idAss)
        {
            return _baseRepository.GetByData(data, idAss);
        }

        public List<ENTRADA_SAIDA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<GRAU_PARENTESCO> GetAllGraus(Int32 idAss)
        {
            return _gpRepository.GetAllItens(idAss);
        }

        public List<AUTORIZACAO_ACESSO> GetAllAutorizacoes(Int32 idAss)
        {
            return _auRepository.GetAllItens(idAss);
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

        public Int32 Create(ENTRADA_SAIDA item, LOG log)
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

        public Int32 Create(ENTRADA_SAIDA item)
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


        public Int32 Edit(ENTRADA_SAIDA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.UNIDADE = null;
                    ENTRADA_SAIDA obj = _baseRepository.GetById(item.ENSA_CD_ID);
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

        public Int32 Edit(ENTRADA_SAIDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ENTRADA_SAIDA obj = _baseRepository.GetById(item.ENSA_CD_ID);
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

        public Int32 Delete(ENTRADA_SAIDA item, LOG log)
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

        public List<ENTRADA_SAIDA> ExecuteFilter(String nome, String documento, Int32? unid, Int32? autorizacao, DateTime? dataEntrada, DateTime? dataSaida, Int32? status, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(nome, documento, unid, autorizacao, dataEntrada, dataSaida, status, idAss);
        }
    }
}
