using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ControleVeiculoViewModel
    {
        [Key]
        public int COVE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE VEÍCULO obrigatorio")]
        public Nullable<int> TIVE_CD_ID { get; set; }
        public Nullable<int> UNID_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE ENTRADA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DE ENTRADA Deve ser uma data válida")]
        public Nullable<System.DateTime> COVE_DT_ENTRADA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE SAÍDA Deve ser uma data válida")]
        public Nullable<System.DateTime> COVE_DT_SAIDA { get; set; }
        [Required(ErrorMessage = "Campo PLACA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A PLACA deve conter no minimo 1 e no máximo 10 caracteres.")]
        public string COVE_NM_PLACA { get; set; }
        [StringLength(50, ErrorMessage = "A MARCA deve conter no máximo 50 caracteres.")]
        public string COVE_NM_MARCA { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 500 caracteres.")]
        public string COVE_DS_DESCRICAO { get; set; }
        public int COVE_IN_ATIVO { get; set; }
        [StringLength(50, ErrorMessage = "O MODELOdeve conter no máximo 50 caracteres.")]
        public string COVE_NM_MODELO { get; set; }
        [StringLength(50, ErrorMessage = "A COR deve conter no máximo 50 caracteres.")]
        public string COVE_NM_COR { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE PREVISTA Deve ser uma data válida")]
        public Nullable<System.DateTime> COVE_DT_PREVISAO_SAIDA { get; set; }
        [StringLength(250, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 250 caracteres.")]
        public string COVE_DS_JUSTIFICATIVA { get; set; }
        public Nullable<int> FORN_CD_ID { get; set; }
        [StringLength(50, ErrorMessage = "O NOME DO MOTORISTA/EMPRESA deve conter no máximo 50 caracteres.")]
        public string COVE_NM_PESSOA { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual TIPO_VEICULO TIPO_VEICULO { get; set; }
        public virtual UNIDADE UNIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTROLE_VEICULO_ACOMPANHAMENTO> CONTROLE_VEICULO_ACOMPANHAMENTO { get; set; }
        public virtual FORNECEDOR FORNECEDOR { get; set; }

    }
}