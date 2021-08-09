using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IUsuarioService : IServiceBase<USUARIO>
    {
        Boolean VerificarCredenciais(String senha, USUARIO usuario);
        USUARIO GetByEmail(String email, Int32 idAss);
        USUARIO GetByLogin(String login, Int32 idAss);
        USUARIO RetriveUserByEmail(String email);
        Int32 CreateUser(USUARIO usuario, LOG log);
        Int32 CreateUser(USUARIO usuario);
        Int32 EditUser(USUARIO usuario, LOG log);
        Int32 VerifyUserSubscription(USUARIO usuario);
        Int32 EditUser(USUARIO usuario);

        Endereco GetAdressCEP(string CEP);
        CONFIGURACAO CarregaConfiguracao(Int32 id);
        List<USUARIO> GetAllUsuariosAdm(Int32 idAss);
        USUARIO GetItemById(Int32 id);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<PERFIL> GetAllPerfis();
        List<USUARIO> GetAllItens(Int32 idAss);
        List<USUARIO> GetAllItensBloqueados(Int32 idAss);
        List<USUARIO> GetAllItensAcessoHoje(Int32 idAss);
        List<USUARIO> ExecuteFilter(Int32? perfilId, Int32? cargoId, String nome, String login, String email, Int32 idAss);
        TEMPLATE GetTemplateByCode(String codigo);
        USUARIO_ANEXO GetAnexoById(Int32 id);
        List<NOTIFICACAO> GetAllItensUser(Int32 id, Int32 idAss);
        List<NOTIFICACAO> GetNotificacaoNovas(Int32 id, Int32 idAss);
        List<NOTICIA> GetAllNoticias(Int32 idAss);
        TEMPLATE GetTemplate(String code);
        USUARIO GetAdministrador(Int32 idAss);
    }
}
