using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IUsuarioControleEntradaRepository : IRepositoryBase<USUARIO_CONTROLE_ENTRADA>
    {
        USUARIO_CONTROLE_ENTRADA CheckExist(USUARIO_CONTROLE_ENTRADA item, Int32 idAss);
        USUARIO_CONTROLE_ENTRADA GetItemById(Int32 id);
        List<USUARIO_CONTROLE_ENTRADA> GetAllItens(Int32 idAss);
        List<USUARIO_CONTROLE_ENTRADA> GetAllItensAdm(Int32 idAss);
        List<USUARIO_CONTROLE_ENTRADA> GetByData(DateTime data, Int32 idAss);
        List<USUARIO_CONTROLE_ENTRADA> ExecuteFilter(Int32? usuario, DateTime? data, Int32 idAss);
    }
}
