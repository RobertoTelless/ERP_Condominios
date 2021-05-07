using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFuncionarioRepository : IRepositoryBase<FUNCIONARIO>
    {
        FUNCIONARIO CheckExist(FUNCIONARIO item);
        FUNCIONARIO GetByNome(String nome);
        FUNCIONARIO GetItemById(Int32 id);
        List<FUNCIONARIO> GetAllItens();
        List<FUNCIONARIO> GetAllItensAdm();
        List<FUNCIONARIO> ExecuteFilter(Int32? sitId, String nome, String cpf, String rg, Int32? funId);
    }
}
