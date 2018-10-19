using System.Linq;
using NUnit.Framework;
using Photon.Hive;
using Photon.Hive.Tests;

namespace PhotonHive.Tests
{
    [TestFixture]
    public class ActorManagerTests
    {
        [Test]
        public void ActorsGetActorsByNumbersTest()
        {
            var manager = new TestActorsManager();

            manager.AddInactive(new TestActor(1, string.Empty));
            manager.AddInactive(new TestActor(2, string.Empty));
            manager.AddInactive(new TestActor(3, string.Empty));
            manager.AddInactive(new TestActor(4, string.Empty));

            var actorIds = new int[] {1, 2, 3, 4};

            var actors = manager.ActorsGetActorsByNumbers(actorIds);
            Assert.IsEmpty(actors);
            Assert.AreEqual(0, actors.Count());
        }
    }
}
