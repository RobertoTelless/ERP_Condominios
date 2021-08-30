using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ProdutoViewModel
    {
        [Key]
        public int PROD_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public Nullable<int> CAPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SUBCATEGORIA obrigatorio")]
        public Nullable<int> SCPR_CD_ID { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 200 caracteres.")]
        public string PROD_NM_NOME { get; set; }
        [StringLength(200, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 200 caracteres.")]
        public string PROD_DS_DESCRICAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int PROD_QN_QUANTIDADE_MINIMA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int PROD_QN_QUANTIDADE_INICIAL { get; set; }
        [Required(ErrorMessage = "Campo ESTOQUE obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int PROD_QN_ESTOQUE { get; set; }
        public Nullable<System.DateTime> PROD_DT_ULTIMA_MOVIMENTACAO { get; set; }
        public int PROD_IN_AVISA_MINIMO { get; set; }
        public System.DateTime PROD_DT_CADASTRO { get; set; }
        public int PROD_IN_ATIVO { get; set; }
        public string PROD_AQ_FOTO { get; set; }
        [StringLength(10, MinimumLength = 1, ErrorMessage = "O CÓDIGO deve conter no minimo 1 caracteres e no máximo 10 caracteres.")]
        public string PROD_CD_CODIGO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_VENDA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_PRECO_PROMOCAO { get; set; }
        [StringLength(1000, ErrorMessage = "A INFORMAÇÃO deve conter no máximo 1000 caracteres.")]
        public string PROD_DS_INFORMACOES { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_QN_QUANTIDADE_MAXIMA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_QN_RESERVA_ESTOQUE { get; set; }
        [StringLength(50, ErrorMessage = "A REFERÊNCIA deve conter no máximo 50 caracteres.")]
        public string PROD_NR_REFERENCIA { get; set; }
        [StringLength(50, ErrorMessage = "A LOCALIZAÇÃO NO ESTOQUE deve conter no máximo 50 caracteres.")]
        public string PROD_NM_LOCALIZACAO_ESTOQUE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PROD_VL_CUSTO { get; set; }
        [StringLength(1000, ErrorMessage = "AS OBSERVAÇÕES devem conter no máximo 1000 caracteres.")]
        public string PROD_TX_OBSERVACOES { get; set; }
        [StringLength(50, ErrorMessage = "A MARCA deve conter no máximo 50 caracteres.")]
        public string PROD_NM_MARCA { get; set; }
        [StringLength(50, ErrorMessage = "O MODELO deve conter no máximo 50 caracteres.")]
        public string PROD_NM_MODELO { get; set; }
        [StringLength(50, ErrorMessage = "A REFERÊNCIA deve conter no máximo 50 caracteres.")]
        public string PROD_NM_REFERENCIA_FABRICANTE { get; set; }
        [StringLength(50, ErrorMessage = "O FABRICANTE deve conter no máximo 50 caracteres.")]
        public string PROD_NM_FABRICANTE { get; set; }
        [StringLength(20, ErrorMessage = "O CÓDIGO DE BARRAS deve conter no máximo 20 caracteres.")]
        public string PROD_NR_BARCODE { get; set; }
        [StringLength(100, ErrorMessage = "O QR CODE deve conter no máximo 100 caracteres.")]
        public string PROD_QR_QRCODE { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500 caracteres.")]
        public string PROD_DS_JUSTIFICATIVA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_QN_NOVA_CONTAGEM { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PROD_QN_CONTAGEM { get; set; }
        public Nullable<int> UNMA_CD_ID { get; set; }

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

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_PRODUTO CATEGORIA_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ANEXO> PRODUTO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_FORNECEDOR> PRODUTO_FORNECEDOR { get; set; }
        public virtual SUBCATEGORIA_PRODUTO SUBCATEGORIA_PRODUTO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual UNIDADE_MATERIAL UNIDADE_MATERIAL { get; set; }
    }
}