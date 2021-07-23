using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ListaConvidadoAnexoRepository : RepositoryBase<LISTA_CONVIDADO_ANEXO>, IListaConvidadoAnexoRepository
    {
        public List<LISTA_CONVIDADO_ANEXO> GetAllItens()
        {
            return Db.LISTA_CONVIDADO_ANEXO.ToList();
        }

        public LISTA_CONVIDADO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<LISTA_CONVIDADO_ANEXO> query = Db.LISTA_CONVIDADO_ANEXO.Where(p => p.LCAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
