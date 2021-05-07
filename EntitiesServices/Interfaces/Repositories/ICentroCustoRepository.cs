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
        CENTRO_CUSTO CheckExist(CENTRO_CUSTO item);
        CENTRO_CUSTO GetItemById(Int32 id);
        List<CENTRO_CUSTO> GetAllItens();
        List<CENTRO_CUSTO> GetAllItensAdm();
        List<CENTRO_CUSTO> GetAllDespesas();
        List<CENTRO_CUSTO> GetAllReceitas();
        List<CENTRO_CUSTO> ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome);
    }
}
