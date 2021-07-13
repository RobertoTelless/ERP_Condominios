using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SystemBRPresentation.ViewModels
{
    public class InventarioViewModel
    {
        public Int32? TIPO;

        public Int32? FILI_CD_ID_P { get; set; }
        public Int32? FILI_CD_ID_I { get; set; }

        public virtual PRODUTO PRODUTO { get; set; }
        public virtual MATERIA_PRIMA MATERIA_PRIMA { get; set; }


        public Int32? Tipo
        {
            get
            {
                if (PRODUTO != null)
                {
                    return 1;
                }
                else if (MATERIA_PRIMA != null)
                {
                    return 2;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                TIPO = value;
            }
        }
    }
}