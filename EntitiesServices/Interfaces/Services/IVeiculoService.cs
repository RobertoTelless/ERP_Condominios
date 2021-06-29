using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IVeiculoService : IServiceBase<VEICULO>
    {
        Int32 Create(VEICULO item, LOG log);
        Int32 Create(VEICULO item);
        Int32 Edit(VEICULO item, LOG log);
        Int32 Edit(VEICULO item);
        Int32 Delete(VEICULO item, LOG log);

        VEICULO CheckExist(VEICULO item, Int32 idAss);
        VEICULO GetItemById(Int32 id);
        List<VEICULO> GetAllItens(Int32 idAss);
        List<VEICULO> GetAllItensAdm(Int32 idAss);
        List<VEICULO> GetByUnidade(Int32 idUnid);
        List<VEICULO> ExecuteFilter(String placa, String marca, Int32? unid, Int32? idTipo, Int32? vaga, Int32 idAss);

        VEICULO_ANEXO GetAnexoById(Int32 id);
        List<TIPO_VEICULO> GetAllTipos(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<VAGA> GetAllVagas(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
