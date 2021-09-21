using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IReservaAnexoRepository : IRepositoryBase<RESERVA_ANEXO>
    {
        List<RESERVA_ANEXO> GetAllItens(Int32 idAss);
        RESERVA_ANEXO GetItemById(Int32 id);
    }
}
