using System.Text;
using DollarEnv.Module;

namespace DollarEnv.Tests;

public class FileParserTests
{
    [Fact]
    public void FileParser_Ignores_CommentsOnOwnLine()
    {
        var stream = StreamOf(
@"# this is just a comment
THIS_IS_A_VARIABLE=1");

        var result = FileParser.ParseVariables(stream);
        
        Assert.Single(result);
        Assert.Equal("1", result["THIS_IS_A_VARIABLE"]);
    }
    
    [Fact]
    public void FileParser_Ignores_CommentsOnSameLine()
    {
        var stream = StreamOf("THIS_IS_A_VARIABLE=1 # this is just a comment");

        var result = FileParser.ParseVariables(stream);
        
        Assert.Single(result);
        Assert.Equal("1", result["THIS_IS_A_VARIABLE"]);
    }
    
    [Fact]
    public void FileParser_Includes_PoundSignWithinQuotes()
    {
        var stream = StreamOf("VARIABLE=\"include text after # sign\"");

        var result = FileParser.ParseVariables(stream);
        
        Assert.Single(result);
        Assert.Equal("include text after # sign", result["VARIABLE"]);
    }
    
    [Fact]
    public void FileParser_ConvertsEscapedNewlinesToUnescapedWithinQuotes()
    {
        var stream = StreamOf("VARIABLE=\"first line\\r\\nsecond line\"");

        var result = FileParser.ParseVariables(stream);
        
        Assert.Single(result);
        Assert.Equal("first line\r\nsecond line", result["VARIABLE"]);
    }
    
    [Fact]
    public void FileParser_Includes_MultiLineValues()
    {
        var text =
@"MULTILINE=""first line
second line"""
            .ReplaceLineEndings();

        var result = FileParser.ParseVariables(StreamOf(text));
        
        Assert.Single(result);
        Assert.Equal($"first line{Environment.NewLine}second line", result["MULTILINE"]);
    }
    
    [Fact]
    public void FileParser_AcceptsUnderscoresInKey()
    {
        var stream = StreamOf("HAS_UNDERSCORES=1");

        var result = FileParser.ParseVariables(stream);
        
        Assert.Single(result);
        Assert.Equal($"1", result["HAS_UNDERSCORES"]);
    }
    
    // Leave variables as whatever case they are. User will need to know if variables are
    // case sensitive in their environment (win vs unix etc.)
    [Fact]
    public void FileParser_AcceptsLowercaseKeyWithoutChange()
    {
        var stream = StreamOf("a=1");

        var result = FileParser.ParseVariables(stream);
        
        Assert.Single(result);
        Assert.Equal($"1", result["a"]);
    }
    
    [Fact]
    public void FileParser_ReturnsLastValue()
    {
        var text =
@"a=1
a=b"
                .ReplaceLineEndings();

        var result = FileParser.ParseVariables(StreamOf(text));
        
        Assert.Single(result);
        Assert.Equal($"b", result["a"]);
    }

    [Fact]
    public void FileParser_AllTogetherNow()
    {
        var text =
@"# my dotenv file
A=1
B=something # or is it?

MULTILINE=""first line
second line""".ReplaceLineEndings();

        var result = FileParser.ParseVariables(StreamOf(text));
        
        Assert.Equal(3, result.Count);
        Assert.Equal($"first line{Environment.NewLine}second line", result["MULTILINE"]);
    }
    
    private static Stream StreamOf(string text) => new MemoryStream(Encoding.UTF8.GetBytes(text));
}