using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVagaRepository : IRepositoryBase<VAGA>
    {
        VAGA CheckExist(VAGA item, Int32 idAss);
        VAGA GetItemById(Int32 id);
        List<VAGA> GetAllItens(Int32 idAss);
        List<VAGA> GetAllItensAdm(Int32 idAss);
        List<VAGA> ExecuteFilter(String numero, String andar, Int32? unid, Int32? idTipo, Int32 idAss);
    }
}
