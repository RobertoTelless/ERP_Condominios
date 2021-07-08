using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAutorizacaoService : IServiceBase<AUTORIZACAO_ACESSO>
    {
        Int32 Create(AUTORIZACAO_ACESSO perfil, LOG log);
        Int32 Create(AUTORIZACAO_ACESSO perfil);
        Int32 Edit(AUTORIZACAO_ACESSO perfil, LOG log);
        Int32 Edit(AUTORIZACAO_ACESSO perfil);
        Int32 Delete(AUTORIZACAO_ACESSO perfil, LOG log);

        AUTORIZACAO_ACESSO CheckExist(AUTORIZACAO_ACESSO conta, Int32 idAss);
        AUTORIZACAO_ACESSO GetItemById(Int32 id);
        List<AUTORIZACAO_ACESSO> GetAllItens(Int32 idAss);
        List<AUTORIZACAO_ACESSO> GetAllItensAdm(Int32 idAss);
        List<AUTORIZACAO_ACESSO> ExecuteFilter(Int32? unid, String nome, String documento, String empresa, Int32? tipo, DateTime? data, Int32 idAss);

        List<TIPO_DOCUMENTO> GetAllTipos(Int32 idAss);
        List<GRAU_PARENTESCO> GetAllGraus(Int32 idAss);
        AUTORIZACAO_ACESSO_ANEXO GetAnexoById(Int32 id);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
