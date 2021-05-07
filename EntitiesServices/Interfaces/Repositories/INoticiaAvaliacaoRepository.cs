using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface INoticiaAvaliacaoRepository : IRepositoryBase<NOTICIA_AVALIACAO>
    {
        List<NOTICIA_AVALIACAO> GetAllItens();
        NOTICIA_AVALIACAO GetItemById(Int32 id);
    }
}
