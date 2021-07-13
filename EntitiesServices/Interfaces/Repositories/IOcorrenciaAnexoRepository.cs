using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IOcorrenciaAnexoRepository : IRepositoryBase<OCORRENCIA_ANEXO>
    {
        List<OCORRENCIA_ANEXO> GetAllItens();
        OCORRENCIA_ANEXO GetItemById(Int32 id);
    }
}
