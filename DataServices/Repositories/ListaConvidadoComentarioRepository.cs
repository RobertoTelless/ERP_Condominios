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
    public class ListaConvidadoComentarioRepository : RepositoryBase<LISTA_CONVIDADO_COMENTARIO>, IListaConvidadoComentarioRepository
    {
        public List<LISTA_CONVIDADO_COMENTARIO> GetAllItens()
        {
            return Db.LISTA_CONVIDADO_COMENTARIO.ToList();
        }

        public LISTA_CONVIDADO_COMENTARIO GetItemById(Int32 id)
        {
            IQueryable<LISTA_CONVIDADO_COMENTARIO> query = Db.LISTA_CONVIDADO_COMENTARIO.Where(p => p.LCCM_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 