namespace WebApplication1.Models.ViewModels
{
    public class EmployeeView
    {
        public int Employee_Id { get; set; }

        public string Employee_Name { get; set; }

        public string salary { get; set; } = "Not Specified";

        public string DepartmentName { get; set; } = "No Department";

        public string HireDate { get; set; }

    }
}
