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
        List<FORNECEDOR> GetAllItens();
        List<FORNECEDOR> GetAllItensAdm();
        FORNECEDOR GetItemById(Int32 id);
        FORNECEDOR GetByEmail(String email);
        FORNECEDOR CheckExist(FORNECEDOR conta);
        List<CATEGORIA_FORNECEDOR> GetAllTipos();
        FORNECEDOR_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? catId, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, String rede, Int32? ativo, out List<FORNECEDOR> objeto);
        Int32 ExecuteFilterSemPedido(String nome, String cidade, Int32? uf, out List<FORNECEDOR> objeto);
        List<TIPO_PESSOA> GetAllTiposPessoa();
        FORNECEDOR_CONTATO GetContatoById(Int32 id);
        Int32 ValidateEditContato(FORNECEDOR_CONTATO item);
        Int32 ValidateCreateContato(FORNECEDOR_CONTATO item);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
    }
}
