using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IEncomendaService : IServiceBase<ENCOMENDA>
    {
        Int32 Create(ENCOMENDA item, LOG log);
        Int32 Create(ENCOMENDA item);
        Int32 Edit(ENCOMENDA item, LOG log);
        Int32 Edit(ENCOMENDA item);
        Int32 Delete(ENCOMENDA item, LOG log);

        ENCOMENDA GetItemById(Int32 id);
        List<ENCOMENDA> GetAllItens(Int32 idAss);
        List<ENCOMENDA> GetAllItensAdm(Int32 idAss);
        List<ENCOMENDA> GetByUnidade(Int32 idUnid);
        List<ENCOMENDA> GetByData(DateTime data, Int32 idAss);
        List<ENCOMENDA> ExecuteFilter(Int32? unid, Int32? forma, Int32? tipo, DateTime? data, Int32? status, Int32 idAss);
        ENCOMENDA_COMENTARIO GetComentarioById(Int32 id);
        ENCOMENDA_ANEXO GetAnexoById(Int32 id);

        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<FORMA_ENTREGA> GetAllFormas(Int32 idAss);
        List<TIPO_ENCOMENDA> GetAllTipos(Int32 idAss);
    }
}
