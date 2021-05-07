using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class DepartamentoRepository : RepositoryBase<DEPARTAMENTO>, IDepartamentoRepository
    {
        public DEPARTAMENTO GetItemById(Int32 id)
        {
            IQueryable<DEPARTAMENTO> query = Db.DEPARTAMENTO;
            query = query.Where(p => p.DEPT_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<DEPARTAMENTO> GetAllItensAdm()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<DEPARTAMENTO> query = Db.DEPARTAMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<DEPARTAMENTO> GetAllItens()
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<DEPARTAMENTO> query = Db.DEPARTAMENTO.Where(p => p.DEPT_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 