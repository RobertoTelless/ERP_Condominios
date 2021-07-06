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
    public class AmbienteImagemRepository : RepositoryBase<AMBIENTE_IMAGEM>, IAmbienteImagemRepository
    {
        public List<AMBIENTE_IMAGEM> GetAllItens()
        {
            return Db.AMBIENTE_IMAGEM.ToList();
        }

        public AMBIENTE_IMAGEM GetItemById(Int32 id)
        {
            IQueryable<AMBIENTE_IMAGEM> query = Db.AMBIENTE_IMAGEM.Where(p => p.AMIM_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 