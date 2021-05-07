using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ILogService : IServiceBase<LOG>
    {
        LOG GetById(Int32 id);
        List<LOG> GetAllItens();
        List<LOG> ExecuteFilter(Int32? usuId, DateTime? data, String operacao);
        List<LOG> GetAllItensDataCorrente();
        List<LOG> GetAllItensMesCorrente();
        List<LOG> GetAllItensMesAnterior();
        List<LOG> GetAllItensUsuario(Int32 id);
    }
}
