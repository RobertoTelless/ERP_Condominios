using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntitiesServices.Model;
using System.Web;

namespace SystemBRPresentation.ViewModels
{
    public class ContaBancariaViewModel
    {
        [Key]
        public int COBA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int BANC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE CONTA obrigatorio")]
        public int TICO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DA AGENCIA obrigatorio")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "O NÚMERO DA AGENCIA deve conter no minimo 3 caracteres e no máximo 10.")]
        public string COBA_NR_AGENCIA { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DA CONTA obrigatorio")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O NÚMERO DA CONTA deve conter no minimo 3 caracteres e no máximo 20.")]
        public string COBA_NR_CONTA { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DA AGENCIA DA CONTA deve conter no máximo 50.")]
        public string COBA_NM_AGENCIA { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DO GERENTE deve conter no máximo 50.")]
        public string COBA_NM_GERENTE { get; set; }
        [StringLength(50, ErrorMessage = "O TELEFONE deve conter no máximo 50.")]
        public string COBA_NR_TELEFONE { get; set; }
        [Required(ErrorMessage = "Campo DATA DE ABERTURA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> COBA_DT_ABERTURA { get; set; }
        public int COBA_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo SALDO INICIAL obrigatorio")]
        [RegularExpression(@"^[0-9]+([,][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> COBA_VL_SALDO_INICIAL { get; set; }
        public Nullable<decimal> COBA_VL_SALDO_ATUAL { get; set; }
        public string COBA_NM_NOME_EXIBE { get; set; }
        public Nullable<int> COBA_IN_CONTA_PADRAO { get; set; }
        public Nullable<int> COBA_IN_CONTA_SELECAO { get; set; }
        public Nullable<int> COBA_IN_DIAS { get; set; }
        public Nullable<System.DateTime> COBA_DT_SELECAO { get; set; }
        public Nullable<int> COBA_IN_TIPO_FILTRO { get; set; }
        public Nullable<int> COBA_IN_MES { get; set; }
        public Nullable<System.DateTime> COBA_DT_SELECAO_FINAL { get; set; }
        [Required(ErrorMessage = "Campo NOME DA CONTA obrigatorio")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "O NOME DA CONTA deve conter no minimo 1 caracteres e no máximo 30.")]
        public string COBA_NM_NOME { get; set; }
        public Nullable<int> MATR_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        public int COBA_IN_PRINCIPAL { get; set; }

        public Nullable<decimal> SaldoInicial
        {
            get
            {
                return COBA_VL_SALDO_INICIAL;
            }
        }
        public Nullable<decimal> SaldoAtual
        {
            get
            {
                return COBA_VL_SALDO_ATUAL;
            }
        }

        public bool Padrao
        {
            get
            {
                if (COBA_IN_CONTA_PADRAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                COBA_IN_CONTA_PADRAO = (value == true) ? 1 : 0;
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual BANCO BANCO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_BANCO_CONTATO> CONTA_BANCO_CONTATO { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_BANCO_LANCAMENTO> CONTA_BANCO_LANCAMENTO { get; set; }
        public virtual MATRIZ MATRIZ { get; set; }
        public virtual TIPO_CONTA TIPO_CONTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR> CONTA_PAGAR1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR_PARCELA> CONTA_PAGAR_PARCELA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMA_PAGAMENTO> FORMA_PAGAMENTO { get; set; }
    }
}