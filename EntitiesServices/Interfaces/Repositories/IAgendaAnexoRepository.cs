using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAgendaAnexoRepository : IRepositoryBase<AGENDA_ANEXO>
    {
        List<AGENDA_ANEXO> GetAllItens();
        AGENDA_ANEXO GetItemById(Int32 id);
    }
}
