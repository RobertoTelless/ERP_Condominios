using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IRamoAtividadeService : IServiceBase<RAMO_ATIVIDADE>
    {
        Int32 Create(RAMO_ATIVIDADE item, LOG log);
        Int32 Create(RAMO_ATIVIDADE item);
        Int32 Edit(RAMO_ATIVIDADE item, LOG log);
        Int32 Edit(RAMO_ATIVIDADE item);
        Int32 Delete(RAMO_ATIVIDADE item, LOG log);
        RAMO_ATIVIDADE GetItemById(Int32 id);
        List<RAMO_ATIVIDADE> GetAllItens();
        List<RAMO_ATIVIDADE> GetAllItensAdm();
    }
}
