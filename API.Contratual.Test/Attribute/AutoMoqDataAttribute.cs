using API.Contratual.Test.Helper;
using AutoFixture.Xunit2;

namespace API.Contratual.Test.Attribute;

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute() : base(() => AutoMoqFixtureFactory.CreateFixture()) { }
}