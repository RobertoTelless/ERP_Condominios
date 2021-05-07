using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaFornecedorAppService : IAppServiceBase<CATEGORIA_FORNECEDOR>
    {
        Int32 ValidateCreate(CATEGORIA_FORNECEDOR item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_FORNECEDOR item, CATEGORIA_FORNECEDOR itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CATEGORIA_FORNECEDOR item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_FORNECEDOR item, USUARIO usuario);
        List<CATEGORIA_FORNECEDOR> GetAllItens();
        CATEGORIA_FORNECEDOR GetItemById(Int32 id);
        List<CATEGORIA_FORNECEDOR> GetAllItensAdm();
    }
}
