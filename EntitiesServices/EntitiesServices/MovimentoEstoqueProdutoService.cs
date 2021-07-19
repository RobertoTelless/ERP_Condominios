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
    public class MovimentoEstoqueProdutoService : ServiceBase<MOVIMENTO_ESTOQUE_PRODUTO>, IMovimentoEstoqueProdutoService
    {
        private readonly IMovimentoEstoqueProdutoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public MovimentoEstoqueProdutoService(IMovimentoEstoqueProdutoRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
        }

        public MOVIMENTO_ESTOQUE_PRODUTO GetByProdId(Int32 prod, Int32 idAss)
        {
            MOVIMENTO_ESTOQUE_PRODUTO item = _baseRepository.GetByProdId(prod, idAss);
            return item;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItens(Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseRepository.GetAllItens(idAss);
            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseRepository.GetAllItensAdm(idAss);
            return lista;
        }

        public MOVIMENTO_ESTOQUE_PRODUTO GetItemById(Int32 id)
        {
            MOVIMENTO_ESTOQUE_PRODUTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensEntrada(Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseRepository.GetAllItensEntrada(idAss);
            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensSaida(Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseRepository.GetAllItensSaida(idAss);
            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilter(Int32? catId, Int32? subCatId, String nome, String barcode, Int32? idAss, DateTime? dtMov)
        {
            return _baseRepository.ExecuteFilter(catId, subCatId, nome, barcode, idAss, dtMov);
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilterAvulso(Int32? operacao, Int32? tipoMovimento, DateTime? dtInicial, DateTime? dtFinal, Int32? idAss, Int32? prod)
        {
            return _baseRepository.ExecuteFilterAvulso(operacao, tipoMovimento, dtInicial, dtFinal, idAss, prod);
        }

        public Int32 Create(MOVIMENTO_ESTOQUE_PRODUTO item, LOG log)
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

        public Int32 Create(MOVIMENTO_ESTOQUE_PRODUTO item)
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

        public Int32 Edit (MOVIMENTO_ESTOQUE_PRODUTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MOVIMENTO_ESTOQUE_PRODUTO obj = _baseRepository.GetById(item.MOEP_CD_ID);
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
    }
}
