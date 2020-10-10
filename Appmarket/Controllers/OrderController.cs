using Appmarket.Helpers;
using Appmarket.Models;
using Appmarket.Respositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

		public OrdersController(IOrderRepository orderRepository,ICombosHelper combosHelper)
		{
			this.orderRepository = orderRepository;
			this.combosHelper = combosHelper;
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

	}

}
