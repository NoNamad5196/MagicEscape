using MagicEscape.Core;
using Xunit;
public class KoreanParticleTests
{
    [Theory]
    [InlineData("불도마뱀 비늘", "불도마뱀 비늘을")]
    [InlineData("월광초 꽃잎", "월광초 꽃잎을")]
    [InlineData("수정 가루", "수정 가루를")]
    [InlineData("독버섯", "독버섯을")]
    [InlineData("푸른 수정 (물)", "푸른 수정 (물)을")]
    [InlineData("붉은 수정 (불)", "붉은 수정 (불)을")]
    [InlineData("계시의 물약", "계시의 물약을")]
    [InlineData("", "를")]
    [InlineData(null, "를")]
    public void ObjectParticle(string name, string expected) => Assert.Equal(expected, KoreanParticle.Object(name));
    [Fact] public void OtherParticles()
    {
        Assert.Equal("가루가", KoreanParticle.Subject("가루"));
        Assert.Equal("비늘이", KoreanParticle.Subject("비늘"));
        Assert.Equal("가루와", KoreanParticle.With("가루"));
        Assert.Equal("비늘과", KoreanParticle.With("비늘"));
        Assert.True(KoreanParticle.HasFinalConsonant("꽃잎  "));
        Assert.False(KoreanParticle.HasFinalConsonant("Ruby"));
    }
}
