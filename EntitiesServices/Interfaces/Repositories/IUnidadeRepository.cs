using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IUnidadeRepository : IRepositoryBase<UNIDADE>
    {
        UNIDADE CheckExist(UNIDADE item, Int32 idAss);
        UNIDADE GetItemById(Int32 id);
        List<UNIDADE> GetAllItens(Int32 idAss);
        List<UNIDADE> GetAllItensAdm(Int32 idAss);
        List<UNIDADE> ExecuteFilter(String numero, Int32? torre, Int32? idTipo, Int32? alugada, Int32 idAss);
    }
}
