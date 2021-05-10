using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface INoticiaRepository : IRepositoryBase<NOTICIA>
    {
        NOTICIA GetItemById(Int32 id);
        List<NOTICIA> GetAllItens(Int32 idAss);
        List<NOTICIA> GetAllItensAdm(Int32 idAss);
        List<NOTICIA> ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link, Int32 idAss);
        List<NOTICIA> GetAllItensValidos(Int32 idAss);
    }
}
