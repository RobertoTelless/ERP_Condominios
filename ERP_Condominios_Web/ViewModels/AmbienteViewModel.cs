using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AmbienteViewModel
    {
        [Key]
        public int AMBI_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public int TIAM_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string AMBI_NM_AMBIENTE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public int AMBI_NR_LOTACAO { get; set; }
        [StringLength(1000, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 1000 caracteres.")]
        public string AMBI_NM_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo CHAVE obrigatorio")]
        public int AMBI_IN_CHAVE { get; set; }
        [StringLength(250, ErrorMessage = "O ARQUIVO DE FOTO deve conter no máximo 250 caracteres.")]
        public string AMBI_AQ_FOTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> AMBI_NR_DIAS_CHAVE { get; set; }
        [Required(ErrorMessage = "Campo GRATUITO obrigatorio")]
        public Nullable<int> AMBI_IN_GRATUITO { get; set; }
        [StringLength(250, ErrorMessage = "O ARQUIVO DE NORMAS deve conter no máximo 250 caracteres.")]
        public string AMBI_AQ_NORMAS_USO { get; set; }
        [StringLength(250, ErrorMessage = "O ARQUIVO DE LISTA deve conter no máximo 250 caracteres.")]
        public string AMBI_AQ_LISTA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime AMBI_DT_CADASTRO { get; set; }
        public int AMBI_IN_ATIVO { get; set; }
        [StringLength(10, ErrorMessage = "A HORA INICIAL deve conter no máximo 10 caracteres.")]
        public string AMBI_HR_INICIO { get; set; }
        [StringLength(10, ErrorMessage = "A HORA FINAL deve conter no máximo 10 caracteres.")]
        public string AMBI_HR_FINAL { get; set; }

        public bool Gratuito
        {
            get
            {
                if (AMBI_IN_GRATUITO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                AMBI_IN_GRATUITO = (value == true) ? 1 : 0;
            }
        }
        public bool Chave
        {
            get
            {
                if (AMBI_IN_CHAVE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                AMBI_IN_CHAVE = (value == true) ? 1 : 0;
            }
        }


        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AMBIENTE_CHAVE> AMBIENTE_CHAVE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AMBIENTE_CUSTO> AMBIENTE_CUSTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AMBIENTE_FINALIDADE> AMBIENTE_FINALIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AMBIENTE_IMAGEM> AMBIENTE_IMAGEM { get; set; }
        public virtual TIPO_AMBIENTE TIPO_AMBIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESERVA> RESERVA { get; set; }
    }
}