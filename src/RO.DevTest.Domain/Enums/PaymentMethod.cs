using System.ComponentModel;

namespace RO.DevTest.Domain.Enums;

public enum PaymentMethod
{
    [Description("Dinheiro")]
    Cash = 1,
    [Description("PIX")]
    Pix = 2,
    [Description("Cartão de Débito")]
    DebitCard = 3,
    [Description("Cartão de Crédito")]
    CreditCard = 4
}