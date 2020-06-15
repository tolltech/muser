using System;
using System.Linq;
using System.Text;

namespace Tolltech.MuserUI.Kontur.PaymentOrder
{
    public class PaymentOrderConverter : IPaymentOrderConverter
    {
        public string Convert(string src)
        {
            if (src == null)
            {
                return null;
            }

            src = src.Replace("\"", string.Empty);
            var elements = src.Split("  ").Skip(5);

            var dictionaryOfElements1 = elements
                .Select(x => x.Split('=')).ToArray();

            var dictionaryOfElements =
                dictionaryOfElements1.ToDictionary(x => x[0], x => x.Length == 2 ? x[1] : string.Empty);

            var sb = new StringBuilder();

            sb.AppendLine("{");
            sb.AppendLine("\"IsBudgetPayment\":false,");
            sb.AppendLine($"\"Number\":\"{dictionaryOfElements["Номер"]}\",");
            sb.AppendLine($"\"Date\":\"{DateTime.Now.Date:yyyy-MM-dd}\",");
            sb.AppendLine($"\"Sum\":{dictionaryOfElements["Сумма"]},");
            sb.AppendLine($"\"RecipientName\":\"{dictionaryOfElements["Получатель"]}\",");
            sb.AppendLine($"\"RecipientInn\":\"{dictionaryOfElements["ПолучательИНН"]}\",");
            sb.AppendLine($"\"RecipientKpp\":\"{dictionaryOfElements["ПолучательКПП"]}\",");
            sb.AppendLine($"\"RecipientAccount\":\"{dictionaryOfElements["ПолучательСчет"]}\",");
            sb.AppendLine($"\"RecipientCorrAccount\":\"{dictionaryOfElements["ПолучательКорсчет"]}\",");
            sb.AppendLine($"\"RecipientBankBik\":\"{dictionaryOfElements["ПолучательБИК"]}\",");
            sb.AppendLine($"\"PayerName\":\"{dictionaryOfElements["Плательщик"]}\",");
            sb.AppendLine($"\"PayerInn\":\"{dictionaryOfElements["ПлательщикИНН"]}\",");
            sb.AppendLine($"\"PayerKpp\":\"{dictionaryOfElements["ПлательщикКПП"]}\",");
            sb.AppendLine($"\"PayerAccount\":\"{dictionaryOfElements["ПлательщикСчет"]}\",");
            sb.AppendLine($"\"PayerBik\":\"{dictionaryOfElements["ПлательщикБИК"]}\",");
            sb.AppendLine($"\"PayerCorrAccount\":\"{dictionaryOfElements["ПлательщикКорсчет"]}\",");
            sb.AppendLine($"\"PaymentPurpose\":\"{dictionaryOfElements["НазначениеПлатежа"]}\",");
            sb.AppendLine($"\"Priority\":{dictionaryOfElements["Очередность"]},");

            var vatAmount = GetVatAmountFromPurpose(dictionaryOfElements["НазначениеПлатежа"]);
            var vatRate = vatAmount == 0 ? 0 : 20;

            sb.AppendLine($"\"NDSRate\":{vatRate},");
            sb.AppendLine($"\"NDSAmount\":{vatAmount}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static decimal GetVatAmountFromPurpose(string purpose)
        {
            if (purpose.Contains("Без НДС"))
                return 0;

            var words = purpose.Split(' ').ToArray();
            var indexOfVat = Array.IndexOf(words, "НДС");
            var indexOfRubles = indexOfVat + 1;
            var indexOfKopecks = indexOfVat + 3;

            var rubles = 0;
            var kopecks = 0;
            if (!int.TryParse(words[indexOfRubles], out rubles))
            {
                return 0;
            }

            if (!int.TryParse(words[indexOfKopecks], out kopecks))
            {
                return rubles;
            }

            return rubles + kopecks / 100m;
        }
    }
}