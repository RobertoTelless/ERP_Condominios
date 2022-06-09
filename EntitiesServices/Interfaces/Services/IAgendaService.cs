using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAgendaService : IServiceBase<AGENDA>
    {
        Int32 Create(AGENDA perfil, LOG log);
        Int32 Create(AGENDA perfil);
        Int32 Edit(AGENDA perfil, LOG log);
        Int32 Edit(AGENDA perfil);
        Int32 Delete(AGENDA perfil, LOG log);

        List<CATEGORIA_AGENDA> GetAllTipos(Int32 idAss);
        AGENDA_ANEXO GetAnexoById(Int32 id);

        List<AGENDA> GetByDate(DateTime data, Int32 idAss);
        List<AGENDA> GetByUser(Int32 id, Int32 idAss);
        AGENDA GetItemById(Int32 id);
        List<AGENDA> GetAllItens(Int32 idAss);
        List<AGENDA> GetAllItensAdm(Int32 idAss);
        List<AGENDA> ExecuteFilter(DateTime? data, Int32? cat, String titulo, String descricao, Int32 idAss, Int32 idUser, Int32 corp);
    }
}
