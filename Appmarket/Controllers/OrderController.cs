using Appmarket.Data;
using Appmarket.Data.Entities;
using Appmarket.Helpers;
using Appmarket.Models;
using Appmarket.Respositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appmarket.Controllers
{
	[Authorize]
	public class OrdersController : Controller
	{
		private readonly IOrderRepository orderRepository;
		private readonly ICombosHelper combosHelper;
		private readonly DataContext _context;

		public OrdersController(IOrderRepository orderRepository,ICombosHelper combosHelper, DataContext context)
		{
			this.orderRepository = orderRepository;
			this.combosHelper = combosHelper;
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var model = await orderRepository.GetOrdersAsync(this.User.Identity.Name);
			return View(model);
		}

		public async Task<IActionResult> Create()
		{
			var model = await this.orderRepository.GetDetailTempsAsync(this.User.Identity.Name);
			return this.View(model);
		}

		public IActionResult AddProduct()
		{
			var model = new AddItemViewModel
			{
				Quantity = 1,
				Products = this.combosHelper.GetComboProducts()
			};

			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> AddProduct(AddItemViewModel model)
		{
			if (this.ModelState.IsValid)
			{
				await this.orderRepository.AddItemToOrderAsync(model, this.User.Identity.Name);
				return this.RedirectToAction("Create");
			}

			return this.View(model);
		}

		public async Task<IActionResult> DeleteItem(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			await this.orderRepository.DeleteDetailTempAsync(id.Value);
			return this.RedirectToAction("Create");
		}

		public async Task<IActionResult> Increase(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			await this.orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, 1);
			return this.RedirectToAction("Create");
		}

		public async Task<IActionResult> Decrease(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			await this.orderRepository.ModifyOrderDetailTempQuantityAsync(id.Value, -1);
			return this.RedirectToAction("Create");
		}

		public async Task<IActionResult> ConfirmOrder()
		{
			var response = await this.orderRepository.ConfirmOrderAsync(this.User.Identity.Name);
			if (response)
			{
				return this.RedirectToAction("Index");
			}

			return this.RedirectToAction("Create");
		}

		public async Task<IActionResult> Deliver(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var order = await this.orderRepository.GetOrdersAsync(id.Value);
			if (order == null)
			{
				return NotFound();
			}

			var model = new DeliverViewModel
			{
				Id = order.Id,
				DeliveryDate = DateTime.Today
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Deliver(DeliverViewModel model)
		{
			if (this.ModelState.IsValid)
			{
				await this.orderRepository.DeliverOrder(model);
				return this.RedirectToAction("Index");
			}

			return this.View();
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Order order = await _context.Orders
				.Include(o => o.User)
				.ThenInclude(u => u.City)
				.Include(o => o.Items)
				.ThenInclude(od => od.Product)
				.ThenInclude(od => od.Category)
				.Include(o => o.Items)
				.ThenInclude(od => od.Product)
				.ThenInclude(od => od.ProductImages)
				.FirstOrDefaultAsync(o => o.Id == id);
			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}


	}

}
