using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAmbienteService : IServiceBase<AMBIENTE>
    {
        Int32 Create(AMBIENTE perfil, LOG log);
        Int32 Create(AMBIENTE perfil);
        Int32 Edit(AMBIENTE perfil, LOG log);
        Int32 Edit(AMBIENTE perfil);
        Int32 Delete(AMBIENTE perfil, LOG log);

        AMBIENTE CheckExist(AMBIENTE conta, Int32 idAss);
        AMBIENTE GetItemById(Int32 id);
        List<AMBIENTE> GetAllItens(Int32 idAss);
        List<AMBIENTE> GetAllItensAdm(Int32 idAss);

        List<TIPO_AMBIENTE> GetAllTipos(Int32 idAss);
        AMBIENTE_IMAGEM GetAnexoById(Int32 id);
        List<AMBIENTE> ExecuteFilter(Int32? tipo, String nome, Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);

        AMBIENTE_CUSTO GetAmbienteCustoById(Int32 id);
        Int32 EditAmbienteCusto(AMBIENTE_CUSTO item);
        Int32 CreateAmbienteCusto(AMBIENTE_CUSTO item);

        AMBIENTE_CHAVE GetAmbienteChaveById(Int32 id);
        Int32 EditAmbienteChave(AMBIENTE_CHAVE item);
        Int32 CreateAmbienteChave(AMBIENTE_CHAVE item);
    }
}
