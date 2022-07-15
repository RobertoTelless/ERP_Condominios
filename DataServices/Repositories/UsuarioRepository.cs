using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class UsuarioRepository : RepositoryBase<USUARIO>, IUsuarioRepository
    {
        public USUARIO GetByEmail(String email, Int32 idAss)
        {
            IQueryable<USUARIO> query = Db.USUARIO.Where(p => p.USUA_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_NM_EMAIL == email);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public USUARIO GetByEmailOnly(String email)
        {
            IQueryable<USUARIO> query = Db.USUARIO.Where(p => p.USUA_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_NM_EMAIL == email);
            return query.FirstOrDefault();
        }

        public USUARIO GetByLogin(String login, Int32 idAss)
        {
            IQueryable<USUARIO> query = Db.USUARIO.Where(p => p.USUA_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_NM_LOGIN == login);
            return query.FirstOrDefault();
        }

        public USUARIO GetItemById(Int32 id)
        {
            IQueryable<USUARIO> query = Db.USUARIO.Where(p => p.USUA_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_CD_ID == id);
            query = query.Include(p => p.PERFIL);
            query = query.Include(p => p.NOTIFICACAO);
            query = query.Include(p => p.TAREFA);
            query = query.Include(p => p.AGENDA);
            query = query.Include(p => p.LOG);
            query = query.Include(p => p.VEICULO);
            return query.FirstOrDefault();
        }

        public USUARIO GetAdministrador(Int32 idAss)
        {
            IQueryable<USUARIO> query = Db.USUARIO.Where(p => p.USUA_IN_ATIVO == 1);
            query = query.Where(p => p.PERFIL.PERF_SG_SIGLA == "ADM");
            query = query.Include(p => p.ASSINANTE);
            query = query.Include(p => p.PERFIL);
            return query.FirstOrDefault();
        }

        public List<USUARIO> GetAllItens(Int32 idAss)
        {
            IQueryable<USUARIO> query = Db.USUARIO.Where(p => p.USUA_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_IN_BLOQUEADO == 0);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<USUARIO> GetAllItensBloqueados(Int32 idAss)
        {
            IQueryable<USUARIO> query = Db.USUARIO.Where(p => p.USUA_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_IN_BLOQUEADO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<USUARIO> GetAllItensAcessoHoje(Int32 idAss)
        {
            IQueryable<USUARIO> query = Db.USUARIO.Where(p => p.USUA_IN_ATIVO == 1);
            query = query.Where(p => p.USUA_IN_BLOQUEADO == 0);
            query = query.Where(p => DbFunctions.TruncateTime(p.USUA_DT_ACESSO) == DbFunctions.TruncateTime(DateTime.Today.Date));
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<USUARIO> GetAllUsuariosAdm(Int32 idAss)
        {
            IQueryable<USUARIO> query = Db.USUARIO.Where(p => p.USUA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<USUARIO> GetAllUsuarios(Int32 idAss)
        {
            IQueryable<USUARIO> query = Db.USUARIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<USUARIO> ExecuteFilter(Int32? perfilId, Int32? cargoId, String nome, String login, String email, Int32 idAss)
        {
            List<USUARIO> lista = new List<USUARIO>();
            IQueryable<USUARIO> query = Db.USUARIO;
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.USUA_NM_EMAIL == email);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.USUA_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(login))
            {
                query = query.Where(p => p.USUA_NM_LOGIN == login);
            }
            if (perfilId != 0)
            {
                query = query.Where(p => p.PERFIL.PERF_CD_ID == perfilId);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.USUA_NM_NOME);
                query = query.Include(p => p.PERFIL);
                lista = query.ToList<USUARIO>();
            }
            return lista;
        }

        public List<USUARIO> ExecuteFilterMorador(String nome, Int32? idUnid, Int32 idAss)
        {
            List<USUARIO> lista = new List<USUARIO>();
            IQueryable<USUARIO> query = Db.USUARIO;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.USUA_NM_NOME.Contains(nome));
            }
            if (idUnid != null)
            {
                query = query.Where(p => p.UNID_CD_ID == idUnid);
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.Where(p => p.USUA_IN_MORADOR == 1);
                query = query.OrderBy(a => a.USUA_NM_NOME);
                lista = query.ToList<USUARIO>();
            }
            return lista;
        }

    }
}
