using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFormaPagamentoAppService : IAppServiceBase<FORMA_PAGAMENTO>
    {
        Int32 ValidateCreate(FORMA_PAGAMENTO item, USUARIO usuario);
        Int32 ValidateEdit(FORMA_PAGAMENTO item, FORMA_PAGAMENTO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(FORMA_PAGAMENTO item, USUARIO usuario);
        Int32 ValidateReativar(FORMA_PAGAMENTO item, USUARIO usuario);
        List<FORMA_PAGAMENTO> GetAllItens(Int32 tipo);
        FORMA_PAGAMENTO GetItemById(Int32 id);
        List<FORMA_PAGAMENTO> GetAllItensAdm();
        List<FORMA_PAGAMENTO> GetAllItens();
    }
}
