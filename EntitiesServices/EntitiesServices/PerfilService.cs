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
    public class PerfilService : ServiceBase<PERFIL>, IPerfilService
    {
        private readonly IPerfilRepository _perfilRepository;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguracaoRepository _configuracaoRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public PerfilService(IPerfilRepository perfilRepository, ILogRepository logRepository, IConfiguracaoRepository configuracaoRepository) : base(perfilRepository)
        {
            _perfilRepository = perfilRepository;
            _logRepository = logRepository;
            _configuracaoRepository = configuracaoRepository;

        }

        public PERFIL GetByName(String nome)
        {
            PERFIL perfil = _perfilRepository.GetByName(nome);
            return perfil;
        }

        public List<PERFIL> GetAllItens()
        {
            return _perfilRepository.GetAllItens();
        }

        public Int32 Create(PERFIL perfil, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _perfilRepository.Add(perfil);
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

        public Int32 Create(PERFIL perfil)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _perfilRepository.Add(perfil);
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


        public Int32 Edit(PERFIL perfil, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PERFIL obj = _perfilRepository.GetById(perfil.PERF_CD_ID);
                    _perfilRepository.Detach(obj);
                    _logRepository.Add(log);
                    _perfilRepository.Update(perfil);
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

        public Int32 Edit(PERFIL perfil)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PERFIL obj = _perfilRepository.GetById(perfil.PERF_CD_ID);
                    _perfilRepository.Detach(obj);
                    _perfilRepository.Update(perfil);
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

        public Int32 Delete(PERFIL perfil, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _perfilRepository.Remove(perfil);
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

        public CONFIGURACAO CarregaConfiguracao()
        {
            CONFIGURACAO conf = _configuracaoRepository.GetById(1);
            return conf;
        }

        public USUARIO GetUserProfile(PERFIL perfil)
        {
            USUARIO usuario = _perfilRepository.GetUserProfile(perfil);
            return usuario;
        }
    }
}
