using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPatrimonioService : IServiceBase<PATRIMONIO>
    {
        Int32 Create(PATRIMONIO perfil, LOG log);
        Int32 Create(PATRIMONIO perfil);
        Int32 Edit(PATRIMONIO perfil, LOG log);
        Int32 Edit(PATRIMONIO perfil);
        Int32 Delete(PATRIMONIO perfil, LOG log);
        PATRIMONIO CheckExist(PATRIMONIO conta);
        PATRIMONIO GetItemById(Int32 id);
        PATRIMONIO GetByNumero(String numero);
        List<PATRIMONIO> GetAllItens();
        List<PATRIMONIO> GetAllItensAdm();
        List<CATEGORIA_PATRIMONIO> GetAllTipos();
        List<PATRIMONIO> CalcularDepreciados();
        List<PATRIMONIO> CalcularBaixados();
        PATRIMONIO_ANEXO GetAnexoById(Int32 id);
        List<PATRIMONIO> ExecuteFilter(Int32? catId, String nome, String numero, Int32? filial);
    }
}
