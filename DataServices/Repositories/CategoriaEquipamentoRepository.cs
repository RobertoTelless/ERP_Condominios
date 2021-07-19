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
    public class CategoriaEquipamentoRepository : RepositoryBase<CATEGORIA_EQUIPAMENTO>, ICategoriaEquipamentoRepository
    {
        public CATEGORIA_EQUIPAMENTO GetItemById(Int32 id)
        {
            IQueryable<CATEGORIA_EQUIPAMENTO> query = Db.CATEGORIA_EQUIPAMENTO;
            query = query.Where(p => p.CAEQ_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CATEGORIA_EQUIPAMENTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CATEGORIA_EQUIPAMENTO> query = Db.CATEGORIA_EQUIPAMENTO;
            return query.ToList();
        }

        public List<CATEGORIA_EQUIPAMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<CATEGORIA_EQUIPAMENTO> query = Db.CATEGORIA_EQUIPAMENTO.Where(p => p.CAEQ_IN_ATIVO == 1);
            return query.ToList();
        }

    }
}
 