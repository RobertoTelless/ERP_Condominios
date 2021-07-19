using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEquipamentoManutencaoRepository : IRepositoryBase<EQUIPAMENTO_MANUTENCAO>
    {
        List<EQUIPAMENTO_MANUTENCAO> GetAllItens(Int32 idAss);
        EQUIPAMENTO_MANUTENCAO GetItemById(Int32 id);
    }
}
