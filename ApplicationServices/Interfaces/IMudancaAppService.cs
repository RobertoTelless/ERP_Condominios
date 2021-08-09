using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IMudancaAppService : IAppServiceBase<SOLICITACAO_MUDANCA>
    {
        Int32 ValidateCreate(SOLICITACAO_MUDANCA item, USUARIO usuario);
        Int32 ValidateEdit(SOLICITACAO_MUDANCA item, SOLICITACAO_MUDANCA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(SOLICITACAO_MUDANCA item, SOLICITACAO_MUDANCA itemAntes);
        Int32 ValidateDelete(SOLICITACAO_MUDANCA item, USUARIO usuario);
        Int32 ValidateReativar(SOLICITACAO_MUDANCA item, USUARIO usuario);
        Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, SOLICITACAO_MUDANCA mudanca, String template);

        SOLICITACAO_MUDANCA CheckExist(SOLICITACAO_MUDANCA item, Int32 idAss);
        SOLICITACAO_MUDANCA GetItemById(Int32 id);
        List<SOLICITACAO_MUDANCA> GetAllItens(Int32 idAss);
        List<SOLICITACAO_MUDANCA> GetAllItensAdm(Int32 idAss);
        List<SOLICITACAO_MUDANCA> GetByUnidade(Int32 idUnid);
        Int32 ExecuteFilter(DateTime? data, Int32? entrada, Int32? status, Int32 idAss, out List<SOLICITACAO_MUDANCA> objeto);

        SOLICITACAO_MUDANCA_ANEXO GetAnexoById(Int32 id);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        SOLICITACAO_MUDANCA_COMENTARIO GetComentarioById(Int32 id);
    }
}
