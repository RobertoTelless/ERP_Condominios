using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public static class Formatters
    {
        public static String CPFFormatter(String CPF)
        {
            if (String.IsNullOrEmpty(CPF))
            {
                return CPF;
            }

            CPF = new String(CPF.Where(c => char.IsDigit(c)).ToArray());
            String cpf = String.Format(@"{0:\000\.000\.000\-00}", Convert.ToInt64(CPF));
            return cpf;
        }

        public static String CNPJFormatter(String CNPJ)
        {
            if (String.IsNullOrEmpty(CNPJ))
            {
                return CNPJ;
            }
            CNPJ = new String(CNPJ.Where(c => char.IsDigit(c)).ToArray());
            String cnpj = String.Format(@"{0:00\.000\.000\/0000\-00}", Convert.ToInt64(CNPJ));
            return cnpj;
        }

        public static String CEPFormatter(String CEP)
        {
            if (String.IsNullOrEmpty(CEP))
            {
                return CEP;
            }
            Decimal valorOriginal = 0;
            string valorFormatado = valorOriginal.ToString("#,0.00", new CultureInfo("pt-BR"));

            CEP = new String(CEP.Where(c => char.IsDigit(c)).ToArray());
            String cep = String.Format(@"{0:00000\-000}", Convert.ToInt64(CEP));
            return cep;
        }

        public static String DecimalFormatter(Decimal number)
        {
            String valorFormatado = number.ToString("#,0.00", new CultureInfo("pt-BR"));
            return valorFormatado;
        }

        public static String LongDateFormatter(DateTime data)
        {
            String frase = string.Empty;
            if (data.Day == DateTime.Now.Day)
            {
                frase += "Hoje ";
            }
            else if (data.Day == DateTime.Today.AddDays(-1).Day)
            {
                frase += "Ontem ";
            }
            frase += data.ToLongTimeString() + " " + data.ToShortDateString();

            return frase;
        }

        public static String LongDateFormatterShort(DateTime data, String hora)
        {
            String frase = string.Empty;
            if (data.Day == DateTime.Now.Day)
            {
                frase += "Hoje ";
            }
            else if (data.Day == DateTime.Today.AddDays(-1).Day)
            {
                frase += "Ontem ";
            }
            else if (data.Day < DateTime.Today.AddDays(-1).Day)
            {
                frase += DiffTimeFormatterTime(data, hora);
            }
            return frase;
        }

        public static String DiffTimeFormatter(DateTime data)
        {
            TimeSpan diff = DateTime.Now - data;
            String output = string.Format("{0}d {1}h {2}m atras", diff.Days, diff.Hours, diff.Minutes);
            return output;
        }

        public static String DiffTimeFormatterTime(DateTime data, String hora)
        {
            String dataCompleta = data.ToShortDateString() + " " + hora;
            DateTime dataTime = Convert.ToDateTime(dataCompleta);

            TimeSpan diff = DateTime.Now - dataTime;
            String output = string.Format("{0}d {1}h {2}m atras", diff.Days, diff.Hours, diff.Minutes);
            return output;
        }

        public static String TraduzMes(Int32 mes)
        {
            if (mes == 1)
            {
                return "Janeiro";
            }
            else if (mes == 2)
            {
                return "Fevereiro";
            }
            else if (mes == 3)
            {
                return "Março";
            }
            else if (mes == 4)
            {
                return "Abril";
            }
            else if (mes == 5)
            {
                return "Maio";
            }
            else if (mes == 6)
            {
                return "Junho";
            }
            else if (mes == 7)
            {
                return "Julho";
            }
            else if (mes == 8)
            {
                return "Agosto";
            }
            else if (mes == 9)
            {
                return "Setembro";
            }
            else if (mes == 10)
            {
                return "Outubro";
            }
            else if (mes == 11)
            {
                return "Novembro";
            }
            return "Dezembro";
        }
    }
}
