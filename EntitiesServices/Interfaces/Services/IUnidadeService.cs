using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IUnidadeService : IServiceBase<UNIDADE>
    {
        Int32 Create(UNIDADE item, LOG log);
        Int32 Create(UNIDADE item);
        Int32 Edit(UNIDADE item, LOG log);
        Int32 Edit(UNIDADE item);
        Int32 Delete(UNIDADE item, LOG log);

        UNIDADE CheckExist(UNIDADE item, Int32 idAss);
        UNIDADE GetItemById(Int32 id);
        List<UNIDADE> GetAllItens(Int32 idAss);
        List<UNIDADE> GetAllItensAdm(Int32 idAss);
        List<UNIDADE> ExecuteFilter(String numero, Int32? torre, Int32? idTipo, Int32? alugada, Int32 idAss);
        UNIDADE_ANEXO GetAnexoById(Int32 id);
        List<TIPO_UNIDADE> GetAllTipos(Int32 idAss);
        List<TORRE> GetAllTorres(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
