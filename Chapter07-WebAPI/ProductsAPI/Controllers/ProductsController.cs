using Microsoft.AspNetCore.Mvc;
using ProductsAPI.DTOs;
using ProductsAPI.Interfaces;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    /// <summary>
    /// API Controller สำหรับจัดการสินค้า
    /// ใช้ best practices: DTOs, Service Layer, Async/Await, Proper HTTP Status Codes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// ดึงข้อมูลสินค้าทั้งหมด
        /// </summary>
        /// <returns>รายการสินค้าทั้งหมด</returns>
        /// <response code="200">ดึงข้อมูลสำเร็จ</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponseDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponseDto>>>> GetAllProducts()
        {
            _logger.LogInformation("GET /api/products - ดึงข้อมูลสินค้าทั้งหมด");
            
            var products = await _productService.GetAllProductsAsync();
            var response = new ApiResponse<IEnumerable<ProductResponseDto>>(
                products,
                "ดึงข้อมูลสินค้าสำเร็จ"
            );

            return Ok(response);
        }

        /// <summary>
        /// ดึงข้อมูลสินค้าตาม ID
        /// </summary>
        /// <param name="id">รหัสสินค้า</param>
        /// <returns>ข้อมูลสินค้า</returns>
        /// <response code="200">ดึงข้อมูลสำเร็จ</response>
        /// <response code="404">ไม่พบสินค้า</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetProductById(int id)
        {
            _logger.LogInformation("GET /api/products/{Id} - ดึงข้อมูลสินค้า", id);
            
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                var errorResponse = new ApiErrorResponse(
                    StatusCodes.Status404NotFound,
                    $"ไม่พบสินค้า ID: {id}"
                );
                return NotFound(errorResponse);
            }

            var response = new ApiResponse<ProductResponseDto>(
                product,
                "ดึงข้อมูลสินค้าสำเร็จ"
            );

            return Ok(response);
        }

        /// <summary>
        /// สร้างสินค้าใหม่
        /// </summary>
        /// <param name="createDto">ข้อมูลสินค้าที่ต้องการสร้าง</param>
        /// <returns>ข้อมูลสินค้าที่สร้างใหม่</returns>
        /// <response code="201">สร้างสินค้าสำเร็จ</response>
        /// <response code="400">ข้อมูลไม่ถูกต้อง</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> CreateProduct(
            [FromBody] CreateProductDto createDto)
        {
            _logger.LogInformation("POST /api/products - สร้างสินค้าใหม่");

            // Validation จะถูกทำโดย FluentValidation อัตโนมัติ
            // ถ้า validation ไม่ผ่าน จะ return 400 BadRequest พร้อม error messages
            
            var product = await _productService.CreateProductAsync(createDto);
            var response = new ApiResponse<ProductResponseDto>(
                product,
                "สร้างสินค้าสำเร็จ"
            );

            // ส่ง status 201 Created พร้อม Location header ที่ชี้ไปยัง resource ที่สร้างใหม่
            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                response
            );
        }

        /// <summary>
        /// แก้ไขข้อมูลสินค้า
        /// </summary>
        /// <param name="id">รหัสสินค้า</param>
        /// <param name="updateDto">ข้อมูลสินค้าที่ต้องการแก้ไข</param>
        /// <returns>ข้อมูลสินค้าที่แก้ไขแล้ว</returns>
        /// <response code="200">แก้ไขสินค้าสำเร็จ</response>
        /// <response code="400">ข้อมูลไม่ถูกต้อง</response>
        /// <response code="404">ไม่พบสินค้า</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductResponseDto>>> UpdateProduct(
            int id,
            [FromBody] UpdateProductDto updateDto)
        {
            _logger.LogInformation("PUT /api/products/{Id} - แก้ไขสินค้า", id);

            var product = await _productService.UpdateProductAsync(id, updateDto);
            
            if (product == null)
            {
                var errorResponse = new ApiErrorResponse(
                    StatusCodes.Status404NotFound,
                    $"ไม่พบสินค้า ID: {id}"
                );
                return NotFound(errorResponse);
            }

            var response = new ApiResponse<ProductResponseDto>(
                product,
                "แก้ไขสินค้าสำเร็จ"
            );

            return Ok(response);
        }

        /// <summary>
        /// ลบสินค้า
        /// </summary>
        /// <param name="id">รหัสสินค้า</param>
        /// <returns>ผลการลบ</returns>
        /// <response code="200">ลบสินค้าสำเร็จ</response>
        /// <response code="404">ไม่พบสินค้า</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteProduct(int id)
        {
            _logger.LogInformation("DELETE /api/products/{Id} - ลบสินค้า", id);

            var result = await _productService.DeleteProductAsync(id);
            
            if (!result)
            {
                var errorResponse = new ApiErrorResponse(
                    StatusCodes.Status404NotFound,
                    $"ไม่พบสินค้า ID: {id}"
                );
                return NotFound(errorResponse);
            }

            var response = new ApiResponse<object>("ลบสินค้าสำเร็จ");
            return Ok(response);
        }

        /// <summary>
        /// ค้นหาสินค้า
        /// </summary>
        /// <param name="keyword">คำค้นหา</param>
        /// <returns>รายการสินค้าที่ค้นพบ</returns>
        /// <response code="200">ค้นหาสำเร็จ</response>
        /// <response code="400">คำค้นหาไม่ถูกต้อง</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponseDto>>>> SearchProducts(
            [FromQuery] string keyword)
        {
            _logger.LogInformation("GET /api/products/search?keyword={Keyword} - ค้นหาสินค้า", keyword);

            if (string.IsNullOrWhiteSpace(keyword))
            {
                var errorResponse = new ApiErrorResponse(
                    StatusCodes.Status400BadRequest,
                    "กรุณาระบุคำค้นหา"
                );
                return BadRequest(errorResponse);
            }

            var products = await _productService.SearchProductsAsync(keyword);
            var response = new ApiResponse<IEnumerable<ProductResponseDto>>(
                products,
                $"พบสินค้า {products.Count()} รายการ"
            );

            return Ok(response);
        }

        /// <summary>
        /// ดึงสินค้าตามหมวดหมู่
        /// </summary>
        /// <param name="category">หมวดหมู่สินค้า</param>
        /// <returns>รายการสินค้าในหมวดหมู่</returns>
        /// <response code="200">ดึงข้อมูลสำเร็จ</response>
        [HttpGet("category/{category}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponseDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponseDto>>>> GetProductsByCategory(
            string category)
        {
            _logger.LogInformation("GET /api/products/category/{Category} - ดึงสินค้าตามหมวดหมู่", category);

            var products = await _productService.GetProductsByCategoryAsync(category);
            var response = new ApiResponse<IEnumerable<ProductResponseDto>>(
                products,
                $"พบสินค้าในหมวดหมู่ {category} จำนวน {products.Count()} รายการ"
            );

            return Ok(response);
        }
    }
}
