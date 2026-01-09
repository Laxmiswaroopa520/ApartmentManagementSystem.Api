using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Enums
{

    public enum PaymentMode
    {
        Cash = 1,
        BankTransfer = 2,
        UPI = 3,
        Cheque = 4,
        Card = 5
    }
}