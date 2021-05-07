using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IEquipamentoService : IServiceBase<EQUIPAMENTO>
    {
        Int32 Create(EQUIPAMENTO perfil, LOG log);
        Int32 Create(EQUIPAMENTO perfil);
        Int32 Edit(EQUIPAMENTO perfil, LOG log);
        Int32 Edit(EQUIPAMENTO perfil);
        Int32 Delete(EQUIPAMENTO perfil, LOG log);
        EQUIPAMENTO CheckExist(EQUIPAMENTO conta);
        EQUIPAMENTO GetItemById(Int32 id);
        EQUIPAMENTO GetByNumero(String numero);
        List<EQUIPAMENTO> GetAllItens();
        List<EQUIPAMENTO> GetAllItensAdm();
        List<CATEGORIA_EQUIPAMENTO> GetAllTipos();
        List<PERIODICIDADE> GetAllPeriodicidades();
        EQUIPAMENTO_ANEXO GetAnexoById(Int32 id);
        List<EQUIPAMENTO> ExecuteFilter(Int32? catId, String nome, String numero, Int32? depreciado, Int32? manutencao);
        Int32 CalcularManutencaoVencida();
        Int32 CalcularDepreciados();
        EQUIPAMENTO_MANUTENCAO GetItemManutencaoById(Int32 id);
        Int32 EditManutencao(EQUIPAMENTO_MANUTENCAO item);
        Int32 CreateManutencao(EQUIPAMENTO_MANUTENCAO item);
    }
}
