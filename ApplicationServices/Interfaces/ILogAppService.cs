using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ILogAppService : IAppServiceBase<LOG>
    {
        LOG GetById(Int32 id);
        List<LOG> GetAllItens();
        Int32 ExecuteFilter(Int32? usuId, DateTime? data, String operacao, out List<LOG> objeto);
        List<LOG> GetAllItensDataCorrente();
        List<LOG> GetAllItensMesCorrente();
        List<LOG> GetAllItensMesAnterior();
        List<LOG> GetAllItensUsuario(Int32 id);
    }
}
