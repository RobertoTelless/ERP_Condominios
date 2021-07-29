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
    public class AssinanteService : ServiceBase<ASSINANTE>, IAssinanteService
    {
        private readonly IAssinanteRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoPessoaRepository _tpRepository;
        private readonly IUFRepository _ufRepository;
        private readonly ITipoCondominioRepository _tcRepository;
        private readonly ITipoUnidadeRepository _tuRepository;
        private readonly ITipoVagaRepository _tvRepository;
        private readonly IAssinanteAnexoRepository _anexoRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public AssinanteService(IAssinanteRepository baseRepository, ILogRepository logRepository, ITipoPessoaRepository tpRepository, IUFRepository ufRepository, ITipoCondominioRepository tcRepository, ITipoUnidadeRepository tuRepository, ITipoVagaRepository tvRepository, IAssinanteAnexoRepository anexoRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tpRepository = tpRepository;
            _ufRepository = ufRepository;
            _tcRepository = tcRepository;
            _tuRepository = tuRepository;
            _tvRepository = tvRepository;
            _anexoRepository = anexoRepository;
        }

        public ASSINANTE CheckExist(ASSINANTE conta)
        {
            ASSINANTE item = _baseRepository.CheckExist(conta);
            return item;
        }

        public ASSINANTE GetItemById(Int32 id)
        {
            ASSINANTE item = _baseRepository.GetItemById(id);
            return item;
        }

        public UF GetUFBySigla(String sigla)
        {
            UF item = _ufRepository.GetItemBySigla(sigla);
            return item;
        }

        public List<ASSINANTE> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<ASSINANTE> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            return _tpRepository.GetAllItens();
        }

        public List<TIPO_CONDOMINIO> GetAllTiposCondominio()
        {
            return _tcRepository.GetAllItens();
        }

        public List<TIPO_UNIDADE> GetAllTiposUnidade(Int32 idAss)
        {
            return _tuRepository.GetAllItens(idAss);
        }

        public List<TIPO_VAGA> GetAllTiposVaga(Int32 idAss)
        {
            return _tvRepository.GetAllItens(idAss);
        }

        public List<UF> GetAllUF()
        {
            return _ufRepository.GetAllItens();
        }

        public ASSINANTE_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<ASSINANTE> ExecuteFilter(Int32 tipo, String nome)
        {
            List<ASSINANTE> lista = _baseRepository.ExecuteFilter(tipo, nome);
            return lista;
        }

        public Int32 Create(ASSINANTE item, LOG log)
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

        public Int32 Create(ASSINANTE item)
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


        public Int32 Edit(ASSINANTE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ASSINANTE obj = _baseRepository.GetById(item.ASSI_CD_ID);
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

        public Int32    Edit(ASSINANTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ASSINANTE obj = _baseRepository.GetById(item.ASSI_CD_ID);
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

        public Int32 Delete(ASSINANTE item, LOG log)
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
