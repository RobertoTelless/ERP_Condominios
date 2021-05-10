using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface INoticiaService : IServiceBase<NOTICIA>
    {
        Int32 Create(NOTICIA item, LOG log);
        Int32 Create(NOTICIA item);
        Int32 Edit(NOTICIA item, LOG log);
        Int32 Edit(NOTICIA item);
        Int32 Delete(NOTICIA item, LOG log);

        NOTICIA GetItemById(Int32 id);
        List<NOTICIA> GetAllItens(Int32 idAss);
        List<NOTICIA> GetAllItensAdm(Int32 idAss);
        List<NOTICIA> ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link, Int32 idAss);
        List<NOTICIA> GetAllItensValidos(Int32 idAss);
        NOTICIA_COMENTARIO GetComentarioById(Int32 id);
    }
}
