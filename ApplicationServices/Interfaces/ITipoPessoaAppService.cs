using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoPessoaAppService : IAppServiceBase<TIPO_PESSOA>
    {
        Int32 ValidateCreate(TIPO_PESSOA item, USUARIO usuario);
        Int32 ValidateEdit(TIPO_PESSOA item, TIPO_PESSOA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(TIPO_PESSOA item, TIPO_PESSOA itemAntes);
        Int32 ValidateDelete(TIPO_PESSOA item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_PESSOA item, USUARIO usuario);
        List<TIPO_PESSOA> GetAllItens();
        List<TIPO_PESSOA> GetAllItensAdm();
        TIPO_PESSOA GetItemById(Int32 id);
    }
}
