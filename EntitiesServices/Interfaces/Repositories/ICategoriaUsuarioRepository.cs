using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaUsuarioRepository : IRepositoryBase<CATEGORIA_USUARIO>
    {
        List<CATEGORIA_USUARIO> GetAllItens(Int32 idAss);
        CATEGORIA_USUARIO GetItemById(Int32 id);
        List<CATEGORIA_USUARIO> GetAllItensAdm(Int32 idAss);
    }
}
