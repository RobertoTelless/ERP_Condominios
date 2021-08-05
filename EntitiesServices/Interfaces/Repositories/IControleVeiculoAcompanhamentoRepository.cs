using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IControleVeiculoAcompanhamentoRepository : IRepositoryBase<CONTROLE_VEICULO_ACOMPANHAMENTO>
    {
        List<CONTROLE_VEICULO_ACOMPANHAMENTO> GetAllItens();
        CONTROLE_VEICULO_ACOMPANHAMENTO GetItemById(Int32 id);
    }
}
