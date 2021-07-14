using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IOcorrenciaAppService : IAppServiceBase<OCORRENCIA>
    {
        Int32 ValidateCreate(OCORRENCIA item, USUARIO usuario);
        Int32 ValidateEdit(OCORRENCIA item, OCORRENCIA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(OCORRENCIA item, OCORRENCIA itemAntes);
        Int32 ValidateDelete(OCORRENCIA item, USUARIO usuario);
        Int32 ValidateReativar(OCORRENCIA item, USUARIO usuario);
        Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, OCORRENCIA ocorrencia, String template);

        OCORRENCIA GetItemById(Int32 id);
        List<OCORRENCIA> GetAllItens(Int32 idAss);
        List<OCORRENCIA> GetAllItensAdm(Int32 idAss);
        List<OCORRENCIA> GetAllItensUser(Int32 id, Int32 idAss);
        List<OCORRENCIA> GetAllItensUnidade(Int32 id, Int32 idAss);
        List<OCORRENCIA> GetOcorrenciasNovas(Int32 id, Int32 idAss);
        Int32 ExecuteFilter(Int32? unidade, Int32? usuario, Int32? cat, String titulo, DateTime? data, String texto, Int32 idAss, out List<OCORRENCIA> objeto);
        
        OCORRENCIA_ANEXO GetAnexoById(Int32 id);
        List<CATEGORIA_OCORRENCIA> GetAllCategorias(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
    }
}
