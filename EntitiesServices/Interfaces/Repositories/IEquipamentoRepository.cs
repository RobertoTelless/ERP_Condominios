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
        EQUIPAMENTO CheckExist(EQUIPAMENTO item);
        EQUIPAMENTO GetByNumero(String numero);
        EQUIPAMENTO GetItemById(Int32 id);
        List<EQUIPAMENTO> GetAllItens();
        List<EQUIPAMENTO> GetAllItensAdm();
        List<EQUIPAMENTO> ExecuteFilter(Int32? catId, String nome, String numero, Int32? depreciado, Int32? manutencao);
        Int32 CalcularManutencaoVencida();
        Int32 CalcularDepreciados();
    }
}
