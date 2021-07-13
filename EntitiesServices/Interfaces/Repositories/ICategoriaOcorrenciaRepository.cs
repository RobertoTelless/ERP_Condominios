using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaOcorrenciaRepository : IRepositoryBase<CATEGORIA_OCORRENCIA>
    {
        List<CATEGORIA_OCORRENCIA> GetAllItens(Int32 idAss);
        CATEGORIA_OCORRENCIA GetItemById(Int32 id);
        List<CATEGORIA_OCORRENCIA> GetAllItensAdm(Int32 idAss);
    }
}
