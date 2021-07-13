using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class ProdutoViewModel
    {
        [Key]
        public int PROD_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> MATR_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public Nullable<int> CAPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public Nullable<int> UNID_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SUBCATEGORIA obrigatorio")]
        public Nullable<int> SCPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 200.")]
        public string PROD_NM_NOME { get; set; }
        [StringLength(1000, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 1000.")]
        public string PROD_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE MÍNIMA obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int PROD_QN_QUANTIDADE_MINIMA { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE INICIAL obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int PROD_QN_QUANTIDADE_INICIAL { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE ESTOQUE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int PROD_QN_ESTOQUE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PROD_DT_ULTIMA_MOVIMENTACAO { get; set; }
        [Required(ErrorMessage = "Campo AVISO DE MÍNIMA obrigatorio")]
        public int PROD_IN_AVISA_MINIMO { get; set; }
        public System.DateTime PROD_DT_CADASTRO { get; set; }
        public int PROD_IN_ATIVO { get; set; }
        public string PROD_AQ_FOTO { get; set; }
        public Nullable<int> PROD_IN_COMPOSTO { get; set; }
        [StringLength(10, ErrorMessage = "O CÓDIGO deve conter no minimo 1 caracteres e no máximo 10.")]
        public String PROD_CD_CODIGO { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PRODUTO obrigatorio")]
        public Nullable<int> PROD_IN_TIPO_PRODUTO { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_VENDA { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_PROMOCAO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES devem conter no máximo 1000.")]
        public string PROD_DS_INFORMACOES { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_NR_GARANTIA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_QN_QUANTIDADE_MAXIMA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_QN_RESERVA_ESTOQUE { get; set; }
        [StringLength(50, ErrorMessage = "AS REFERENCIAS devem conter no máximo 50.")]
        public string PROD_NR_REFERENCIA { get; set; }
        public string PROD_NM_ORIGEM { get; set; }
        [StringLength(50, ErrorMessage = "AS INFORMAÇÕES devem conter no máximo 50.")]
        public string PROD_NM_LOCALIZACAO_ESTOQUE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_QN_PESO_BRUTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_QN_PESO_LIQUIDO { get; set; }
        public Nullable<int> PROD_IN_TIPO_EMBALAGEM { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_NR_LARGURA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_NR_COMPRIMENTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_NR_ALTURA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_NR_DIAMETRO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_CUSTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_MINIMO { get; set; }
        public string PROD_TX_OBSERVACOES { get; set; }
        [StringLength(50, ErrorMessage = "A MARCA deve conter no máximo 50.")]
        public string PROD_NM_MARCA { get; set; }
        [StringLength(50, ErrorMessage = "O MODELO deve conter no máximo 50.")]
        public string PROD_NM_MODELO { get; set; }
        [StringLength(50, ErrorMessage = "A REFERENCIA deve conter no máximo 50.")]
        public string PROD_NM_REFERENCIA_FABRICANTE { get; set; }
        [StringLength(50, ErrorMessage = "O FABRICANTE deve conter no máximo 50.")]
        public string PROD_NM_FABRICANTE { get; set; }
        [StringLength(50, ErrorMessage = "O CÓDIGO DE BARRAS deve conter no máximo 50.")]
        public string PROD_NR_BARCODE { get; set; }
        public string PROD_QR_QRCODE { get; set; }
        public Nullable<int> PROD_IN_BALANCA_PDV { get; set; }
        public Nullable<int> PROD_IN_BALANCA_RETAGUARDA { get; set; }
        public Nullable<int> PROD_NR_DIAS_VALIDADE { get; set; }
        public Nullable<int> PROD_IN_TIPO_COMBO { get; set; }
        public Nullable<int> PROD_IN_OPCAO_COMBO { get; set; }
        public Nullable<int> PROD_IN_DIVISAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_NR_VALIDADE { get; set; }
        public Nullable<int> PROD_IN_COBRAR_MAIOR { get; set; }
        [StringLength(5000, ErrorMessage = "O INFORMAÇÃO NUTRIOCIONAL deve conter no máximo 5000.")]
        public string PROD_DS_INFORMACAO_NUTRICIONAL { get; set; }
        public Nullable<int> PROD_IN_GERAR_ARQUIVO { get; set; }
        [StringLength(50, ErrorMessage = "O NÚMERO NVM deve conter no máximo 50.")]
        public string PROD_NR_NCM { get; set; }
        [StringLength(14, ErrorMessage = "O GTIN EAN deve conter no máximo 14.")]
        public string PROD_CD_GTIN_EAN { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_PC_MARKUP_MININO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_MARKUP_PADRAO { get; set; }
        public string PROD_DS_JUSTIFICATIVA { get; set; }

        public Nullable<int> PROR_CD_ID { get; set; }
        [StringLength(50, ErrorMessage = "O CÓDIGO CEST deve conter no máximo 50.")]
        public string PROD_NR_CEST { get; set; }
        [StringLength(14, ErrorMessage = "O GTIN EAN TRIBUTÁVEL deve conter no máximo 14.")]
        public string PROD_NR_GTIN_EAN_TRIB { get; set; }
        [StringLength(50, ErrorMessage = "A UNIDADE TRIBUTÁVEL deve conter no máximo 50.")]
        public string PROD_NM_UNIDADE_TRIB { get; set; }
        [StringLength(50, ErrorMessage = "O FATOR DE CONVERSÃO deve conter no máximo 50.")]
        public string PROD_NR_FATOR_CONVERSAO { get; set; }
        [StringLength(50, ErrorMessage = "O CÓDIGO DE ENQUADRAMENTO IPI deve conter no máximo 50.")]
        public string PROD_NR_ENQUADRE_IPI { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_IPI_FIXO { get; set; }

        [StringLength(50, ErrorMessage = "O SLUG deve conter no máximo 50.")]
        public string PROD_NM_SLUG { get; set; }
        [StringLength(250, ErrorMessage = "AS KEYWORDS devem conter no máximo 250.")]
        public string PROD_NM_KEYWORDS { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO SEO deve conter no máximo 50.")]
        public string PROD_NM_TITULO_SEO { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO SEO deve conter no máximo 500.")]
        public string PROD_DS_DESCRICAO_CEO { get; set; }
        [StringLength(50, ErrorMessage = "A TAG deve conter no máximo 50.")]
        public string PROD_NR_TAG { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PRTP_VL_PRECO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PRTP_VL_PRECO_PROMOCAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PRTP_VL_DESCONTO_MAXIMO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_QN_NOVA_CONTAGEM { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_QN_CONTAGEM { get; set; }
        public Nullable<decimal> PRTP_VL_CUSTO { get; set; }

        public bool AvisaMinima
        {
            get
            {
                if (PROD_IN_AVISA_MINIMO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_AVISA_MINIMO = (value == true) ? 1 : 0;
            }
        }

        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PrecoVenda
        {
            get
            {
                return PROD_VL_PRECO_VENDA;
            }
            set
            {
                PROD_VL_PRECO_VENDA = value;
            }
        }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PrecoPromocao
        {
            get
            {
                return PROD_VL_PRECO_PROMOCAO;
            }
            set
            {
                PROD_VL_PRECO_PROMOCAO = value;
            }
        }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> Garantia
        {
            get
            {
                return PROD_NR_GARANTIA;
            }
            set
            {
                PROD_NR_GARANTIA = value;
            }
        }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> QuantidadeMaxima
        {
            get
            {
                return PROD_QN_QUANTIDADE_MAXIMA;
            }
            set
            {
                PROD_QN_QUANTIDADE_MAXIMA = value;
            }
        }
        public bool Composto
        {
            get
            {
                if (PROD_IN_COMPOSTO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_COMPOSTO = (value == true) ? 1 : 0;
            }
        }

        public bool BalancaPDV
        {
            get
            {
                if (PROD_IN_BALANCA_PDV == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_BALANCA_PDV = (value == true) ? 1 : 0;
            }
        }

        public bool BalancaRetaguarda
        {
            get
            {
                if (PROD_IN_BALANCA_RETAGUARDA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_BALANCA_RETAGUARDA = (value == true) ? 1 : 0;
            }
        }
        public bool ProdutoTipoCombo
        {
            get
            {
                if (PROD_IN_TIPO_COMBO-- == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_TIPO_COMBO = (value == true) ? 1 : 0;
            }
        }
        public bool ProdutoOpcaoCombo
        {
            get
            {
                if (PROD_IN_OPCAO_COMBO-- == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_OPCAO_COMBO = (value == true) ? 1 : 0;
            }
        }
        public bool CobrarMaior
        {
            get
            {
                if (PROD_IN_COBRAR_MAIOR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_COBRAR_MAIOR = (value == true) ? 1 : 0;
            }
        }
        public bool ArquivoTexto
        {
            get
            {
                if (PROD_IN_GERAR_ARQUIVO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PROD_IN_GERAR_ARQUIVO = (value == true) ? 1 : 0;
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATENDIMENTO> ATENDIMENTO { get; set; }
        public virtual CATEGORIA_PRODUTO CATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FICHA_TECNICA> FICHA_TECNICA { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ITEM_PEDIDO_COMPRA> ITEM_PEDIDO_COMPRA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ITEM_PEDIDO_VENDA> ITEM_PEDIDO_VENDA { get; set; }
        public virtual MATRIZ MATRIZ { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRECO_PRODUTO> PRECO_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ANEXO> PRODUTO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_FORNECEDOR> PRODUTO_FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_GRADE> PRODUTO_GRADE { get; set; }
        public virtual PRODUTO_ORIGEM PRODUTO_ORIGEM { get; set; }
        public virtual SUBCATEGORIA_PRODUTO SUBCATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_TABELA_PRECO> PRODUTO_TABELA_PRECO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ESTOQUE_FILIAL> PRODUTO_ESTOQUE_FILIAL { get; set; }
    }
}