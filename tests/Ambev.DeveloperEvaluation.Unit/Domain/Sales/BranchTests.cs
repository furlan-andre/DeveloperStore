using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Unit.Domain.Sales.Builders;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public class BranchTests
{
    [Fact(DisplayName = "Should create valid branch")]
    public void Given_ValidData_When_CreatingBranch_Then_ShouldCreateBranch()
    {
        var branch = ReferenceDataTestBuilder.CreateBranch();

        Assert.NotNull(branch);
    }

    [Fact(DisplayName = "Should keep informed branch id")]
    public void Given_ValidId_When_CreatingBranch_Then_ShouldKeepId()
    {
        var id = Guid.NewGuid();
        var name = "Main Branch";

        var branch = new Branch(id, name);

        Assert.Equal(id, branch.Id);
    }

    [Fact(DisplayName = "Should keep informed branch name")]
    public void Given_ValidName_When_CreatingBranch_Then_ShouldKeepName()
    {
        var name = "Main Branch";
        var id = Guid.NewGuid();

        var branch = new Branch(id, name);

        Assert.Equal(name, branch.Name);
    }

    [Fact(DisplayName = "Should throw DomainException when branch id is empty")]
    public void Given_EmptyId_When_CreatingBranch_Then_ShouldThrowDomainException()
    {
        var id = Guid.Empty;
        var name = "Main Branch";

        var action = () => new Branch(id, name);

        Assert.Throws<DomainException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when branch name is invalid")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidName_When_CreatingBranch_Then_ShouldThrowDomainException(string? name)
    {
        var id = Guid.NewGuid();

        var action = () => new Branch(id, name);

        Assert.Throws<DomainException>(action);
    }
}
