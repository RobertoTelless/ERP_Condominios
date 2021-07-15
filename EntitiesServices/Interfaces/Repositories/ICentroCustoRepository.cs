using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICentroCustoRepository : IRepositoryBase<CENTRO_CUSTO>
    {
        CENTRO_CUSTO CheckExist(CENTRO_CUSTO item, Int32 idAss);
        CENTRO_CUSTO GetItemById(Int32 id);
        List<CENTRO_CUSTO> GetAllItens(Int32 idAss);
        List<CENTRO_CUSTO> GetAllItensAdm(Int32 idAss);
        List<CENTRO_CUSTO> GetAllDespesas(Int32 idAss);
        List<CENTRO_CUSTO> GetAllReceitas(Int32 idAss);
        List<CENTRO_CUSTO> ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome, Int32 idAss);
    }
}
