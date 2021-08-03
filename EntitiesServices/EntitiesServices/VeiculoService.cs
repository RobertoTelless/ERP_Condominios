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
    public class VeiculoService : ServiceBase<VEICULO>, IVeiculoService
    {
        private readonly IVeiculoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IVeiculoAnexoRepository _anexoRepository;
        private readonly ITipoVeiculoRepository _tuRepository;
        private readonly IUnidadeRepository _toRepository;
        private readonly IVagaRepository _vaRepository;
        private readonly ICategoriaNotificacaoRepository _cnRepository;
        private readonly IUsuarioRepository _usuRepository;
        protected ERP_CondominioEntities Db = new ERP_CondominioEntities();

        public VeiculoService(IVeiculoRepository baseRepository, ILogRepository logRepository, IVeiculoAnexoRepository anexoRepository, ITipoVeiculoRepository tuRepository, IUnidadeRepository toRepository, IVagaRepository vaRepository, ICategoriaNotificacaoRepository cnRepository, IUsuarioRepository usuRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _anexoRepository = anexoRepository;
            _tuRepository = tuRepository;
            _toRepository = toRepository;
            _cnRepository = cnRepository;
            _usuRepository = usuRepository;
            _vaRepository = vaRepository;
        }

        public VEICULO CheckExist(VEICULO tarefa, Int32 idAss)
        {
            VEICULO item = _baseRepository.CheckExist(tarefa, idAss);
            return item;
        }

        public VEICULO GetItemById(Int32 id)
        {
            VEICULO item = _baseRepository.GetItemById(id);
            return item;
        }

        public VEICULO_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<VEICULO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<VEICULO> GetByUnidade(Int32 idUnid)
        {
            return _baseRepository.GetByUnidade(idUnid);
        }

        public List<VEICULO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<TIPO_VEICULO> GetAllTipos(Int32 idAss)
        {
            return _tuRepository.GetAllItens(idAss);
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

        public List<VAGA> GetAllVagas(Int32 idAss)
        {
            return _vaRepository.GetAllItens(idAss);
        }

        public Int32 Create(VEICULO item, LOG log)
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

        public Int32 Create(VEICULO item)
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


        public Int32 Edit(VEICULO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.UNIDADE = null;
                    VEICULO obj = _baseRepository.GetById(item.VEIC_CD_ID);
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

        public Int32 Edit(VEICULO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    VEICULO obj = _baseRepository.GetById(item.VEIC_CD_ID);
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

        public Int32 Delete(VEICULO item, LOG log)
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

        public List<VEICULO> ExecuteFilter(String placa, String marca, Int32? unid, Int32? idTipo, Int32? vaga, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(placa, marca, unid, idTipo, vaga, idAss);
        }
    }
}
