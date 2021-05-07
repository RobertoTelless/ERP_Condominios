using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFormaPagamentoService : IServiceBase<FORMA_PAGAMENTO>
    {
        Int32 Create(FORMA_PAGAMENTO perfil, LOG log);
        Int32 Create(FORMA_PAGAMENTO perfil);
        Int32 Edit(FORMA_PAGAMENTO perfil, LOG log);
        Int32 Edit(FORMA_PAGAMENTO perfil);
        Int32 Delete(FORMA_PAGAMENTO perfil, LOG log);
        FORMA_PAGAMENTO GetItemById(Int32 id);
        List<FORMA_PAGAMENTO> GetAllItens(Int32 tipo);
        List<FORMA_PAGAMENTO> GetAllItensAdm();
        List<FORMA_PAGAMENTO> GetAllItens();
    }
}
