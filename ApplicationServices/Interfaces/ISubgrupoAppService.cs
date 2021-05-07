using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ISubgrupoAppService : IAppServiceBase<SUBGRUPO>
    {
        Int32 ValidateCreate(SUBGRUPO item, USUARIO usuario);
        Int32 ValidateEdit(SUBGRUPO item, SUBGRUPO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(SUBGRUPO item, SUBGRUPO itemAntes);
        Int32 ValidateDelete(SUBGRUPO item, USUARIO usuario);
        Int32 ValidateReativar(SUBGRUPO item, USUARIO usuario);
        SUBGRUPO CheckExist(SUBGRUPO obj);
        List<SUBGRUPO> GetAllItens();
        List<SUBGRUPO> GetAllItensAdm();
        SUBGRUPO GetItemById(Int32 id);
        List<GRUPO> GetAllGrupos();
    }
}
