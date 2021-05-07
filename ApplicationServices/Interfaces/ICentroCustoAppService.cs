using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICentroCustoAppService : IAppServiceBase<CENTRO_CUSTO>
    {
        Int32 ValidateCreate(CENTRO_CUSTO item, USUARIO usuario);
        Int32 ValidateEdit(CENTRO_CUSTO item, CENTRO_CUSTO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CENTRO_CUSTO item, USUARIO usuario);
        Int32 ValidateReativar(CENTRO_CUSTO item, USUARIO usuario);
        Int32 ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome, out List<CENTRO_CUSTO> objeto);
        List<CENTRO_CUSTO> GetAllItens();
        CENTRO_CUSTO GetItemById(Int32 id);
        List<CENTRO_CUSTO> GetAllItensAdm();
        CENTRO_CUSTO CheckExist(CENTRO_CUSTO obj);
        List<CENTRO_CUSTO> GetAllDespesas();
        List<CENTRO_CUSTO> GetAllReceitas();
    }
}
