using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class FornecedorAppService : AppServiceBase<FORNECEDOR>, IFornecedorAppService
    {
        private readonly IFornecedorService _baseService;

        public FornecedorAppService(IFornecedorService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<FORNECEDOR> GetAllItens(Int32 idAss)
        {
            List<FORNECEDOR> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<UF> GetAllUF()
        {
            List<UF> lista = _baseService.GetAllUF();
            return lista;
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public List<FORNECEDOR> GetAllItensAdm(Int32 idAss)
        {
            List<FORNECEDOR> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public FORNECEDOR GetItemById(Int32 id)
        {
            FORNECEDOR item = _baseService.GetItemById(id);
            return item;
        }

        public FORNECEDOR GetByEmail(String email)
        {
            FORNECEDOR item = _baseService.GetByEmail(email);
            return item;
        }

        public FORNECEDOR CheckExist(FORNECEDOR conta, Int32 idAss)
        {
            FORNECEDOR item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_FORNECEDOR> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_FORNECEDOR> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public FORNECEDOR_ANEXO GetAnexoById(Int32 id)
        {
            FORNECEDOR_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, String rede, Int32? ativo, Int32 idAss, out List<FORNECEDOR> objeto)
        {
            try
            {
                objeto = new List<FORNECEDOR>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, razao, nome, cpf, cnpj, email, cidade, uf, rede, ativo, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ExecuteFilterSemPedido(String nome, String cidade, Int32? uf, Int32 idAss, out List<FORNECEDOR> objeto)
        {
            try
            {
                objeto = new List<FORNECEDOR>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterSemPedido(nome, cidade, uf, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(FORNECEDOR item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.FORN_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.FORN_NR_DOCUMENTO = "-";

                // Checa endereço
                if (String.IsNullOrEmpty(item.FORM_NM_ENDERECO))
                {
                    item.FORM_NM_ENDERECO = "-";
                }
                if (String.IsNullOrEmpty(item.FORN_NM_BAIRRO))
                {
                    item.FORN_NM_BAIRRO = "-";
                }
                if (String.IsNullOrEmpty(item.FORN_NM_CIDADE))
                {
                    item.FORN_NM_CIDADE = "-";
                }
                if (String.IsNullOrEmpty(item.FORN_NR_CEP))
                {
                    item.FORN_NR_CEP = "-";
                }
                if (item.UF_CD_ID == null)
                {
                    item.UF_CD_ID = 28;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddFORN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FORNECEDOR>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FORNECEDOR item, FORNECEDOR itemAntes, USUARIO usuario)
        {
            try
            {
                // Checa endereço
                if (String.IsNullOrEmpty(item.FORM_NM_ENDERECO))
                {
                    item.FORM_NM_ENDERECO = "-";
                }
                if (String.IsNullOrEmpty(item.FORN_NM_BAIRRO))
                {
                    item.FORN_NM_BAIRRO = "-";
                }
                if (String.IsNullOrEmpty(item.FORN_NM_CIDADE))
                {
                    item.FORN_NM_CIDADE = "-";
                }
                if (String.IsNullOrEmpty(item.FORN_NR_CEP))
                {
                    item.FORN_NR_CEP = "-";
                }
                if (item.UF_CD_ID == null)
                {
                    item.UF_CD_ID = 28;
                }
                item.TIPO_PESSOA = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditFORN",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FORNECEDOR>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<FORNECEDOR>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FORNECEDOR item, FORNECEDOR itemAntes)
        {
            try
            {
                // Checa endereço
                if (String.IsNullOrEmpty(item.FORM_NM_ENDERECO))
                {
                    item.FORM_NM_ENDERECO = "-";
                }
                if (String.IsNullOrEmpty(item.FORN_NM_BAIRRO))
                {
                    item.FORN_NM_BAIRRO = "-";
                }
                if (String.IsNullOrEmpty(item.FORN_NM_CIDADE))
                {
                    item.FORN_NM_CIDADE = "-";
                }
                if (String.IsNullOrEmpty(item.FORN_NR_CEP))
                {
                    item.FORN_NR_CEP = "-";
                }
                item.TIPO_PESSOA = null;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(FORNECEDOR item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                //if (item.CONTA_PAGAR.Count > 0 || item.EQUIPAMENTO_MANUTENCAO.Count > 0)
                //{
                //    return 1;
                //}
               
                // Acerta campos
                item.FORN_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelFORN",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FORNECEDOR>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FORNECEDOR item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.FORN_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatFORN",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<FORNECEDOR>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            List<TIPO_PESSOA> lista = _baseService.GetAllTiposPessoa();
            return lista;
        }

        public FORNECEDOR_CONTATO GetContatoById(Int32 id)
        {
            FORNECEDOR_CONTATO lista = _baseService.GetContatoById(id);
            return lista;
        }

        public Int32 ValidateEditContato(FORNECEDOR_CONTATO item)
        {
            try
            {
                // Persiste
                return _baseService.EditContato(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateContato(FORNECEDOR_CONTATO item)
        {
            try
            {
                // Persiste
                item.FOCO_IN_ATIVO = 1;
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
