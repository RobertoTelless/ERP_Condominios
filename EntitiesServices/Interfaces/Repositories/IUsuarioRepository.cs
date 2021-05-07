using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IUsuarioRepository : IRepositoryBase<USUARIO>
    {
        USUARIO GetByEmail(String email);
        USUARIO GetByLogin(String login);
        USUARIO GetItemById(Int32 id);
        List<USUARIO> GetAllUsuarios();
        List<USUARIO> GetAllItens();
        List<USUARIO> GetAllItensBloqueados();
        List<USUARIO> GetAllItensAcessoHoje();
        List<USUARIO> GetAllUsuariosAdm();
        List<USUARIO> ExecuteFilter(Int32? perfilId, Int32? cargoId, String nome, String login, String email);
        USUARIO GetComprador();
        USUARIO GetAprovador();
        USUARIO GetAdministrador();
    }
}
