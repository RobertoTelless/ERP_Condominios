using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaBancariaRepository : IRepositoryBase<CONTA_BANCO>
    {
        CONTA_BANCO CheckExist(CONTA_BANCO conta, Int32 idAss);
        CONTA_BANCO GetItemById(Int32 id);
        CONTA_BANCO GetContaPadrao(Int32 idAss);
        Decimal GetTotalContas(Int32 idAss);
        List<CONTA_BANCO> GetAllItens(Int32 idAss);
        List<CONTA_BANCO> GetAllItensAdm(Int32 idAss);
    }
}

