using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IUnidadeAppService : IAppServiceBase<UNIDADE>
    {
        Int32 ValidateCreate(UNIDADE item, USUARIO usuario);
        Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes, USUARIO usuario);
        Int32 ValidateEdit(UNIDADE item, UNIDADE itemAntes);
        Int32 ValidateDelete(UNIDADE item, USUARIO usuario);
        Int32 ValidateReativar(UNIDADE item, USUARIO usuario);
        Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario);

        UNIDADE CheckExist(UNIDADE item, Int32 idAss);
        UNIDADE GetItemById(Int32 id);
        List<UNIDADE> GetAllItens(Int32 idAss);
        List<UNIDADE> GetAllItensAdm(Int32 idAss);
        Int32 ExecuteFilter(String numero, Int32? torre, Int32? idTipo, Int32? alugada, Int32 idAss, out List<UNIDADE> objeto);
        UNIDADE_ANEXO GetAnexoById(Int32 id);
        List<TIPO_UNIDADE> GetAllTipos(Int32 idAss);
        List<TORRE> GetAllTorres(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        TORRE GetTorreById(Int32 id);
    }
}
