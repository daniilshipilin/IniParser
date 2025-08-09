using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace IniParser.Tests;

[TestFixture]
public class Tests
{
    private const string EXAMPLEINISHA512 = "82327697e09473dbb9567749f127fd2e96ced5581b55ff413f6feabe49073701377f640b3cff4071b338b4e8aaa638e11d9bc86f54feda62c2c848245578cf62";

    private const string SECTION1 = "Numbers";
    private const string SECTION2 = "Strings";
    private const string SECTION3 = "Images";
    private const string NONEXISTENTSECTION = "NonExistentSection";
    private const string SECTIONWITHNOKEYS = "SectionWithNoKeys";
    private const string SECTIONWITHEMPTYKEY = "SectionWithEmptyKey";

    private const string KEY1 = "Binary";
    private const string KEY2 = "Octal";
    private const string KEY3 = "Decimal";
    private const string KEY4 = "Hex";
    private const string KEY5 = "Text";
    private const string KEY6 = "Base64";
    private const string KEY7 = "MD5";
    private const string KEY8 = "SHA1";
    private const string KEY9 = "Image";
    private const string KEY10 = "ImageSHA512";
    private const string EMPTYKEY = "EmptyKey";

    private const string VALUE1 = "11001010";
    private const string VALUE2 = "312";
    private const string VALUE3 = "202";
    private const string VALUE4 = "CA";
    private const string VALUE5 = "Hello";
    private const string VALUE6 = "SGVsbG8=";
    private const string VALUE7 = "8b1a9953c4611296a827abf8c47804d7";
    private const string VALUE8 = "f7ff9e8b7bb2e09b70935a5d785e0cc5d9d0abf0";
    private const string VALUE9 = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMjHxIGmVAAAElElEQVRYR9VXW0hsZRQe65xRx/ttdC6OM6OOHm8J3htwvJR2vIsoIoyKD2ooSqJoIoyjaB68RGSWVCg9BBH4EBGCWD2cRzkPig9BRASdc6DoBiXEjKv1/e1tG9NxztEGWvDx7/1f1lr/+tf6/r1V/wv58Y3iSHI4bhGpgqSuwAmpVEGe9wxO7yfWhT9X9M+Ty3FLGgqM0Gb/bc/7hlc83xY+8u4lf/5LlylGGgqc0GumAu9nOfe99xJWqT1LLXUHTn5bscWfulJKT3eGbO3t7c9yV4CP4VOtte1uyV17adGHxcXFj3Nych5wd+ASsqGhwWQ2WzzR0dGk0WgoNjaWrFZrFQ/hOJ4Rk/5Lcblc6tbW1keJiYkUExNDBoOB8vLyPFlZWZ7MzMw/UlNTHyQnJ69rtdoXefrNH4/ZbH6O5Ss2Rna7nUZHR2ljY4O2trZodXWVxsbGqK2tjYqKishkMh0xXpCWXl9sNtvL7MBJZWUlDQ4O0uTkJK2trdHe3p7A9vY2zc66aWRkhIaHh6mxsZFSU9O8HJXXpYR9agnKzc11p6WlUU1NDfX09FBXVxe1tLQQ5wQ1NTURGxBjDoeD6urqxHtvb68YwzqO2AdP7QSHs4d3f4qQQ3F1dTXOnTIyMpCAZ7BYLAIcJbLZMsQRyfNTUsxUUFCwKKn0X3hxHJfbTzhz7Ky0tJSVpVBSUtKFQHICWm0C6fV64nwR67hcKTs729PZ2VkiqfZP+vv7l7EYKCkpIZ1OJ7IfZYj2ImAsKiqKIiMjRZlyhYj1cIz17Uqqr5bl5eWw2dnZX/V6AyUkJAiEh4cLhIWF/atVAhwB4BkOxcfHi3nd3d2nR0dHdyQTvmV/f79uenpa7CQkJMQvBAcHCyjflW1hYSEdHh66JBO+5fj42I2aDg0NPVMMqNXqs1Z+Pv9+Wcu8QLu7u19IJnzLwcHBO2VlZSKUWHwRlDv1B8gR5ovHrP7q+2NnZ2cLiYczjIiIODt/+Rnne75POaYE+nCUyKOlpXs/M6VffW+sr6+/VVtbK2ocnM/8LhQgoeSkVL4r+/EsIy4uTlQAwo9ynpqaesjqr47A3Nzcq2A8JhAmFhsZjUZR26iKv1vf0OkAnXAeBMVsSuXl5cSJfV8y4VvcbnfZ0NCQWITFUAInnhQgrszMO4JHmpubaWZm5k3JhG/Z3Ny8zd5+X19fT8yGgnqhjK/bM8CA3BqNcvsPEHbcBfn5+VRVVUV9fX20uLj4kmTiallaWppyOrupoqJC0CrfbMIgwgpcFHoZGIfDYEJQOC6miYmJI78SUBaOgmZ8fPwb3HRwwGKxCuVISGWiKRMOwDPmIArp6eki/E6n07uwsFAtqfZf5ufny9n7EyQikgoGZK5H2V0GzIEjcBjODwwMrEgqn1w6OjpaOAlPQCRQjtqW+R4AW56HXP9YwxT8MYf+ep9ozAd23vXD8wZlNpQh92EenOBr+l3+ULmZ70NWqGW8zcp/v2zngCZUc8rjx3wdt/Oym/9s52OIY0MDbOQjxtf8/AO33zG+ZAfnOfR2nhawnxbsEMau8V+gUv0FJyWg+o5wz/8AAAAASUVORK5CYII=";
    private const string VALUE10 = "7df39ab6d5ea46e87b8a83870d05b202d946aa3999198ed543350c6ed211289e4fd1b289bc82190daf2ed41e1005b0bfaf98fc6e7d761d558129a35a529ff5b5";

    private const string DEFAULTINIFILEPATH = @".\Sample\Sample.ini";
    private const string TMPDIR = @".\Sample\TMP";

    private string tmpIniFilePath = string.Empty;
    private Lib.IParser ini = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        try
        {
            if (!Directory.Exists(TMPDIR))
            {
                Directory.CreateDirectory(TMPDIR);
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.ToString());
        }
    }

    [SetUp]
    public void Setup()
    {
        try
        {
            this.tmpIniFilePath = Path.Combine(TMPDIR, $"{Path.GetRandomFileName()}.ini");
            File.Copy(DEFAULTINIFILEPATH, this.tmpIniFilePath);
            ini = new Lib.IniParser(this.tmpIniFilePath);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.ToString());
        }
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (File.Exists(this.tmpIniFilePath))
            {
                File.Delete(this.tmpIniFilePath);
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.ToString());
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        try
        {
            if (Directory.Exists(TMPDIR))
            {
                Directory.Delete(TMPDIR);
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.ToString());
        }
    }

    /// <summary>
    /// Test #1 - IniSuccessfullyParsed.
    /// </summary>
    [Test]
    public void SampleIniSuccessfullyParsed()
    {
        if (ini is not null)
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    /// <summary>
    /// Test #2 - IniHashMatchesBaseHash.
    /// </summary>
    [Test]
    public void SampleIniHashMatchesPrecomputedHash()
    {
        byte[] iniBytes = File.ReadAllBytes(DEFAULTINIFILEPATH);
        string resultHash = GenerateSHA512String(iniBytes);

        if (EXAMPLEINISHA512.Equals(resultHash))
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    /// <summary>
    /// Test #3 - CheckIfKeyValuesMatch.
    /// </summary>
    [Test]
    public void CheckIfKeyValuesMatch()
    {
        if (VALUE1.Equals(ini.GetValue(SECTION1, KEY1)) &&
            VALUE2.Equals(ini.GetValue(SECTION1, KEY2)) &&
            VALUE3.Equals(ini.GetValue(SECTION1, KEY3)) &&
            VALUE4.Equals(ini.GetValue(SECTION1, KEY4)) &&
            VALUE5.Equals(ini.GetValue(SECTION2, KEY5)) &&
            VALUE6.Equals(ini.GetValue(SECTION2, KEY6)) &&
            VALUE7.Equals(ini.GetValue(SECTION2, KEY7)) &&
            VALUE8.Equals(ini.GetValue(SECTION2, KEY8)) &&
            VALUE9.Equals(ini.GetValue(SECTION3, KEY9)) &&
            VALUE10.Equals(ini.GetValue(SECTION3, KEY10)))
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    /// <summary>
    /// Test #4 - SetKeyValues.
    /// </summary>
    [Test]
    public void SetKeyValues()
    {
        ini.SetValue(SECTION1, KEY1, VALUE1);
        ini.SetValue(SECTION1, KEY2, VALUE2);
        ini.SetValue(SECTION1, KEY3, VALUE3);
        ini.SetValue(SECTION1, KEY4, VALUE4);
        ini.SetValue(SECTION2, KEY5, VALUE5);
        ini.SetValue(SECTION2, KEY6, VALUE6);
        ini.SetValue(SECTION2, KEY7, VALUE7);
        ini.SetValue(SECTION2, KEY8, VALUE8);
        ini.SetValue(SECTION3, KEY9, VALUE9);
        ini.SetValue(SECTION3, KEY10, VALUE10);

        ini.SaveIni();
        ini.ReloadIni();

        this.CheckIfKeyValuesMatch();

        Assert.Fail();
    }

    /// <summary>
    /// Test #5 - DeleteKeys.
    /// </summary>
    [Test]
    public void DeleteKeys()
    {
        bool deleted_1 = ini.DeleteKey(SECTION1, KEY1);
        bool deleted_2 = ini.DeleteKey(SECTION1, KEY2);
        bool deleted_3 = ini.DeleteKey(SECTION1, KEY3);
        bool deleted_4 = ini.DeleteKey(SECTION1, KEY4);
        bool deleted_5 = ini.DeleteKey(SECTION2, KEY5);
        bool deleted_6 = ini.DeleteKey(SECTION2, KEY6);
        bool deleted_7 = ini.DeleteKey(SECTION2, KEY7);
        bool deleted_8 = ini.DeleteKey(SECTION2, KEY8);
        bool deleted_9 = ini.DeleteKey(SECTION3, KEY9);
        bool deleted_10 = ini.DeleteKey(SECTION3, KEY10);

        ini.SaveIni();
        ini.ReloadIni();

        if (!VALUE1.Equals(ini.GetValue(SECTION1, KEY1)) && deleted_1 &&
            !VALUE2.Equals(ini.GetValue(SECTION1, KEY2)) && deleted_2 &&
            !VALUE3.Equals(ini.GetValue(SECTION1, KEY3)) && deleted_3 &&
            !VALUE4.Equals(ini.GetValue(SECTION1, KEY4)) && deleted_4 &&
            !VALUE5.Equals(ini.GetValue(SECTION2, KEY5)) && deleted_5 &&
            !VALUE6.Equals(ini.GetValue(SECTION2, KEY6)) && deleted_6 &&
            !VALUE7.Equals(ini.GetValue(SECTION2, KEY7)) && deleted_7 &&
            !VALUE8.Equals(ini.GetValue(SECTION2, KEY8)) && deleted_8 &&
            !VALUE9.Equals(ini.GetValue(SECTION3, KEY9)) && deleted_9 &&
            !VALUE10.Equals(ini.GetValue(SECTION3, KEY10)) && deleted_10)
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    /// <summary>
    /// Test #6 - Check for non existent section.
    /// </summary>
    [Test]
    public void CheckNonExistentSection()
    {
        if (ini.GetSectionKeysAndValues(NONEXISTENTSECTION).Count == 0)
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    /// <summary>
    /// Test #7 - Check for empty section.
    /// </summary>
    [Test]
    public void CheckEmptySection()
    {
        if (ini.GetSectionKeysAndValues(SECTIONWITHNOKEYS).Count == 0)
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    /// <summary>
    /// Test #8 - Check for section with empty key.
    /// </summary>
    [Test]
    public void CheckSectionWithEmptyKey()
    {
        var section = ini.GetSectionKeysAndValues(SECTIONWITHEMPTYKEY);

        if (string.IsNullOrEmpty(section[EMPTYKEY]))
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    /// <summary>
    /// Generates SHA512 hash represented as hex string (lowercase).
    /// </summary>
    private static string GenerateSHA512String(byte[] inputBytes)
    {
        byte[] hashBytes = SHA512.HashData(inputBytes);
        var sb = new StringBuilder();

        foreach (byte hashByte in hashBytes)
        {
            sb.Append(hashByte.ToString("x2"));
        }

        return sb.ToString();
    }
}
