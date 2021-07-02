using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFornecedorCnpjService : IServiceBase<FORNECEDOR_QUADRO_SOCIETARIO>
    {
        FORNECEDOR_QUADRO_SOCIETARIO CheckExist(FORNECEDOR_QUADRO_SOCIETARIO fqs, Int32 idAss);
        List<FORNECEDOR_QUADRO_SOCIETARIO> GetAllItens(Int32 idAss);
        List<FORNECEDOR_QUADRO_SOCIETARIO> GetByFornecedor(FORNECEDOR fornecedor);

        Int32 Create(FORNECEDOR_QUADRO_SOCIETARIO fqs, LOG log);
        Int32 Create(FORNECEDOR_QUADRO_SOCIETARIO fqs);
    }
}