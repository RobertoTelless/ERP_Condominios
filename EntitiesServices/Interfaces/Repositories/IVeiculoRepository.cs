using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IVeiculoRepository : IRepositoryBase<VEICULO>
    {
        VEICULO CheckExist(VEICULO item, Int32 idAss);
        VEICULO GetItemById(Int32 id);
        List<VEICULO> GetAllItens(Int32 idAss);
        List<VEICULO> GetAllItensAdm(Int32 idAss);
        List<VEICULO> GetByUnidade(Int32 idUnid);
        List<VEICULO> ExecuteFilter(String placa, String marca, Int32? unid, Int32? idTipo, Int32? vaga, Int32 idAss);
    }
}
