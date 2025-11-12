using FluentValidation;
using ProductsAPI.DTOs;

namespace ProductsAPI.Validators
{
    /// <summary>
    /// Validator สำหรับการสร้างสินค้าใหม่
    /// ใช้ FluentValidation เพื่อความยืดหยุ่นและอ่านง่ายกว่า Data Annotations
    /// </summary>
    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            // ตรวจสอบชื่อสินค้า
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("ชื่อสินค้าจำเป็นต้องระบุ")
                .MaximumLength(200).WithMessage("ชื่อสินค้าต้องไม่เกิน 200 ตัวอักษร")
                .MinimumLength(3).WithMessage("ชื่อสินค้าต้องมีอย่างน้อย 3 ตัวอักษร");

            // ตรวจสอบคำอธิบาย (ถ้ามี)
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("คำอธิบายต้องไม่เกิน 1000 ตัวอักษร")
                .When(x => !string.IsNullOrEmpty(x.Description));

            // ตรวจสอบราคา
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("ราคาต้องมากกว่า 0")
                .LessThanOrEqualTo(1000000).WithMessage("ราคาต้องไม่เกิน 1,000,000");

            // ตรวจสอบจำนวนสินค้า
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("จำนวนสินค้าต้องมากกว่าหรือเท่ากับ 0")
                .LessThanOrEqualTo(100000).WithMessage("จำนวนสินค้าต้องไม่เกิน 100,000");

            // ตรวจสอบหมวดหมู่
            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("หมวดหมู่สินค้าจำเป็นต้องระบุ")
                .MaximumLength(100).WithMessage("หมวดหมู่ต้องไม่เกิน 100 ตัวอักษร")
                .Must(BeValidCategory).WithMessage("หมวดหมู่ไม่ถูกต้อง ต้องเป็น: Electronics, Clothing, Books, Food, Toys, Other");
        }

        // Custom validation rule สำหรับหมวดหมู่
        private bool BeValidCategory(string category)
        {
            var validCategories = new[] { "Electronics", "Clothing", "Books", "Food", "Toys", "Other" };
            return validCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Validator สำหรับการแก้ไขสินค้า
    /// </summary>
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("ชื่อสินค้าจำเป็นต้องระบุ")
                .MaximumLength(200).WithMessage("ชื่อสินค้าต้องไม่เกิน 200 ตัวอักษร")
                .MinimumLength(3).WithMessage("ชื่อสินค้าต้องมีอย่างน้อย 3 ตัวอักษร");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("คำอธิบายต้องไม่เกิน 1000 ตัวอักษร")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("ราคาต้องมากกว่า 0")
                .LessThanOrEqualTo(1000000).WithMessage("ราคาต้องไม่เกิน 1,000,000");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("จำนวนสินค้าต้องมากกว่าหรือเท่ากับ 0")
                .LessThanOrEqualTo(100000).WithMessage("จำนวนสินค้าต้องไม่เกิน 100,000");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("หมวดหมู่สินค้าจำเป็นต้องระบุ")
                .MaximumLength(100).WithMessage("หมวดหมู่ต้องไม่เกิน 100 ตัวอักษร")
                .Must(BeValidCategory).WithMessage("หมวดหมู่ไม่ถูกต้อง ต้องเป็น: Electronics, Clothing, Books, Food, Toys, Other");
        }

        private bool BeValidCategory(string category)
        {
            var validCategories = new[] { "Electronics", "Clothing", "Books", "Food", "Toys", "Other" };
            return validCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
        }
    }
}
