using JetBrains.Annotations;

namespace Tolltech.MuserUI.Kontur.PaymentOrder
{
    public interface IPaymentOrderConverter
    {
        [CanBeNull] string Convert([CanBeNull] string src);
    }
}