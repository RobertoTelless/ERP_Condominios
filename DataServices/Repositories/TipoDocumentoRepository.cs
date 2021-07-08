using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TipoDocumentoRepository : RepositoryBase<TIPO_DOCUMENTO>, ITipoDocumentoRepository
    {
        public TIPO_DOCUMENTO GetItemById(Int32 id)
        {
            IQueryable<TIPO_DOCUMENTO> query = Db.TIPO_DOCUMENTO;
            query = query.Where(p => p.TIDO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_DOCUMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_DOCUMENTO> query = Db.TIPO_DOCUMENTO.Where(p => p.TIDO_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<TIPO_DOCUMENTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_DOCUMENTO> query = Db.TIPO_DOCUMENTO;
            return query.ToList();
        }
    }
}
