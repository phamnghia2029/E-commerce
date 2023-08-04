﻿using API.Repositories;
using FluentValidation;

namespace API.Models.Requests.Post
{
    public class UpdateEmployeeRequest: BaseRequest
    {
        public int EmployeeId { get; set; }
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public int? DepartmentId { get; set; }
        public string? Title { get; set; }
        public string? TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateEmployeeRequestValidator : BaseValidator<UpdateEmployeeRequest>
    {
        private readonly DepartmentRepository _departmentRepository;

        public UpdateEmployeeRequestValidator(DepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        protected override void AddRules(UpdateEmployeeRequest request)
        {
            RuleFor(x => x.FirstName).MinimumLength(4).WithMessage("First name cần dài hơn 4 kí tự");
            RuleFor(x => x.LastName).MinimumLength(4).WithMessage("Last name cần dài hơn 4 kí tự");
            RuleFor(x => x.DepartmentId).Must(departmentId => _departmentRepository.IsExists(departmentId ?? -1)).WithMessage("Không tìm thấy department.");
        }
    }
}
