using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IUsuarioAppService : IAppServiceBase<USUARIO>
    {
        USUARIO GetByEmail(String email, Int32 idAss);
        USUARIO GetByLogin(String login, Int32 idAss);
        List<USUARIO> GetAllUsuariosAdm(Int32 idAss);
        USUARIO GetItemById(Int32 id);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<USUARIO> GetAllItens(Int32 idAss);
        List<USUARIO> GetAllItensBloqueados(Int32 idAss);
        List<USUARIO> GetAllItensAcessoHoje(Int32 idAss);
        USUARIO_ANEXO GetAnexoById(Int32 id);
        List<NOTIFICACAO> GetAllItensUser(Int32 id, Int32 idAss);
        List<NOTIFICACAO> GetNotificacaoNovas(Int32 id, Int32 idAss);

        Int32 ValidateCreate(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateCreateAssinante(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateEdit(USUARIO usuario, USUARIO usuarioAntes, USUARIO usuarioLogado);
        Int32 ValidateEdit(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateLogin(String email, String senha, out USUARIO usuario);
        Int32 ValidateDelete(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateBloqueio(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateDesbloqueio(USUARIO usuario, USUARIO usuarioLogado);
        Int32 ValidateChangePassword(USUARIO usuario);
        Int32 ValidateReativar(USUARIO usuario, USUARIO usuarioLogado);

        Int32 GenerateNewPassword(String email);
        List<PERFIL> GetAllPerfis();
        Int32 ExecuteFilter(Int32? perfilId, Int32? cargoId, String nome, String login, String email, Int32 idAss, out List<USUARIO> objeto);
        List<NOTICIA> GetAllNoticias(Int32 idAss);
        USUARIO GetAdministrador(Int32 idAss);
    }
}
