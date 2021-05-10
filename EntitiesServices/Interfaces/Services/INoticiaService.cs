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
        List<NOTICIA> GetAllItens();
        List<NOTICIA> GetAllItensAdm();
        List<NOTICIA> ExecuteFilter(String titulo, String autor, DateTime? data, String texto, String link);
        List<NOTICIA> GetAllItensValidos();
        NOTICIA_COMENTARIO GetComentarioById(Int32 id);
    }
}
