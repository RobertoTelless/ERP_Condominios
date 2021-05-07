using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IBancoService : IServiceBase<BANCO>
    {
        Int32 Create(BANCO perfil, LOG log);
        Int32 Create(BANCO perfil);
        Int32 Edit(BANCO perfil, LOG log);
        Int32 Edit(BANCO perfil);
        Int32 Delete(BANCO perfil, LOG log);
        BANCO CheckExist(BANCO conta);
        BANCO GetByCodigo(String nome);
        BANCO GetItemById(Int32 id);
        List<BANCO> GetAllItens();
        List<BANCO> GetAllItensAdm();
        List<BANCO> ExecuteFilter(String codigo, String nome);
    }
}
