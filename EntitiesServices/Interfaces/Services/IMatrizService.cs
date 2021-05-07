using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMatrizService : IServiceBase<MATRIZ>
    {
        Int32 Create(MATRIZ perfil, LOG log);
        Int32 Create(MATRIZ perfil);
        Int32 Edit(MATRIZ perfil, LOG log);
        Int32 Edit(MATRIZ perfil);
        Int32 Delete(MATRIZ perfil, LOG log);
        MATRIZ GetItemById(Int32 id);
        List<MATRIZ> GetAllItens();
        List<MATRIZ> GetAllItensAdm();
        List<TIPO_PESSOA> GetAllTipoPessoa();
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
    }
}
