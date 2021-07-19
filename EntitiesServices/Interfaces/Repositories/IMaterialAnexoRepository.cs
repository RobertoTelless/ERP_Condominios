using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMaterialAnexoRepository : IRepositoryBase<MATERIAL_ANEXO>
    {
        List<MATERIAL_ANEXO> GetAllItens();
        MATERIAL_ANEXO GetItemById(Int32 id);
    }
}
