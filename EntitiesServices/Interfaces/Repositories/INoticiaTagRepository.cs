using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface INoticiaTagRepository : IRepositoryBase<NOTICIA_TAG>
    {
        List<NOTICIA_TAG> GetAllItens();
        NOTICIA_TAG GetItemById(Int32 id);
    }
}
