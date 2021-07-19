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

        EQUIPAMENTO CheckExist(EQUIPAMENTO conta, Int32 idAss);
        EQUIPAMENTO GetItemById(Int32 id);
        EQUIPAMENTO GetByNumero(String numero, Int32 idAss);
        List<EQUIPAMENTO> GetAllItens(Int32 idAss);
        List<EQUIPAMENTO> GetAllItensAdm(Int32 idAss);
        List<CATEGORIA_EQUIPAMENTO> GetAllTipos(Int32 idAss);
        List<PERIODICIDADE> GetAllPeriodicidades(Int32 idAss);
        EQUIPAMENTO_ANEXO GetAnexoById(Int32 id);
        List<EQUIPAMENTO> ExecuteFilter(Int32? catId, String nome, String numero, Int32? depreciado, Int32? manutencao, Int32 idAss);
        
        Int32 CalcularManutencaoVencida(Int32 idAss);
        Int32 CalcularDepreciados(Int32 idAss);
        EQUIPAMENTO_MANUTENCAO GetItemManutencaoById(Int32 id);
        Int32 EditManutencao(EQUIPAMENTO_MANUTENCAO item);
        Int32 CreateManutencao(EQUIPAMENTO_MANUTENCAO item);
    }
}
