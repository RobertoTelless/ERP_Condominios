using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPatrimonioAppService : IAppServiceBase<PATRIMONIO>
    {
        Int32 ValidateCreate(PATRIMONIO perfil, USUARIO usuario);
        Int32 ValidateEdit(PATRIMONIO perfil, PATRIMONIO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(PATRIMONIO item, PATRIMONIO itemAntes);
        Int32 ValidateDelete(PATRIMONIO perfil, USUARIO usuario);
        Int32 ValidateReativar(PATRIMONIO perfil, USUARIO usuario);
        List<PATRIMONIO> GetAllItens();
        List<PATRIMONIO> GetAllItensAdm();
        PATRIMONIO GetItemById(Int32 id);
        PATRIMONIO GetByNumero(String numero);
        PATRIMONIO CheckExist(PATRIMONIO conta);
        List<CATEGORIA_PATRIMONIO> GetAllTipos();
        List<PATRIMONIO> CalcularDepreciados();
        List<PATRIMONIO> CalcularBaixados();
        PATRIMONIO_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? catId, String nome, String nuemro, Int32? filial, out List<PATRIMONIO> objeto);
        Int32 CalcularDiasDepreciacao(PATRIMONIO item);
    }
}
