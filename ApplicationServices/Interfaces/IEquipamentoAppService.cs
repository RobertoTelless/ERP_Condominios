using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IEquipamentoAppService : IAppServiceBase<EQUIPAMENTO>
    {
        Int32 ValidateCreate(EQUIPAMENTO perfil, USUARIO usuario);
        Int32 ValidateEdit(EQUIPAMENTO perfil, EQUIPAMENTO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(EQUIPAMENTO item, EQUIPAMENTO itemAntes);
        Int32 ValidateDelete(EQUIPAMENTO perfil, USUARIO usuario);
        Int32 ValidateReativar(EQUIPAMENTO perfil, USUARIO usuario);

        List<EQUIPAMENTO> GetAllItens(Int32 idAss);
        List<EQUIPAMENTO> GetAllItensAdm(Int32 idAss);
        EQUIPAMENTO GetItemById(Int32 id);
        EQUIPAMENTO GetByNumero(String numero, Int32 idAss);
        EQUIPAMENTO CheckExist(EQUIPAMENTO conta, Int32 idAss);
        List<CATEGORIA_EQUIPAMENTO> GetAllTipos(Int32 idAss);
        EQUIPAMENTO_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? catId, String nome, String numero, Int32? depreciado, Int32? manutencao, Int32 idAss, out List<EQUIPAMENTO> objeto);
        
        Int32 CalcularDiasDepreciacao(EQUIPAMENTO item);
        Int32 CalcularManutencaoVencida(Int32 idAss);
        Int32 CalcularDepreciados(Int32 idAss);
        Int32 CalcularDiasManutencao(EQUIPAMENTO item);
        List<PERIODICIDADE> GetAllPeriodicidades(Int32 idAss);
        EQUIPAMENTO_MANUTENCAO GetItemManutencaoById(Int32 id);
        Int32 ValidateEditManutencao(EQUIPAMENTO_MANUTENCAO item);
        Int32 ValidateCreateManutencao(EQUIPAMENTO_MANUTENCAO item);
    }
}
