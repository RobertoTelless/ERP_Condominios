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
    public class ContaBancariaService : ServiceBase<CONTA_BANCO>, IContaBancariaService
    {
        private readonly IContaBancariaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoContaRepository _tipoRepository;
        private readonly IContaBancariaContatoRepository _contRepository;
        private readonly IContaBancariaLancamentoRepository _lancRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public ContaBancariaService(IContaBancariaRepository baseRepository, ILogRepository logRepository, ITipoContaRepository tipoRepository, IContaBancariaContatoRepository contRepository, IContaBancariaLancamentoRepository lancRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _contRepository = contRepository;
            _lancRepository = lancRepository;
        }

        public CONTA_BANCO CheckExist(CONTA_BANCO conta, Int32 idAss)
        {
            CONTA_BANCO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public CONTA_BANCO GetItemById(Int32 id)
        {
            CONTA_BANCO item = _baseRepository.GetItemById(id);
            return item;
        }

        public CONTA_BANCO GetContaPadrao(Int32 idAss)
        {
            CONTA_BANCO item = _baseRepository.GetContaPadrao(idAss);
            return item;
        }

        public List<CONTA_BANCO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CONTA_BANCO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public CONTA_BANCO_CONTATO GetContatoById(Int32 id)
        {
            return _contRepository.GetItemById(id);
        }

        public CONTA_BANCO_LANCAMENTO GetLancamentoById(Int32 id)
        {
            return _lancRepository.GetItemById(id);
        }

        public Decimal GetTotalContas(Int32 idAss)
        {
            return _baseRepository.GetTotalContas(idAss);
        }

        public List<TIPO_CONTA> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public Decimal GetTotalReceita(Int32 conta)
        {

            return _lancRepository.GetTotalReceita(conta);
        }

        public Decimal GetTotalDespesa(Int32 conta)
        {

            return _lancRepository.GetTotalDespesa(conta);
        }

        public Decimal GetTotalReceitaMes(Int32 conta, Int32 mes)
        {

            return _lancRepository.GetTotalReceitaMes(conta, mes);
        }

        public Decimal GetTotalDespesaMes(Int32 conta, Int32 mes)
        {

            return _lancRepository.GetTotalDespesaMes(conta, mes);
        }

        public List<CONTA_BANCO_LANCAMENTO> GetLancamentosMes(Int32 conta, Int32 mes)
        {

            return _lancRepository.GetLancamentosMes(conta, mes);
        }

        public List<CONTA_BANCO_LANCAMENTO> GetLancamentosDia(Int32 conta, DateTime data)
        {

            return _lancRepository.GetLancamentosDia(conta, data);
        }

        public List<CONTA_BANCO_LANCAMENTO> GetLancamentosFaixa(Int32 conta, DateTime inicio, DateTime final)
        {

            return _lancRepository.GetLancamentosFaixa(conta, inicio, final);
        }

        public List<CONTA_BANCO_LANCAMENTO> ExecuteFilterLanc(Int32 conta, DateTime? data, Int32? tipo, String desc)
        {
            return _lancRepository.ExecuteFilter(conta, data, tipo, desc);
        }

        public Int32 Create(CONTA_BANCO item, LOG log)
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

        public Int32 Create(CONTA_BANCO item)
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


        public Int32 Edit(CONTA_BANCO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONTA_BANCO obj = _baseRepository.GetById(item.COBA_CD_ID);
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

        public Int32 Edit(CONTA_BANCO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.TIPO_CONTA = null;
                    CONTA_BANCO obj = _baseRepository.GetById(item.COBA_CD_ID);
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

        public Int32 Delete(CONTA_BANCO item, LOG log)
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

        public Int32 EditContato(CONTA_BANCO_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONTA_BANCO_CONTATO obj = _contRepository.GetById(item.CBCT_CD_ID);
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

        public Int32 CreateContato(CONTA_BANCO_CONTATO item)
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

        public Int32 EditLancamento(CONTA_BANCO_LANCAMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONTA_BANCO_LANCAMENTO obj = _lancRepository.GetById(item.CBLA_CD_ID);
                    _lancRepository.Detach(obj);
                    _lancRepository.Update(item);
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

        public Int32 CreateLancamento(CONTA_BANCO_LANCAMENTO item, CONTA_BANCO conta)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _lancRepository.Add(item);
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
