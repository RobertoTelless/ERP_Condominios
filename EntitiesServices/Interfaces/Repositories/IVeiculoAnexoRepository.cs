using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVeiculoAnexoRepository : IRepositoryBase<VEICULO_ANEXO>
    {
        List<VEICULO_ANEXO> GetAllItens();
        VEICULO_ANEXO GetItemById(Int32 id);
    }
}
