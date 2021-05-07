using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFuncionarioAppService : IAppServiceBase<FUNCIONARIO>
    {
        Int32 ValidateCreate(FUNCIONARIO perfil, USUARIO usuario);
        Int32 ValidateEdit(FUNCIONARIO perfil, FUNCIONARIO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(FUNCIONARIO item, FUNCIONARIO itemAntes);
        Int32 ValidateDelete(FUNCIONARIO perfil, USUARIO usuario);
        Int32 ValidateReativar(FUNCIONARIO perfil, USUARIO usuario);
        List<FUNCIONARIO> GetAllItens();
        List<FUNCIONARIO> GetAllItensAdm();
        FUNCIONARIO GetItemById(Int32 id);
        FUNCIONARIO GetByNome(String nome);
        FUNCIONARIO CheckExist(FUNCIONARIO conta);

        List<SITUACAO> GetAllSituacao();
        List<SEXO> GetAllSexo();
        List<FUNCAO> GetAllFuncao();
        List<ESCOLARIDADE> GetAllEscolaridade();

        FUNCIONARIO_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? sitId, String nome, String cpf, String rg, Int32? funId, out List<FUNCIONARIO> objeto);
    }
}
