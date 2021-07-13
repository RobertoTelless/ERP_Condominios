using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBR_Presentation.ViewModels
{
    public class InsumoViewModel
    {
        [Key]
        public int MAPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> MATR_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public Nullable<int> CAMA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo UNIDADE obrigatorio")]
        public int UNID_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string MAPR_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "A DESCRIÇÃO deve conter no minimo 1 caracteres e no máximo 1000.")]
        public string MAPR_DS_DESCRICAO { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int MAPR_QN_QUANTIDADE_M { get; set; }
        [Required(ErrorMessage = "Campo QUANTIDADE INICIAL obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int MAPR_QN_QUANTIDADE_INICIAL { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int MAPR_QN_ESTOQUE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> MAPR_DT_ULTIMA_MOVIMENTACAO { get; set; }
        public int MAPR_IN_AVISA_MINIMO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime MAPR_DT_CADASTRO { get; set; }
        public int MAPR_IN_ATIVO { get; set; }
        public Nullable<int> SCMA_CD_ID { get; set; }
        public string MAPR_CD_CODIGO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> MAPR_DT_VALIDADE { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MAPR_QN_ESTOQUE_INICIAL { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MAPR_QN_ESTOQUE_MAXIMO { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MAPR_QN_ESTOQUE_MINIMO { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MAPR_QN_RESERVA_ESTOQUE { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MAPR_PERDA_PROCESSAMENTO { get; set; }
        public string MAPR_AQ_FOTO { get; set; }
        public string MAPR_TX_OBSERVACOES { get; set; }
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MAPR_NR_FATOR_CONVERSAO { get; set; }
        [StringLength(50, ErrorMessage = "A REFERENCIA deve conter no máximo 50.")]
        public string MAPR_NM_REFERENCIA { get; set; }
        [Required(ErrorMessage = "Campo CUSTO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MAPR_VL_CUSTO { get; set; }

        public bool AvisaMinima
        {
            get
            {
                if (MAPR_IN_AVISA_MINIMO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                MAPR_IN_AVISA_MINIMO = (value == true) ? 1 : 0;
            }
        }

        public virtual CATEGORIA_MATERIA CATEGORIA_MATERIA { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATERIA_PRIMA_ANEXO> MATERIA_PRIMA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATERIA_PRIMA_FORNECEDOR> MATERIA_PRIMA_FORNECEDOR { get; set; }
        public virtual MATRIZ MATRIZ { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_MATERIA_PRIMA> MOVIMENTO_ESTOQUE_MATERIA_PRIMA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FICHA_TECNICA_DETALHE> FICHA_TECNICA_DETALHE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ITEM_PEDIDO_COMPRA> ITEM_PEDIDO_COMPRA { get; set; }
    }
}