using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IEntradaSaidaAppService : IAppServiceBase<ENTRADA_SAIDA>
    {
        Int32 ValidateCreate(ENTRADA_SAIDA item, USUARIO usuario);
        Int32 ValidateEdit(ENTRADA_SAIDA item, ENTRADA_SAIDA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(ENTRADA_SAIDA item, ENTRADA_SAIDA itemAntes);
        Int32 ValidateDelete(ENTRADA_SAIDA item, USUARIO usuario);
        Int32 ValidateReativar(ENTRADA_SAIDA item, USUARIO usuario);
        Int32 GerarNotificacao(NOTIFICACAO item, USUARIO usuario, ENTRADA_SAIDA entrada, String template);

        ENTRADA_SAIDA GetItemById(Int32 id);
        List<ENTRADA_SAIDA> GetItemByData(DateTime data, Int32 idAss);
        List<ENTRADA_SAIDA> GetAllItens(Int32 idAss);
        List<ENTRADA_SAIDA> GetAllItensAdm(Int32 idAss);
        List<ENTRADA_SAIDA> GetByUnidade(Int32 idUnid);
        Int32 ExecuteFilter(String nome, String documento, Int32? unid, Int32? autorizacao, DateTime? dataEntrada, DateTime? dataSaida, Int32? status, Int32 idAss, out List<ENTRADA_SAIDA> objeto);

        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllCatNotificacao(Int32 idAss);
        List<USUARIO> GetAllUsuarios(Int32 idAss);
        List<AUTORIZACAO_ACESSO> GetAllAutorizacoes(Int32 idAss);
        List<GRAU_PARENTESCO> GetAllGraus(Int32 idAss);
    }
}
