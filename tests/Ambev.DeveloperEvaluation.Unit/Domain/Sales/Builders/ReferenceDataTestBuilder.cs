using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales.Builders;

public static class ReferenceDataTestBuilder
{
    private static readonly Faker Faker = new();

    public static Customer CreateCustomer()
    {
        return new Customer(Guid.NewGuid(), Faker.Person.FullName);
    }

    public static Branch CreateBranch()
    {
        return new Branch(Guid.NewGuid(), Faker.Company.CompanyName());
    }

    public static Product CreateProduct()
    {
        return new Product(Guid.NewGuid(), Faker.Commerce.ProductName());
    }
}
