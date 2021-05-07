using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Attributes
{
    public class CheckTimeAttributeMain  : ValidationAttribute
    {
        public CheckTimeAttributeMain() { }

        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;

            bool valido = UtilTimeMain.CheckTime(value.ToString());
            return valido;
        }
    }

    public static class UtilTimeMain
    {
        public static string RemoveNaoNumericos(string text)
        {
            if (text.Length > 8)
            {
                text = text.Substring(0, 8);
            }
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[^0-9]");
            string ret = reg.Replace(text, string.Empty);
            return ret;
        }

        public static bool CheckTime(string time)
        {
            // Retira mascara
            String timeLimpo = RemoveNaoNumericos(time);
            
            //Recupera horas
            String hora = timeLimpo.Substring(0, 2);
            Int32 horaNum = Convert.ToInt32(hora);
            if (horaNum > 23)
            {
                return false;
            }

            //Recupera minutos
            String minuto = timeLimpo.Substring(2, 2);
            Int32 MinutoNum = Convert.ToInt32(minuto);
            if (MinutoNum > 59)
            {
                return false;
            }
            return true;
        }
    }







}
