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
    public class FornecedorService : ServiceBase<FORNECEDOR>, IFornecedorService
    {
        private readonly IFornecedorRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICategoriaFornecedorRepository _tipoRepository;
        private readonly IFornecedorAnexoRepository _anexoRepository;
        private readonly ITipoPessoaRepository _pesRepository;
        private readonly IFornecedorContatoRepository _contRepository;
        private readonly IUFRepository _ufRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public FornecedorService(IFornecedorRepository baseRepository, ILogRepository logRepository, ICategoriaFornecedorRepository tipoRepository, IFornecedorAnexoRepository anexoRepository, ITipoPessoaRepository pesRepository, IFornecedorContatoRepository contRepository, IUFRepository ufRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _pesRepository = pesRepository;
            _contRepository = contRepository;
            _ufRepository = ufRepository;
        }

        public FORNECEDOR CheckExist(FORNECEDOR conta, Int32 idAss)
        {
            FORNECEDOR item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public FORNECEDOR GetItemById(Int32 id)
        {
            FORNECEDOR item = _baseRepository.GetItemById(id);
            return item;
        }
        public UF GetUFbySigla(String sigla)
        {
            return _ufRepository.GetItemBySigla(sigla);
        }

        public FORNECEDOR GetByEmail(String email)
        {
            FORNECEDOR item = _baseRepository.GetByEmail(email);
            return item;
        }

        public List<FORNECEDOR> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<UF> GetAllUF()
        {
            return _ufRepository.GetAllItens();
        }

        public List<FORNECEDOR> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<CATEGORIA_FORNECEDOR> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public FORNECEDOR_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<FORNECEDOR> ExecuteFilter(Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, String rede, Int32? ativo, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(catId, razao, nome, cpf, cnpj, email, cidade, uf, rede, ativo, idAss);

        }

        public List<FORNECEDOR> ExecuteFilterSemPedido(String nome, String cidade, Int32? uf, Int32 idAss)
        {
            return _baseRepository.ExecuteFilterSemPedido(nome, cidade, uf, idAss);
        }

        public Int32 Create(FORNECEDOR item, LOG log)
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

        public Int32 Create(FORNECEDOR item)
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


        public Int32 Edit(FORNECEDOR item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FORNECEDOR obj = _baseRepository.GetById(item.FORN_CD_ID);
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

        public Int32 Edit(FORNECEDOR item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FORNECEDOR obj = _baseRepository.GetById(item.FORN_CD_ID);
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

        public Int32 Delete(FORNECEDOR item, LOG log)
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

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            return _pesRepository.GetAllItens();
        }

        public FORNECEDOR_CONTATO GetContatoById(Int32 id)
        {
            return _contRepository.GetItemById(id);
        }

        public Int32 EditContato(FORNECEDOR_CONTATO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    FORNECEDOR_CONTATO obj = _contRepository.GetById(item.FOCO_CD_ID);
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

        public Int32 CreateContato(FORNECEDOR_CONTATO item)
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
