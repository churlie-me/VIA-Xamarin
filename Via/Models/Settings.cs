using System.Collections.Generic;

namespace Via.Models
{
    public class Cultures
{
    public string __invalid_name__en_GB { get; set; }
    public string __invalid_name__fr_FR { get; set; }
    public string __invalid_name__nl_NL { get; set; }
}


public class PasswordRules
{
    public int minimalCharacters { get; set; }
    public bool needsUpperCaseLetter { get; set; }
    public bool needsLowerCaseLetter { get; set; }
    public bool needsDecimalDigit { get; set; }
}

public partial class Settings
{
    public Cultures cultures { get; set; }
    public PasswordRules passwordRules { get; set; }
    public List<string> mandatoryFields { get; set; }
    public List<string> speedUnits { get; set; }
    public List<string> genders { get; set; }
    public List<string> mailIntervals { get; set; }
}
}