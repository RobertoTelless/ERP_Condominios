using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IVagaService : IServiceBase<VAGA>
    {
        Int32 Create(VAGA item, LOG log);
        Int32 Create(VAGA item);
        Int32 Edit(VAGA item, LOG log);
        Int32 Edit(VAGA item);
        Int32 Delete(VAGA item, LOG log);

        VAGA CheckExist(VAGA item, Int32 idAss);
        VAGA GetItemById(Int32 id);
        List<VAGA> GetAllItens(Int32 idAss);
        List<VAGA> GetAllItensAdm(Int32 idAss);
        List<VAGA> ExecuteFilter(String numero, String andar, Int32? unid, Int32? idTipo, Int32 idAss);
        List<TIPO_VAGA> GetAllTipos(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<VEICULO> GetVeiculosUnidade(Int32 idUnid);
    }
}
