using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IOcorrenciaRepository : IRepositoryBase<OCORRENCIA>
    {
        OCORRENCIA GetItemById(Int32 id);
        List<OCORRENCIA> GetAllItens(Int32 idAss);
        List<OCORRENCIA> GetAllItensAdm(Int32 idAss);
        List<OCORRENCIA> GetAllItensUser(Int32 id, Int32 idAss);
        List<OCORRENCIA> GetAllItensUnidade(Int32 id, Int32 idAss);
        List<OCORRENCIA> GetOcorrenciasNovas(Int32 id, Int32 idAss);
        List<OCORRENCIA> ExecuteFilter(Int32? unidade, Int32? usuario, Int32? cat, String titulo, DateTime? data, String texto, Int32 idAss);
    }
}
