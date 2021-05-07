using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAssinanteRepository : IRepositoryBase<ASSINANTE>
    {
        ASSINANTE CheckExist(ASSINANTE conta);
        ASSINANTE GetItemById(Int32 id);
        List<ASSINANTE> GetAllItens();
        List<ASSINANTE> GetAllItensAdm();
        List<ASSINANTE> ExecuteFilter(Int32 tipo, String nome);
    }
}

