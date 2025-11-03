namespace StudentInfoLoginRoles.ViewModels
{
    public class AdminViewModels
    {
        public class UserRolesViewModel
        {
            public string UserId { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public IList<string> Roles { get; set; } = new List<string>();
        }

        public class EditRolesViewModel
        {
            public string UserId { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public List<RoleCheckbox> Roles { get; set; } = new List<RoleCheckbox>();
        }

        public class RoleCheckbox
        {
            public string RoleName { get; set; } = string.Empty;
            public bool IsSelected { get; set; }
        }

    }
}
