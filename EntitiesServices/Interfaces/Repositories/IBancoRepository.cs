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
        BANCO CheckExist(BANCO conta);
        BANCO GetByCodigo(String codigo);
        BANCO GetItemById(Int32 id);
        List<BANCO> GetAllItens();
        List<BANCO> GetAllItensAdm();
        List<BANCO> ExecuteFilter(String codigo, String nome);
    }
}

