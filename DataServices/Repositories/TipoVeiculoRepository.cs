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
    public class TipoVeiculoRepository : RepositoryBase<TIPO_VEICULO>, ITipoVeiculoRepository
    {
        public TIPO_VEICULO GetItemById(Int32 id)
        {
            IQueryable<TIPO_VEICULO> query = Db.TIPO_VEICULO;
            query = query.Where(p => p.TIVE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_VEICULO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_VEICULO> query = Db.TIPO_VEICULO;
            return query.ToList();
        }

        public List<TIPO_VEICULO> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_VEICULO> query = Db.TIPO_VEICULO.Where(p => p.TIVE_IN_ATIVO == 1);
            return query.ToList();
        }
    }
}
 