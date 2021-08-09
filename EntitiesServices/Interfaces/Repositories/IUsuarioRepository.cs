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
        USUARIO GetByEmail(String email, Int32 idAss);
        USUARIO GetByLogin(String login, Int32 idAss);
        USUARIO GetItemById(Int32 id);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<USUARIO> GetAllItens(Int32 idAss);
        List<USUARIO> GetAllItensBloqueados(Int32 idAss);
        List<USUARIO> GetAllItensAcessoHoje(Int32 idAss);
        List<USUARIO> GetAllUsuariosAdm(Int32 idAss);
        List<USUARIO> ExecuteFilter(Int32? perfilId, Int32? cargoId, String nome, String login, String email, Int32 idAss);
        USUARIO GetAdministrador(Int32 idAss);
        USUARIO GetByEmailOnly(String email);
    }
}
