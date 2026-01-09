using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Enums
{

    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Overdue = 3,
        PartiallyPaid = 4
    }
}