using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITemplateRepository : IRepositoryBase<TEMPLATE>
    {
        TEMPLATE GetByCode(String code, Int32 idAss);
        List<TEMPLATE> GetAllItens(Int32 idAss);
        TEMPLATE GetItemById(Int32 id);
        List<TEMPLATE> GetAllItensAdm(Int32 idAss);
        List<TEMPLATE> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss);
        TEMPLATE CheckExist(TEMPLATE item, Int32 idAss);
    }
}
