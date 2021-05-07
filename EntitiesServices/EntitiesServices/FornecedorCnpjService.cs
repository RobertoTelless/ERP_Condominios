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
    public class FornecedorCnpjService : ServiceBase<FORNECEDOR_QUADRO_SOCIETARIO>, IFornecedorCnpjService
    {
        private readonly IFornecedorCnpjRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public FornecedorCnpjService(IFornecedorCnpjRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
        }

        public FORNECEDOR_QUADRO_SOCIETARIO CheckExist(FORNECEDOR_QUADRO_SOCIETARIO fqs)
        {
            FORNECEDOR_QUADRO_SOCIETARIO item = _baseRepository.CheckExist(fqs);
            return item;
        }

        public List<FORNECEDOR_QUADRO_SOCIETARIO> GetAllItens()
        {
            List<FORNECEDOR_QUADRO_SOCIETARIO> lista = _baseRepository.GetAllItens();
            return lista;
        }

        public List<FORNECEDOR_QUADRO_SOCIETARIO> GetByFornecedor(FORNECEDOR fornecedor)
        {
            List<FORNECEDOR_QUADRO_SOCIETARIO> lista = _baseRepository.GetByFornecedor(fornecedor);
            return lista;
        }

        public Int32 Create(FORNECEDOR_QUADRO_SOCIETARIO fqs, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(fqs);
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

        public Int32 Create(FORNECEDOR_QUADRO_SOCIETARIO fqs)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(fqs);
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