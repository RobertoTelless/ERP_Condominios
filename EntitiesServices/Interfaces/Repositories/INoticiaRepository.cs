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
        List<NOTICIA> GetAllItens();
        List<NOTICIA> GetAllItensAdm();
        List<NOTICIA> ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link);
        List<NOTICIA> GetAllItensValidos();
    }
}
