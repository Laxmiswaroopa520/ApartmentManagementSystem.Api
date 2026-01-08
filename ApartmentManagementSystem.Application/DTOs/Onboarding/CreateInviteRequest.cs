using System;

public class CreateInviteRequest
{
    public string Email { get; set; }
    public string Mobile { get; set; }
    public Guid RoleId { get; set; }
}
