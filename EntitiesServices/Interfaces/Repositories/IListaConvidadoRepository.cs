using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IListaConvidadoRepository : IRepositoryBase<LISTA_CONVIDADO>
    {
        LISTA_CONVIDADO CheckExist(LISTA_CONVIDADO item, Int32 idAss);
        LISTA_CONVIDADO GetItemById(Int32 id);
        List<LISTA_CONVIDADO> GetAllItens(Int32 idAss);
        List<LISTA_CONVIDADO> GetAllItensAdm(Int32 idAss);
        List<LISTA_CONVIDADO> GetByUnidade(Int32 idUnid);
        List<LISTA_CONVIDADO> ExecuteFilter(String nome, DateTime? data, Int32? unid, Int32? reserva, Int32 idAss);
    }
}
