using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAssinanteService : IServiceBase<ASSINANTE>
    {
        Int32 Create(ASSINANTE perfil, LOG log);
        Int32 Create(ASSINANTE perfil);
        Int32 Edit(ASSINANTE perfil, LOG log);
        Int32 Edit(ASSINANTE perfil);
        Int32 Delete(ASSINANTE perfil, LOG log);
        ASSINANTE CheckExist(ASSINANTE conta);
        ASSINANTE GetItemById(Int32 id);
        List<ASSINANTE> GetAllItens();
        List<ASSINANTE> GetAllItensAdm();
        List<ASSINANTE> ExecuteFilter(Int32 tipo, String nome);
    }
}
