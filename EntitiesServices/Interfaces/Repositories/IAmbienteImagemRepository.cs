using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAmbienteImagemRepository : IRepositoryBase<AMBIENTE_IMAGEM>
    {
        List<AMBIENTE_IMAGEM> GetAllItens();
        AMBIENTE_IMAGEM GetItemById(Int32 id);
    }
}
