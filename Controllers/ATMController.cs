using Microsoft.AspNetCore.Mvc;
using ATMManagementApplication.Models;
using ATMManagementApplication.Data;
using System.Linq;
using System;

namespace ATMManagementApplication.Controllers{
    
    [ApiController]
    [Route("api/atm")]
    public class ATMController : ControllerBase{

        private readonly ATMContext _context;
        public ATMController(ATMContext context){
            _context = context;
        }
        //kiemtra so du
        [HttpGet("balance/{customerId}")]
        public IActionResult GetBalance(int customerId){
            var customer = _context.Customers.Find(customerId);
            if(customer == null) return NotFound("Customer not found");

            return Ok(new {balance = customer.Balance});

        }
        // rut tien
        [HttpPost("withdraw")]
        public IActionResult Withdraw([FromBody] WithdrawRequest request){
            var customer = _context.Customers.Find(request.CustomerId);
            if(customer==null) 
                return NotFound("Customer not found");

            if(customer.Balance < request.Amount)
                return BadRequest("Insufficient balance");
            
            customer.Balance -= request.Amount; 

            var transaction = new Transaction{
                CustomerId = request.CustomerId,
                Amount = request.Amount,
                Timestamp = DateTime.Now,
                IsSuccessful = true
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return Ok(new {message ="Withdraw successful",newBalance = customer.Balance});

        }
        //lich su giao dich
        [HttpGet("transactions/{customerId}")]
        public IActionResult GetTransactionHistory (int customerId) {
             var customer = _context.Customers.Find(customerId);
             if (customer == null) return NotFound("Customer not found!");

             var transactions = _context.Transactions
             .Where(transaction => transaction.CustomerId == customerId)
             .OrderByDescending(transaction => transaction.Timestamp);
             .ToList();

             return Ok(transactions);
        }
        //chuyen tien
        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferRequest request) {
            var sender = _context.Customers.Find(request.SenderId);
            var receiver = _context.Customers.Find(request.ReceiverId);

            if(sender == null || receiver == null)
                return NotFound("Sender or receiver not found");

            if(sender.Balance < request.Amount)
                return BadRequest("Insufficient balance");

            sender.Balance -= request.Amount;
            receiver.Balance += request.Amount;

            var senderTransaction = new Transaction {
                CustomerId = sender.CustomerId,
                Amount = -request.Amount,
                Timestamp = DateTime.Now,
                IsSuccessful = true
            };
            var receiverTransaction = new Transaction {
                CustomerId = receiver.CustomerId,
                Amount = request.Amount,
                Timestamp = DateTime.Now,
                IsSuccessful = true
            };

            _context.Transactions.Add(senderTransaction);
            _context.Transactions.Add(receiverTransaction);
            _context.SaveChanges();

            return Ok(new { message = "Transfer successful", senderNewBalance = sender.Balance, receiverNewBalance = receiver.Balance });
        }

    } 
    public class WithdrawRequest{
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}