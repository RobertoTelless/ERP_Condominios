using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IDepartamentoService : IServiceBase<DEPARTAMENTO>
    {
        Int32 Create(DEPARTAMENTO item, LOG log);
        Int32 Create(DEPARTAMENTO item);
        Int32 Edit(DEPARTAMENTO item, LOG log);
        Int32 Edit(DEPARTAMENTO item);
        Int32 Delete(DEPARTAMENTO item, LOG log);
        DEPARTAMENTO GetItemById(Int32 id);
        List<DEPARTAMENTO> GetAllItens();
        List<DEPARTAMENTO> GetAllItensAdm();
    }
}
