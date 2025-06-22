using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.DbContext;
using Newtonsoft.Json.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace WebApp.Controllers
{
	public class CartOrderController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IHttpClientFactory _clientFactory;
		public CartOrderController(ApplicationDbContext context, IHttpClientFactory clientFactory)
		{
			_context = context;
			_clientFactory = clientFactory;
		}



		[HttpGet]
		public async Task<IActionResult> CheckPayment(int id)
		{
			Console.WriteLine("111111111111----" + id);
			try
			{
				// Tìm đơn hàng theo ID
				var order = await _context.Orders
					.FirstOrDefaultAsync(o => o.OrderID == id);

				if (order == null)
				{
					return NotFound($"Order with ID {id} not found.");
				}
				if(order.Status == "Pair")
				{
					return Ok(new { message = $"Payment verified and order status updated to Paid for order ID {id}.", status = "success" });

				}
				decimal totalAmount = order.TotalAmount;

				// Gửi yêu cầu đến localhost/lsgd.php để lấy thông tin giao dịch
				var response = await SendRequestToLSGD(totalAmount, id);

				if (response == null)
				{
					return StatusCode(500, "Failed to fetch transaction data from LSGD.");
				}

				// Kiểm tra giao dịch và cập nhật trạng thái đơn hàng nếu thành công
				bool paymentVerified = VerifyPayment(response, id, totalAmount);

				if (paymentVerified)
				{
					// Cập nhật trạng thái đơn hàng thành "Paid"
					order.Status = "Paid";
					_context.Orders.Update(order);
					await _context.SaveChangesAsync();

					return Ok(new { message = $"Payment verified and order status updated to Paid for order ID {id}." , status = "success"});
				}
				else
				{
					return BadRequest("No matching transaction found for the specified amount.");
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		private async Task<JObject> SendRequestToLSGD(decimal amount, int orderId)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/lsgd.php");
			var requestBody = new
			{
				mid = "14",
				code = "00",
				des = "success",
				clientIp = "118.69.159.154",
				// Thêm dữ liệu giao dịch nếu cần thiết
			};
			request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

			var client = _clientFactory.CreateClient();
			var response = await client.SendAsync(request);
            Console.WriteLine(response);
            if (response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStringAsync();
				Console.WriteLine(responseBody);
				return JObject.Parse(responseBody);
			}
			else
			{
				return null;
			}
		}

		private bool VerifyPayment(JObject response, int orderId, decimal expectedAmount)
		{
			try
			{
				var transactions = response["transactions"].Children().ToList();
				var matchingTransaction = transactions.FirstOrDefault(t =>
					t["Description"].ToString().Contains($"THANHTOAN{orderId}") &&
					t["Amount"].ToString() == expectedAmount.ToString("N0")
				);

				return matchingTransaction != null;
			}
			catch (Exception)
			{
				return false;
			}
		}


		[HttpPost]
		public async Task<IActionResult> CreateOrder([FromBody] OrderCreationViewModel model)
		{
			try
			{
				// Kiểm tra số lượng tồn kho của từng sản phẩm và lấy giá của sản phẩm từ cơ sở dữ liệu
				foreach (var item in model.CartItems)
				{
					var product = await _context.Products.FindAsync(item.id);
					if (product == null)
					{
						return BadRequest($"Product with ID {item.id} not found.");
					}

					if (product.StockQuantity < item.quantity)
					{
						return BadRequest($"Not enough stock for product {product.ProductName}. Available: {product.StockQuantity}, requested: {item.quantity}");
					}

					// Lấy giá của sản phẩm từ cơ sở dữ liệu để tính tổng tiền của đơn hàng
					item.price = product.Price;
				}

				// Tạo đối tượng Order từ dữ liệu nhận được
				var order = new Order
				{
					CustomerID = 1, // Thay thế bằng logic lấy CustomerID từ dữ liệu người dùng
					OrderDate = DateTime.Now,
					TotalAmount = model.CartItems.Sum(item => item.quantity * item.price),
					ShippingAddress = $"{model.ShippingInfo.address}, {model.ShippingInfo.ward}, {model.ShippingInfo.district}, {model.ShippingInfo.province}",
					Status = "Pending", // Trạng thái mặc định
					OrderDetails = model.CartItems.Select(item => new OrderDetail
					{
						ProductID = item.id,
						Quantity = item.quantity,
						UnitPrice = item.price
					}).ToList()
				};

				// Trừ số lượng tồn kho sau khi order thành công
				foreach (var item in order.OrderDetails)
				{
					var product = await _context.Products.FindAsync(item.ProductID);
					if (product != null)
					{
						product.StockQuantity -= item.Quantity;
					}
				}

				// Thêm order vào context và lưu vào cơ sở dữ liệu
				_context.Orders.Add(order);
				await _context.SaveChangesAsync();

				// Trả về ID của đơn hàng vừa tạo thành công
				return Ok(new { OrderID = order.OrderID, TotalAmount = order.TotalAmount });
			}
			catch (Exception ex)
			{
				// Xử lý khi có lỗi xảy ra
				return BadRequest(ex.Message);
			}
		}



	}

	// ViewModel để nhận dữ liệu từ front end
	public class OrderCreationViewModel
	{
		public int CustomerID { get; set; } // Thay thế bằng các thuộc tính cần thiết từ cookie của bạn
		public ShippingInfo ShippingInfo { get; set; }
		public List<CartItem> CartItems { get; set; }
	}

	public class ShippingInfo
	{
		public string fullName { get; set; }
		public string address { get; set; }
		public string ward { get; set; }
		public string district { get; set; }
		public string province { get; set; }
		public string phoneNumber { get; set; }
		public string email { get; set; }
	}

	public class CartItem
	{
		public int id { get; set; }
		public string name { get; set; }
		public decimal price { get; set; }
		public int quantity { get; set; }
		public string image { get; set; }
	

	}
}
