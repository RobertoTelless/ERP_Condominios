using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaEquipamentoService : IServiceBase<CATEGORIA_EQUIPAMENTO>
    {
        Int32 Create(CATEGORIA_EQUIPAMENTO perfil, LOG log);
        Int32 Create(CATEGORIA_EQUIPAMENTO perfil);
        Int32 Edit(CATEGORIA_EQUIPAMENTO perfil, LOG log);
        Int32 Edit(CATEGORIA_EQUIPAMENTO perfil);
        Int32 Delete(CATEGORIA_EQUIPAMENTO perfil, LOG log);
        CATEGORIA_EQUIPAMENTO GetItemById(Int32 id);
        List<CATEGORIA_EQUIPAMENTO> GetAllItens();
        List<CATEGORIA_EQUIPAMENTO> GetAllItensAdm();
    }
}
