using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IRamoAtividadeAppService : IAppServiceBase<RAMO_ATIVIDADE>
    {
        Int32 ValidateCreate(RAMO_ATIVIDADE item, USUARIO usuario);
        Int32 ValidateEdit(RAMO_ATIVIDADE item, RAMO_ATIVIDADE itemAntes, USUARIO usuario);
        Int32 ValidateEdit(RAMO_ATIVIDADE item, RAMO_ATIVIDADE itemAntes);
        Int32 ValidateDelete(RAMO_ATIVIDADE item, USUARIO usuario);
        Int32 ValidateReativar(RAMO_ATIVIDADE item, USUARIO usuario);
        List<RAMO_ATIVIDADE> GetAllItens();
        List<RAMO_ATIVIDADE> GetAllItensAdm();
        RAMO_ATIVIDADE GetItemById(Int32 id);
    }
}
