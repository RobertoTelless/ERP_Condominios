using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAutorizacaoAppService : IAppServiceBase<AUTORIZACAO_ACESSO>
    {
        Int32 ValidateCreate(AUTORIZACAO_ACESSO perfil, USUARIO usuario);
        Int32 ValidateEdit(AUTORIZACAO_ACESSO perfil, AUTORIZACAO_ACESSO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(AUTORIZACAO_ACESSO item, AUTORIZACAO_ACESSO itemAntes);
        Int32 ValidateDelete(AUTORIZACAO_ACESSO perfil, USUARIO usuario);
        Int32 ValidateReativar(AUTORIZACAO_ACESSO perfil, USUARIO usuario);

        List<AUTORIZACAO_ACESSO> GetAllItens(Int32 idAss);
        List<AUTORIZACAO_ACESSO> GetAllItensAdm(Int32 idAss);
        AUTORIZACAO_ACESSO GetItemById(Int32 id);
        AUTORIZACAO_ACESSO CheckExist(AUTORIZACAO_ACESSO conta, Int32 idAss);
        Int32 ExecuteFilter(Int32? unid, String nome, String documento, String empresa, Int32? tipo, DateTime? data, Int32 idAss, out List<AUTORIZACAO_ACESSO> objeto);

        List<TIPO_DOCUMENTO> GetAllTipos(Int32 idAss);
        List<GRAU_PARENTESCO> GetAllGraus(Int32 idAss);
        AUTORIZACAO_ACESSO_ANEXO GetAnexoById(Int32 id);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
