using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoRepository : RepositoryBase<PRODUTO>, IProdutoRepository
    {
        public PRODUTO CheckExist(PRODUTO conta, Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO;
            query = query.Where(p => p.PROD_NM_NOME == conta.PROD_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public PRODUTO CheckExist(String barcode, String codigo, Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO;
            if (barcode != null)
            {
                query = query.Where(p => p.PROD_NR_BARCODE == barcode);
            }
            if (codigo != null)
            {
                query = query.Where(p => p.PROD_CD_CODIGO == codigo);
            }
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public PRODUTO GetByNome(String nome, Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.PROD_NM_NOME == nome);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Include(p => p.MOVIMENTO_ESTOQUE_PRODUTO);
            return query.FirstOrDefault();
        }

        public PRODUTO GetItemById(Int32 id)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO;
            query = query.Where(p => p.PROD_CD_ID == id);
            query = query.Include(p => p.PRODUTO_FORNECEDOR);
            query = query.Include(p => p.MOVIMENTO_ESTOQUE_PRODUTO);
            return query.FirstOrDefault();
        }

        public List<PRODUTO> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PRODUTO> GetPontoPedido(Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.PROD_QN_ESTOQUE < p.PROD_QN_QUANTIDADE_MINIMA);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PRODUTO> GetEstoqueZerado(Int32 idAss)
        {
            IQueryable<PRODUTO> query = Db.PRODUTO.Where(p => p.PROD_IN_ATIVO == 1);
            query = query.Where(p => p.PROD_QN_ESTOQUE == 0);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<PRODUTO> ExecuteFilter(Int32? catId, Int32? subId, String nome, String marca, String codigo, String cod, Int32? idAss, Int32 ativo)
        {
            List<PRODUTO> lista = new List<PRODUTO>();
            IQueryable<PRODUTO> query = Db.PRODUTO;
            query = query.Where(p => p.PROD_IN_ATIVO == ativo);
            if (catId != null)
            {
                query = query.Where(p => p.CAPR_CD_ID == catId);
            }
            if (subId != null)
            {
                query = query.Where(p => p.SCPR_CD_ID == subId);
            }
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PROD_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(marca))
            {
                query = query.Where(p => p.PROD_NM_MARCA.Contains(marca));
            }
            if (!String.IsNullOrEmpty(codigo))
            {
                query = query.Where(p => p.PROD_NR_BARCODE == codigo);
            }
            if (!String.IsNullOrEmpty(cod))
            {
                query = query.Where(p => p.PROD_CD_CODIGO.Contains(cod));
            }
            if (query != null)
            {
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                query = query.OrderBy(a => a.PROD_NM_NOME);
                lista = query.ToList<PRODUTO>();
            }
            return lista;
        }

    }
}
