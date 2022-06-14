using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IEncomendaAppService : IAppServiceBase<ENCOMENDA>
    {
        Int32 ValidateCreate(ENCOMENDA item, USUARIO usuario);
        Int32 ValidateEdit(ENCOMENDA item, ENCOMENDA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(ENCOMENDA item, ENCOMENDA itemAntes);
        Int32 ValidateDelete(ENCOMENDA item, USUARIO usuario);
        Int32 ValidateReativar(ENCOMENDA item, USUARIO usuario);
        Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, ENCOMENDA entrada, String unid, String template);

        ENCOMENDA GetItemById(Int32 id);
        List<ENCOMENDA> GetAllItens(Int32 idAss);
        List<ENCOMENDA> GetAllItensAdm(Int32 idAss);
        List<ENCOMENDA> GetByUnidade(Int32 idUnid);
        List<ENCOMENDA> GetItemByData(DateTime data, Int32 idAss);
        ENCOMENDA_COMENTARIO GetComentarioById(Int32 id);
        ENCOMENDA_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? unid, Int32? forma, Int32? tipo, DateTime? data, Int32? status, Int32 idAss, out List<ENCOMENDA> objeto);

        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<FORMA_ENTREGA> GetAllFormas(Int32 idAss);
        List<TIPO_ENCOMENDA> GetAllTipos(Int32 idAss);
    }
}
