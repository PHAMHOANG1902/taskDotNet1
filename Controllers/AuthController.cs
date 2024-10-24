using Microsoft.AspNetCore.Mvc;
using ATMManagementApplication.Models;
using ATMManagementApplication.Data;
using System.Linq;

namespace ATMManagementApplication.Controllers{
    
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase{
        
        private readonly ATMContext _context;
        public AuthController(ATMContext context){
            _context = context;
            
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Customer login){
            var customer = _context.Customers
                .FirstOrDefault(c => c.Name == login.Name 
                                && c.Password == login.Password);
            if(customer == null){
                return Unauthorized("Invalid credentials");
            }

            return Ok(new {message = "Login successful", customerId = customer.CustomerId});

        }
        //dki ng dung
        [HttpPost("register")]
        public IActionResult Register([FromBody] Customer newCustomer){
             var existingCustomer = _context.Customers.FirstOrDefault(c => c.Name == newCustomer.Name);
             if(existingCustomer != null){
             return BadRequest("User with this name already exists");
            }

        _context.Customers.Add(newCustomer);
        _context.SaveChanges();

        return Ok(new { message = "Registration cuccessful", customerId = newCustomer.CustomerId});
        }
        //thay dop mk
        [HttpPost("change-password")]
        public IActionResult ChangePassword([Frombody] ChangePasswordRequest request) {
             var customer = _context.Customers.Find(request.CustomerId);
             if(customer == null)
                return NotFound("Customer not found");

            if(customer.Password != request.OldPassword)
                return BadRequest("Old password is incorrect");

            customer.Password = request.OldPassword;
            _context.SaveChanges();

            return Ok(new {message = "Password changed successfully"});
        }

}
