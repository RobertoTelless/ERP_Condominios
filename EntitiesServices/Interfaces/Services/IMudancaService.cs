using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMudancaService : IServiceBase<SOLICITACAO_MUDANCA>
    {
        Int32 Create(SOLICITACAO_MUDANCA item, LOG log);
        Int32 Create(SOLICITACAO_MUDANCA item);
        Int32 Edit(SOLICITACAO_MUDANCA item, LOG log);
        Int32 Edit(SOLICITACAO_MUDANCA item);
        Int32 Delete(SOLICITACAO_MUDANCA item, LOG log);

        SOLICITACAO_MUDANCA CheckExist(SOLICITACAO_MUDANCA item, Int32 idAss);
        SOLICITACAO_MUDANCA GetItemById(Int32 id);
        List<SOLICITACAO_MUDANCA> GetAllItens(Int32 idAss);
        List<SOLICITACAO_MUDANCA> GetAllItensAdm(Int32 idAss);
        List<SOLICITACAO_MUDANCA> GetByUnidade(Int32 idUnid);
        List<SOLICITACAO_MUDANCA> ExecuteFilter(DateTime? data, Int32? entrada, Int32? status, Int32? idUnid, Int32 idAss);

        SOLICITACAO_MUDANCA_ANEXO GetAnexoById(Int32 id);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        SOLICITACAO_MUDANCA_COMENTARIO GetComentarioById(Int32 id);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
    }
}
