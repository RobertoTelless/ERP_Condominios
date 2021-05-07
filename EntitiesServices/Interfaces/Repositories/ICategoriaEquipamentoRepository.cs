using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaEquipamentoRepository : IRepositoryBase<CATEGORIA_EQUIPAMENTO>
    {
        List<CATEGORIA_EQUIPAMENTO> GetAllItens();
        CATEGORIA_EQUIPAMENTO GetItemById(Int32 id);
        List<CATEGORIA_EQUIPAMENTO> GetAllItensAdm();
    }
}
