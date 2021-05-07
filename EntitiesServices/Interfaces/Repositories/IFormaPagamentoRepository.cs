using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFormaPagamentoRepository : IRepositoryBase<FORMA_PAGAMENTO>
    {
        List<FORMA_PAGAMENTO> GetAllItens(Int32 tipo);
        List<FORMA_PAGAMENTO> GetAllItensAdm();
        FORMA_PAGAMENTO GetItemById(Int32 id);
        List<FORMA_PAGAMENTO> GetAllItens();
    }
}
