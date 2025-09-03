namespace QRbasedFoodOrdering.Models
{
    
        public class RoleAssignment
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public bool IsChecked { get; set; }
        }
        public class UserRoleViewModel
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public List<RoleAssignment> UserRoles { get; set; }

            public UserRoleViewModel()
            {
                this.UserRoles = new List<RoleAssignment>();
            }
        }
    }

