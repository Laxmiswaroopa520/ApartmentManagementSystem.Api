using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Enums
{

    public enum ComplaintStatus
    {
        Raised = 1,
        Assigned = 2,
        InProgress = 3,
        Resolved = 4,
        AwaitingApproval = 5,
        Closed = 6,
        Rejected = 7
    }
}