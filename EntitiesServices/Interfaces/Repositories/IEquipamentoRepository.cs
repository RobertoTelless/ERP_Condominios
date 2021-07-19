using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEquipamentoRepository : IRepositoryBase<EQUIPAMENTO>
    {
        EQUIPAMENTO CheckExist(EQUIPAMENTO item, Int32 idAss);
        EQUIPAMENTO GetByNumero(String numero, Int32 idAss);
        EQUIPAMENTO GetItemById(Int32 id);
        List<EQUIPAMENTO> GetAllItens(Int32 idAss);
        List<EQUIPAMENTO> GetAllItensAdm(Int32 idAss);
        List<EQUIPAMENTO> ExecuteFilter(Int32? catId, String nome, String numero, Int32? depreciado, Int32? manutencao, Int32 idAss);
        Int32 CalcularManutencaoVencida(Int32 idAss);
        Int32 CalcularDepreciados(Int32 idAss);
    }
}
