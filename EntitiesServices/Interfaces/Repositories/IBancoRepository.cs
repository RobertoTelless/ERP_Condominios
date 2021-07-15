using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IBancoRepository : IRepositoryBase<BANCO>
    {
        BANCO CheckExist(BANCO conta, Int32 idAss);
        BANCO GetByCodigo(String codigo);
        BANCO GetItemById(Int32 id);
        List<BANCO> GetAllItens(Int32 idAss);
        List<BANCO> GetAllItensAdm(Int32 idAss);
        List<BANCO> ExecuteFilter(String codigo, String nome, Int32 idAss);
    }
}

