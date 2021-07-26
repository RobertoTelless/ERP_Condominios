using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IOcorrenciaService : IServiceBase<OCORRENCIA>
    {
        Int32 Create(OCORRENCIA item, LOG log);
        Int32 Create(OCORRENCIA item);
        Int32 Edit(OCORRENCIA item, LOG log);
        Int32 Edit(OCORRENCIA item);
        Int32 Delete(OCORRENCIA item, LOG log);

        OCORRENCIA GetItemById(Int32 id);
        List<OCORRENCIA> GetAllItens(Int32 idAss);
        List<OCORRENCIA> GetAllItensAdm(Int32 idAss);
        List<OCORRENCIA> GetAllItensUser(Int32 id, Int32 idAss);
        List<OCORRENCIA> GetAllItensUnidade(Int32 id, Int32 idAss);
        List<OCORRENCIA> GetOcorrenciasNovas(Int32 id, Int32 idAss);
        List<OCORRENCIA> ExecuteFilter(Int32? unidade, Int32? usuario, Int32? cat, String titulo, DateTime? data, String texto, Int32 idAss);
        
        OCORRENCIA_ANEXO GetAnexoById(Int32 id);
        List<CATEGORIA_OCORRENCIA> GetAllCategorias(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
