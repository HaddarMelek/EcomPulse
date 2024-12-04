using System;
using System.Collections.Generic;

namespace EcomPulse.Web.ViewModel;

public class UserVM
{
    public Guid UserId { get; set; }
    public List<OrderItemVM> Orders { get; set; } = new();
}