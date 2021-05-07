using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoPessoaService : IServiceBase<TIPO_PESSOA>
    {
        Int32 Create(TIPO_PESSOA item, LOG log);
        Int32 Create(TIPO_PESSOA item);
        Int32 Edit(TIPO_PESSOA item, LOG log);
        Int32 Edit(TIPO_PESSOA item);
        Int32 Delete(TIPO_PESSOA item, LOG log);
        TIPO_PESSOA GetItemById(Int32 id);
        List<TIPO_PESSOA> GetAllItens();
        List<TIPO_PESSOA> GetAllItensAdm();
    }
}
