using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class MovimentoEstoqueProdutoRepository : RepositoryBase<MOVIMENTO_ESTOQUE_PRODUTO>, IMovimentoEstoqueProdutoRepository
    {
        public MOVIMENTO_ESTOQUE_PRODUTO GetByProdId(Int32 prod, Int32 idAss)
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            query = query.Where(x => x.PROD_CD_ID == prod);
            query = query.Where(x => x.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public MOVIMENTO_ESTOQUE_PRODUTO GetItemById(Int32 id)
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            query = query.Where(p => p.MOEP_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItens(Int32 idAss)
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOEP_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensEntrada(Int32 idAss)
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOEP_IN_ATIVO == 1);
            query = query.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 1);
            return query.ToList();
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllItensSaida(Int32 idAss)
        {
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOEP_IN_ATIVO == 1);
            query = query.Where(p => p.MOEP_IN_TIPO_MOVIMENTO == 2);
            return query.ToList();
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilter(Int32? catId, Int32? subCatId, String nome, String barcode, Int32? idAss, DateTime? dtMov)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            if (catId != null)
            {
                query = query.Where(p => p.PRODUTO.CAPR_CD_ID == catId);
            }
            if (subCatId != null)
            {
                query = query.Where(p => p.PRODUTO.SCPR_CD_ID == subCatId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PRODUTO.PROD_NM_NOME == nome);
            }
            if (!String.IsNullOrEmpty(barcode))
            {
                query = query.Where(p => p.PRODUTO.PROD_NR_BARCODE == barcode);
            }
            if (dtMov != new DateTime(0001, 01, 01, 00, 00, 00))
            {
                query = query.Where(p => p.MOEP_DT_MOVIMENTO == dtMov);
            }

            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(p => p.PRODUTO.PROD_NM_NOME);
                lista = query.ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
            }

            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilterAvulso(Int32? operacao, Int32? tipoMovimento, DateTime? dtInicial, DateTime? dtFinal, Int32? idAss, Int32? prod)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
            IQueryable<MOVIMENTO_ESTOQUE_PRODUTO> query = Db.MOVIMENTO_ESTOQUE_PRODUTO;
            if (operacao != null)
            {
                query = query.Where(x => x.MOEP_IN_TIPO_MOVIMENTO == operacao);
            }
            if (tipoMovimento != null)
            {
                query = query.Where(x => x.MOEP_IN_CHAVE_ORIGEM == tipoMovimento);
            }
            if (prod != null)
            {
                query = query.Where(x => x.PROD_CD_ID == prod);
            }
            if (query != null)
            {
                query = query.Where(x => x.ASSI_CD_ID == idAss);
                query = query.OrderBy(x => x.MOEP_DT_MOVIMENTO);
                lista = query.ToList<MOVIMENTO_ESTOQUE_PRODUTO>();

                if (dtInicial != null)
                {
                    lista = lista.Where(x => x.MOEP_DT_MOVIMENTO.Date >= dtInicial.Value.Date).ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
                }

                if (dtFinal != null)
                {
                    lista = lista.Where(x => x.MOEP_DT_MOVIMENTO.Date >= dtFinal.Value.Date).ToList<MOVIMENTO_ESTOQUE_PRODUTO>();
                }
            }

            return lista;
        }
    }
}
