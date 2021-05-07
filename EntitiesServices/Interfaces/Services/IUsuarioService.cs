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
        USUARIO GetByEmail(String email);
        USUARIO GetByLogin(String login);
        USUARIO RetriveUserByEmail(String email);
        Int32 CreateUser(USUARIO usuario, LOG log);
        Int32 CreateUser(USUARIO usuario);
        Int32 EditUser(USUARIO usuario, LOG log);
        Int32 VerifyUserSubscription(USUARIO usuario);
        Int32 EditUser(USUARIO usuario);
        Endereco GetAdressCEP(string CEP);
        CONFIGURACAO CarregaConfiguracao(Int32 id);
        List<USUARIO> GetAllUsuariosAdm();
        USUARIO GetItemById(Int32 id);
        List<USUARIO> GetAllUsuarios();
        List<PERFIL> GetAllPerfis();
        List<USUARIO> GetAllItens();
        List<USUARIO> GetAllItensBloqueados();
        List<USUARIO> GetAllItensAcessoHoje();
        List<USUARIO> ExecuteFilter(Int32? perfilId, Int32? cargoId, String nome, String login, String email);
        TEMPLATE GetTemplateByCode(String codigo);
        USUARIO_ANEXO GetAnexoById(Int32 id);
        List<NOTIFICACAO> GetAllItensUser(Int32 id);
        List<NOTIFICACAO> GetNotificacaoNovas(Int32 id);
        List<NOTICIA> GetAllNoticias();
        TEMPLATE GetTemplate(String code);
        USUARIO GetComprador();
        USUARIO GetAprovador();
        USUARIO GetAdministrador();
    }
}
