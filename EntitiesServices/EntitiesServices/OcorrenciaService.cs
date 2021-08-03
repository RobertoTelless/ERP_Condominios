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
    public class OcorrenciaService : ServiceBase<OCORRENCIA>, IOcorrenciaService
    {
        private readonly IOcorrenciaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IOcorrenciaAnexoRepository _anexoRepository;
        private readonly IOcorrenciaComentarioRepository _comRepository;
        private readonly ICategoriaOcorrenciaRepository _catRepository;
        private readonly IUnidadeRepository _uniRepository;
        private readonly IUsuarioRepository _usuRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public OcorrenciaService(IOcorrenciaRepository baseRepository, ILogRepository logRepository, IOcorrenciaAnexoRepository anexoRepository, IOcorrenciaComentarioRepository comRepository, ICategoriaOcorrenciaRepository catRepository, IUnidadeRepository uniRepository, IUsuarioRepository usuRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _anexoRepository = anexoRepository;
            _comRepository = comRepository;
            _catRepository = catRepository;
            _uniRepository = uniRepository;
            _usuRepository = usuRepository;
        }

        public OCORRENCIA GetItemById(Int32 id)
        {
            OCORRENCIA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<OCORRENCIA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<OCORRENCIA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public OCORRENCIA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<OCORRENCIA> GetAllItensUser(Int32 id, Int32 idAss)
        {
            return _baseRepository.GetAllItensUser(id, idAss);
        }

        public List<OCORRENCIA> GetAllItensUnidade(Int32 id, Int32 idAss)
        {
            return _baseRepository.GetAllItensUnidade(id, idAss);
        }

        public List<CATEGORIA_OCORRENCIA> GetAllCategorias(Int32 idAss)
        {
            return _catRepository.GetAllItens(idAss);
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _uniRepository.GetAllItens(idAss);
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            return _usuRepository.GetAllItens(idAss);
        }

        public List<OCORRENCIA> GetOcorrenciasNovas(Int32 id, Int32 idAss)
        {
            return _baseRepository.GetOcorrenciasNovas(id, idAss);
        }

        public List<OCORRENCIA> ExecuteFilter(Int32? unidade, Int32? usuario, Int32? cat, String titulo, DateTime? data, String texto, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(unidade, usuario, cat, titulo, data, texto, idAss);

        }

        public Int32 Create(OCORRENCIA item, LOG log)
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

        public Int32 Create(OCORRENCIA item)
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


        public Int32 Edit(OCORRENCIA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    item.ASSINANTE = null;
                    item.CATEGORIA_OCORRENCIA = null;
                    item.UNIDADE = null;
                    OCORRENCIA obj = _baseRepository.GetById(item.OCOR_CD_ID);
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

        public Int32 Edit(OCORRENCIA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    item.ASSINANTE = null;
                    item.CATEGORIA_OCORRENCIA = null;
                    OCORRENCIA obj = _baseRepository.GetById(item.OCOR_CD_ID);
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

        public Int32 Delete(OCORRENCIA item, LOG log)
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
