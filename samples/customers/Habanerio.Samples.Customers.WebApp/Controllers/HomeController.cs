using Habanerio.Samples.Customers.MongoEFCore.Entities;
using Habanerio.Samples.Customers.MongoEFCore.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Samples.WebApp.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly ICustomersRepository _customersRepository;

    public HomeController(ICustomersRepository customersRepository, ILogger<HomeController> logger)
    {
        _customersRepository = customersRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var customers = await _customersRepository.SearchAsync(pageNo: 1, pageSize: 25, cancellationToken: cancellationToken);

        return View(customers);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string firstName = "", string lastName = "", string email = "", CancellationToken cancellationToken = default)
    {
        var customers = await _customersRepository.SearchAsync(firstName, lastName, email, 1, 25, cancellationToken);

        return View(customers);
    }

    [Route("details/{id}")]
    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken = default)
    {
        var customer = await _customersRepository.Find(id, cancellationToken);

        return View(customer);
    }

    [HttpPost]
    [Route("details/{id}")]
    public async Task<IActionResult> Details(CustomerDbEntity customer, CancellationToken cancellationToken = default)
    {
        _customersRepository.Update(customer, cancellationToken);

        await _customersRepository.SaveChangesAsync(cancellationToken);

        return View(customer);
    }

    [Route("populate")]
    public async Task<IActionResult> Populate()
    {
        var firstNames = new[] { "John", "Jane", "Jack", "Jill", "Jim", "Jenny", "Michael", "Johnny", "Jason" };
        var lastNames = new[] { "Doe", "Smith", "Johnson", "Brown", "White", "Black", "Green", "Blue", "Red", "Orange", "Pink" };

        var i = 1;
        var j = 1;
        foreach (var firstName in firstNames)
        {
            foreach (var lastName in lastNames)
            {
                var customer = new CustomerDbEntity()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = $"{firstName.Substring(0, 1)}{lastName.Substring(0, 1)}{i}{j}@samples.com"
                };

                _customersRepository.Add(customer);

                j++;
            }

            i++;
        }

        await _customersRepository.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
