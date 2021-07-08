using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAutorizacaoRepository : IRepositoryBase<AUTORIZACAO_ACESSO>
    {
        AUTORIZACAO_ACESSO CheckExist(AUTORIZACAO_ACESSO item, Int32 idAss);
        AUTORIZACAO_ACESSO GetItemById(Int32 id);
        List<AUTORIZACAO_ACESSO> GetAllItens(Int32 idAss);
        List<AUTORIZACAO_ACESSO> GetAllItensAdm(Int32 idAss);
        List<AUTORIZACAO_ACESSO> ExecuteFilter(Int32? unid, String nome, String documento, String empresa, Int32? tipo, DateTime? data, Int32 idAss);
    }
}
