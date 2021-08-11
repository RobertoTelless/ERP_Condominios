using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEncomendaRepository : IRepositoryBase<ENCOMENDA>
    {
        ENCOMENDA CheckExist(ENCOMENDA item, Int32 idAss);
        ENCOMENDA GetItemById(Int32 id);
        List<ENCOMENDA> GetAllItens(Int32 idAss);
        List<ENCOMENDA> GetAllItensAdm(Int32 idAss);
        List<ENCOMENDA> GetByUnidade(Int32 idUnid);
        List<ENCOMENDA> ExecuteFilter(Int32? unid, Int32? forma, Int32? tipo, DateTime? data, Int32? status, Int32 idAss);
    }
}
