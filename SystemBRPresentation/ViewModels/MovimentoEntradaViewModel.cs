using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SystemBRPresentation.ViewModels
{
    public class MovimentoEntradaViewModel
    {
        public PRODUTO produto { get; set; }
        public MATERIA_PRIMA insumo { get; set; }
        public MOVIMENTO_ESTOQUE_PRODUTO mvmtProduto { get; set; }
        public MOVIMENTO_ESTOQUE_MATERIA_PRIMA mvmtMateria { get; set; }
        public List<MOVIMENTO_ESTOQUE_PRODUTO> listaMvmtProduto { get; set; }
        public List<MOVIMENTO_ESTOQUE_MATERIA_PRIMA> listaMvmtMateria { get; set; }
    }
}