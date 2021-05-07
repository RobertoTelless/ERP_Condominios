using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFuncionarioService : IServiceBase<FUNCIONARIO>
    {
        Int32 Create(FUNCIONARIO perfil, LOG log);
        Int32 Create(FUNCIONARIO perfil);
        Int32 Edit(FUNCIONARIO perfil, LOG log);
        Int32 Edit(FUNCIONARIO perfil);
        Int32 Delete(FUNCIONARIO perfil, LOG log);
        FUNCIONARIO CheckExist(FUNCIONARIO conta);
        FUNCIONARIO GetItemById(Int32 id);
        FUNCIONARIO GetByNome(String nome);
        List<FUNCIONARIO> GetAllItens();
        List<FUNCIONARIO> GetAllItensAdm();
        List<SEXO> GetAllSexo();
        List<SITUACAO> GetAllSituacao();
        List<ESCOLARIDADE> GetAllEscolaridade();
        List<FUNCAO> GetAllFuncao();
        FUNCIONARIO_ANEXO GetAnexoById(Int32 id);
        List<FUNCIONARIO> ExecuteFilter(Int32? sitId, String nome, String cpf, String rg, Int32? funId);
    }
}
