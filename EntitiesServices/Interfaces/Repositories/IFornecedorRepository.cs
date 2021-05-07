using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFornecedorRepository : IRepositoryBase<FORNECEDOR>
    {
        FORNECEDOR CheckExist(FORNECEDOR item);
        FORNECEDOR GetByEmail(String email);
        FORNECEDOR GetItemById(Int32 id);
        List<FORNECEDOR> GetAllItens();
        List<FORNECEDOR> GetAllItensAdm();
        List<FORNECEDOR> ExecuteFilter(Int32? catId, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, String rede, Int32? ativo);
        List<FORNECEDOR> ExecuteFilterSemPedido(String nome, String cidade, Int32? uf);
    }
}
