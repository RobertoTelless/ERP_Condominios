using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAmbienteAppService : IAppServiceBase<AMBIENTE>
    {
        Int32 ValidateCreate(AMBIENTE perfil, USUARIO usuario);
        Int32 ValidateEdit(AMBIENTE perfil, AMBIENTE perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(AMBIENTE item, AMBIENTE itemAntes);
        Int32 ValidateDelete(AMBIENTE perfil, USUARIO usuario);
        Int32 ValidateReativar(AMBIENTE perfil, USUARIO usuario);

        List<AMBIENTE> GetAllItens(Int32 idAss);
        List<AMBIENTE> GetAllItensAdm(Int32 idAss);
        AMBIENTE GetItemById(Int32 id);
        AMBIENTE CheckExist(AMBIENTE conta, Int32 idAss);
        Int32 ExecuteFilter(Int32? tipo, String nome, Int32 idAss, out List<AMBIENTE> objeto);

        List<TIPO_AMBIENTE> GetAllTipos(Int32 idAss);
        AMBIENTE_IMAGEM GetAnexoById(Int32 id);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);

        AMBIENTE_CUSTO GetAmbienteCustoById(Int32 id);
        Int32 ValidateEditAmbienteCusto(AMBIENTE_CUSTO item);
        Int32 ValidateCreateAmbienteCusto(AMBIENTE_CUSTO item);

        AMBIENTE_CHAVE GetAmbienteChaveById(Int32 id);
        Int32 ValidateEditAmbienteChave(AMBIENTE_CHAVE item, USUARIO usuario);
        Int32 ValidateCreateAmbienteChave(AMBIENTE_CHAVE item, USUARIO usuario);
    }
}
