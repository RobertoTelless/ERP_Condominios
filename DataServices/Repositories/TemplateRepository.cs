using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class TemplateRepository : RepositoryBase<TEMPLATE>, ITemplateRepository
    {
        public TEMPLATE GetByCode(String code)
        {
            IQueryable<TEMPLATE> query = Db.TEMPLATE.Where(p => p.TEMP_SG_SIGLA == code);
            return query.FirstOrDefault();
        }

        public TEMPLATE GetItemById(Int32 id)
        {
            IQueryable<TEMPLATE> query = Db.TEMPLATE;
            query = query.Where(p => p.TEMP_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TEMPLATE> GetAllItensAdm()
        {
            IQueryable<TEMPLATE> query = Db.TEMPLATE;
            return query.ToList();
        }

        public List<TEMPLATE> GetAllItens()
        {
            IQueryable<TEMPLATE> query = Db.TEMPLATE.Where(p => p.TEMP_IN_ATIVO == 1);
            return query.ToList();
        }

        public TEMPLATE CheckExist(TEMPLATE item)
        {
            IQueryable<TEMPLATE> query = Db.TEMPLATE;
            query = query.Where(p => p.TEMP_SG_SIGLA == item.TEMP_SG_SIGLA);
            return query.FirstOrDefault();
        }

        public List<TEMPLATE> ExecuteFilter(String sigla, String nome, String conteudo)
        {
            List<TEMPLATE> lista = new List<TEMPLATE>();
            IQueryable<TEMPLATE> query = Db.TEMPLATE;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.TEMP_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(sigla))
            {
                query = query.Where(p => p.TEMP_SG_SIGLA.Contains(sigla));
            }
            if (!String.IsNullOrEmpty(conteudo))
            {
                query = query.Where(p => p.TEMP_TX_CONTEUDO_LIMPO.Contains(conteudo));
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.TEMP_SG_SIGLA);
                lista = query.ToList<TEMPLATE>();
            }
            return lista;
        }
    }
}
 