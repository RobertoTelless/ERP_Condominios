using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPatrimonioRepository : IRepositoryBase<PATRIMONIO>
    {
        PATRIMONIO CheckExist(PATRIMONIO item);
        PATRIMONIO GetByNumero(String numero);
        PATRIMONIO GetItemById(Int32 id);
        List<PATRIMONIO> GetAllItens();
        List<PATRIMONIO> CalcularDepreciados();
        List<PATRIMONIO> CalcularBaixados();
        List<PATRIMONIO> GetAllItensAdm();
        List<PATRIMONIO> ExecuteFilter(Int32? catId, String nome, String numero, Int32? filial);
    }
}
