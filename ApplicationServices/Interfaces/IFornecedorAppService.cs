using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFornecedorAppService : IAppServiceBase<FORNECEDOR>
    {
        Int32 ValidateCreate(FORNECEDOR perfil, USUARIO usuario);
        Int32 ValidateEdit(FORNECEDOR perfil, FORNECEDOR perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(FORNECEDOR item, FORNECEDOR itemAntes);
        Int32 ValidateDelete(FORNECEDOR perfil, USUARIO usuario);
        Int32 ValidateReativar(FORNECEDOR perfil, USUARIO usuario);

        List<FORNECEDOR> GetAllItens(Int32 idAss);
        List<FORNECEDOR> GetAllItensAdm(Int32 idAss);
        FORNECEDOR GetItemById(Int32 id);
        FORNECEDOR GetByEmail(String email);
        FORNECEDOR CheckExist(FORNECEDOR conta, Int32 idAss);

        List<CATEGORIA_FORNECEDOR> GetAllTipos(Int32 idAss);
        FORNECEDOR_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, String rede, Int32? ativo, Int32 idAss, out List<FORNECEDOR> objeto);
        Int32 ExecuteFilterSemPedido(String nome, String cidade, Int32? uf, Int32 idAss, out List<FORNECEDOR> objeto);
        List<TIPO_PESSOA> GetAllTiposPessoa();
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);

        FORNECEDOR_CONTATO GetContatoById(Int32 id);
        Int32 ValidateEditContato(FORNECEDOR_CONTATO item);
        Int32 ValidateCreateContato(FORNECEDOR_CONTATO item);
    }
}
