using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class RamoAtividadeRepository : RepositoryBase<RAMO_ATIVIDADE>, IRamoAtividadeRepository
    {
        public RAMO_ATIVIDADE GetItemById(Int32 id)
        {
            IQueryable<RAMO_ATIVIDADE> query = Db.RAMO_ATIVIDADE;
            query = query.Where(p => p.RAAT_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<RAMO_ATIVIDADE> GetAllItens()
        {
            IQueryable<RAMO_ATIVIDADE> query = Db.RAMO_ATIVIDADE.Where(p => p.RAAT_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<RAMO_ATIVIDADE> GetAllItensAdm()
        {
            IQueryable<RAMO_ATIVIDADE> query = Db.RAMO_ATIVIDADE;
            return query.ToList();
        }
    }
}
