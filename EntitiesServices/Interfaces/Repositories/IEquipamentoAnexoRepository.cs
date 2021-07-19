using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEquipamentoAnexoRepository : IRepositoryBase<EQUIPAMENTO_ANEXO>
    {
        List<EQUIPAMENTO_ANEXO> GetAllItens(Int32 idAss);
        EQUIPAMENTO_ANEXO GetItemById(Int32 id);
    }
}
