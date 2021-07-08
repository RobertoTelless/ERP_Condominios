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
    public class AutorizacaoService : ServiceBase<AUTORIZACAO_ACESSO>, IAutorizacaoService
    {
        private readonly IAutorizacaoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoDocumentoRepository _tipoRepository;
        private readonly IAutorizacaoAnexoRepository _anexoRepository;
        private readonly IUnidadeRepository _uniRepository;
        private readonly IUsuarioRepository _usuRepository;
        private readonly IGrauParentescoRepository _graRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public AutorizacaoService(IAutorizacaoRepository baseRepository, ILogRepository logRepository, ITipoDocumentoRepository tipoRepository, IAutorizacaoAnexoRepository anexoRepository, IUnidadeRepository uniRepository, IUsuarioRepository usuRepository, IGrauParentescoRepository graRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _anexoRepository = anexoRepository;
            _uniRepository = uniRepository;
            _usuRepository = usuRepository;
            _graRepository = graRepository;
        }

        public AUTORIZACAO_ACESSO CheckExist(AUTORIZACAO_ACESSO conta, Int32 idAss)
        {
            AUTORIZACAO_ACESSO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public AUTORIZACAO_ACESSO GetItemById(Int32 id)
        {
            AUTORIZACAO_ACESSO item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<AUTORIZACAO_ACESSO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<AUTORIZACAO_ACESSO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<TIPO_DOCUMENTO> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public List<GRAU_PARENTESCO> GetAllGraus(Int32 idAss)
        {
            return _graRepository.GetAllItens(idAss);
        }

        public AUTORIZACAO_ACESSO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<AUTORIZACAO_ACESSO> ExecuteFilter(Int32? unid, String nome, String documento, String empresa, Int32? tipo, DateTime? data, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(unid, nome, documento, empresa, tipo, data, idAss);

        }

        public Int32 Create(AUTORIZACAO_ACESSO item, LOG log)
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

        public Int32 Create(AUTORIZACAO_ACESSO item)
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


        public Int32 Edit(AUTORIZACAO_ACESSO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AUTORIZACAO_ACESSO obj = _baseRepository.GetById(item.AUAC_CD_ID);
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

        public Int32 Edit(AUTORIZACAO_ACESSO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    AUTORIZACAO_ACESSO obj = _baseRepository.GetById(item.AUAC_CD_ID);
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

        public Int32 Delete(AUTORIZACAO_ACESSO item, LOG log)
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

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _uniRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }
    }
}
