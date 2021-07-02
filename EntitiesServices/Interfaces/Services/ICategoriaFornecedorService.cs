using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaFornecedorService : IServiceBase<CATEGORIA_FORNECEDOR>
    {
        Int32 Create(CATEGORIA_FORNECEDOR perfil, LOG log);
        Int32 Create(CATEGORIA_FORNECEDOR perfil);
        Int32 Edit(CATEGORIA_FORNECEDOR perfil, LOG log);
        Int32 Edit(CATEGORIA_FORNECEDOR perfil);
        Int32 Delete(CATEGORIA_FORNECEDOR perfil, LOG log);

        CATEGORIA_FORNECEDOR GetItemById(Int32 id);
        List<CATEGORIA_FORNECEDOR> GetAllItens(Int32 idAss);
        List<CATEGORIA_FORNECEDOR> GetAllItensAdm(Int32 idAss);
    }
}
