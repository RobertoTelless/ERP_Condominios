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
    public class FuncionarioService : ServiceBase<FUNCIONARIO>, IFuncionarioService
    {
        private readonly IFuncionarioRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ISituacaoRepository _sitRepository;
        private readonly IFuncionarioAnexoRepository _anexoRepository;
        private readonly ISexoRepository _sexodRepository;
        private readonly IFuncaoRepository _funRepository;
        private readonly IEscolaridadeRepository _escoRepository;
        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public FuncionarioService(IFuncionarioRepository baseRepository, ILogRepository logRepository, ISituacaoRepository sitRepository, IFuncionarioAnexoRepository anexoRepository, ISexoRepository sexoRepository, IFuncaoRepository funRepository, IEscolaridadeRepository escoRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _sitRepository = sitRepository;
            _anexoRepository = anexoRepository;
            _sexodRepository = sexoRepository;
            _funRepository = funRepository;
            _escoRepository = escoRepository;
        }

        public FUNCIONARIO CheckExist(FUNCIONARIO conta)
        {
            FUNCIONARIO item = _baseRepository.CheckExist(conta);
            return item;
        }

        public FUNCIONARIO GetItemById(Int32 id)
        {
            FUNCIONARIO item = _baseRepository.GetItemById(id);
            return item;
        }

        public FUNCIONARIO GetByNome(String nome)
        {
            FUNCIONARIO item = _baseRepository.GetByNome(nome);
            return item;
        }

        public List<FUNCIONARIO> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<FUNCIONARIO> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<SITUACAO> GetAllSituacao()
        {
            return _sitRepository.GetAllItens();
        }

        public List<SEXO> GetAllSexo()
        {
            return _sexodRepository.GetAllItens();
        }

        public FUNCIONARIO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<FUNCAO> GetAllFuncao()
        {
            return _funRepository.GetAllItens();
        }

        public List<ESCOLARIDADE> GetAllEscolaridade()
        {
            return _escoRepository.GetAllItens();        }


        public List<FUNCIONARIO> ExecuteFilter(Int32? sitId, String nome, String cpf, String rg, Int32? funId)
        {
            return _baseRepository.ExecuteFilter(sitId, nome, cpf, rg, funId);

        }

        public Int32 Create(FUNCIONARIO item, LOG log)
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

        public Int32 Create(FUNCIONARIO item)
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


        public Int32 Edit(FUNCIONARIO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FUNCIONARIO obj = _baseRepository.GetById(item.FUNC_CD_ID);
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

        public Int32 Edit(FUNCIONARIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FUNCIONARIO obj = _baseRepository.GetById(item.FUNC_CD_ID);
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

        public Int32 Delete(FUNCIONARIO item, LOG log)
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
