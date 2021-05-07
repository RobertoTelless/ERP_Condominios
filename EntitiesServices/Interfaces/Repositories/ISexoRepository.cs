using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ISexoRepository : IRepositoryBase<SEXO>
    {
        List<SEXO> GetAllItens();
        List<SEXO> GetAllItensAdm();
        SEXO GetItemById(Int32 id);
    }
}
