using AutoFixture;
using AutoFixture.AutoMoq;

namespace API.Contratual.Test.Helper;

public static class AutoMoqFixtureFactory
{
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        fixture?.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        return fixture;
    }
}